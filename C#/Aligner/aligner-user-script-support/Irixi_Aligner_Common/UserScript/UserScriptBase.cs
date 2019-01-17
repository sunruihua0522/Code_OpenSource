using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Irixi_Aligner_Common.UserScript
{
    /// <summary>
    /// The status indicate whether the command has 
    /// been executed or not, or errors occured.
    /// </summary>
    public enum UserScriptExecStatus
    {
        NotExecuted,
        Executing,
        Executed,
        Error
    }

    [Serializable]
    public class UserScriptBase : NotifyPropertyChangedBase, IUserScript
    {
        #region Variables 
        protected const string MSG_PASSED = "Pass";
        int _order;
        string _summary = "";
        bool _isError = true;
        string _errMessage = "";
        UserScriptExecStatus _execStatus = UserScriptExecStatus.NotExecuted;

        #endregion

        #region Construtors

        public UserScriptBase()
        {
            PropertiesAllowTemplated = new List<Property>();

            CreatePropertiesAllowTemplated();

            Validate();
            UpdateSummary();
        }

        public UserScriptBase(SerializationInfo info, StreamingContext context)
        {
            PropertiesAllowTemplated = new List<Property>();
            CreatePropertiesAllowTemplated();

            Order = (int)info.GetValue("Order", typeof(int));
        }

        #endregion

        #region Properties

        [Browsable(false), ReadOnly(false)]
        public List<Property> PropertiesAllowTemplated { get; set; }

        [Browsable(false), ReadOnly(true)]
        public virtual string Name { get; }

        [Browsable(false), ReadOnly(true)]
        public virtual string Usage => "";

        [Browsable(false)]
        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
                OnPropertyChanged();
            }
        }

        [Browsable(false), ReadOnly(true)]
        public SystemService Service { get; set; }

        [Browsable(false), ReadOnly(true)]
        public string Summary
        {
            get
            {
                return _summary;
            }
        }

        [Browsable(false), ReadOnly(true)]
        public UserScriptExecStatus ExecStatus
        {
            get
            {
                return _execStatus;
            }
             set
            {
                _execStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Get whether the arguments are valid.
        /// <para>If it's not, get the error message from the ErrorMessage property.</para>
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public bool IsError
        {
            get
            {
                return _isError;
            }
            set
            {
                _isError = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Get the error message which is created by the Validate() method.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public string ErrorMessage
        {
            get
            {
                return _errMessage;
            }
            protected set
            {
                if (value == MSG_PASSED)
                    IsError = false;
                else
                    IsError = true;

                _errMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods should be implemented by the child

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Order", Order, typeof(int));
        }

        protected virtual string ChildCreateSummary() { return ""; }

        /// <summary>
        /// Create the list containing the property to be templated in the property grid control.
        /// </summary>
        protected virtual void CreatePropertiesAllowTemplated() { }

        /// <summary>
        /// In the object deserialization, the reference-type-properties which are relevant to SystemService can 
        /// not be recovered since the SystemService Property is null.
        /// <para>This method should be called in the Load()<see cref="UserScriptManager.Load(string)"/> method 
        /// of the user script manager in order to recover those properties.</para>
        /// 
        /// </summary>
        public virtual void RecoverReferenceTypeProperties()
        {
            Validate();
            UpdateSummary();
        }

        protected virtual void ChildPerform() => throw new NotImplementedException();
        
        public virtual void Validate() => throw new NotImplementedException();

        #endregion

        #region  Methods

        protected void UpdateSummary()
        {
            _summary = ChildCreateSummary();
            OnPropertyChanged();
        }

        protected override void UpdateProperty<T>(ref T OldValue, T NewValue, [CallerMemberName] string PropertyName = "")
        {
            base.UpdateProperty(ref OldValue, NewValue, PropertyName);

            Validate();
            UpdateSummary();
        }

        public void Perform()
        {
            try
            {
                this.ExecStatus = UserScriptExecStatus.Executing;
                ChildPerform();
                this.ExecStatus = UserScriptExecStatus.Executed;
                
            }
            catch(Exception ex)
            {
                this.ExecStatus = UserScriptExecStatus.Error;
                throw ex;
            }
        }

        public virtual void Reset()
        {
            this.ExecStatus = UserScriptExecStatus.NotExecuted;
        }


        public override string ToString()
        {
            return $"{Name}, {Usage}";
        }

        #endregion
    }
}
