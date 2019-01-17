using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Irixi_Aligner_Common.UserScript.Implementation
{
    [Serializable]
    public class UserScriptMove : UserScriptBase
    {
        #region Variables

        LogicalAxis _axis;
        MoveMode _mode = MoveMode.REL;
        double _distance = 0;
        int _speed = 100;

        string _axisHashString;

        #endregion

        #region Constructors

        public UserScriptMove():base()
        {
            
        }

        public UserScriptMove(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _axisHashString = (string)info.GetValue("Axis", typeof(string));
            Mode = (MoveMode)info.GetValue("Mode", typeof(MoveMode));
            Distance = (double)info.GetValue("Distance", typeof(double));
            Speed = (int)info.GetValue("Speed", typeof(int));
        }

        #endregion

        #region Properties

        public override string Name => "Move";

        public override string Usage => "Move the specified axis.";

        [DisplayFormat(NullDisplayText = "Axis not set"), Display(GroupName = "Common")]
        public LogicalAxis Axis
        {
            get
            {
                return _axis;
            }
            set
            {
                UpdateProperty(ref _axis, value);
            }
        }
        
        [Display(GroupName = "Common")]
        public MoveMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                UpdateProperty(ref _mode, value);
            }
        }

        [Display(GroupName = "Common")]
        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                UpdateProperty(ref _distance, value);
            }

        }

        [Display(GroupName = "Common"), DisplayName("Speed(%)")]
        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                UpdateProperty(ref _speed, value);
            }
        }

        #endregion

        #region Methods

        protected override string ChildCreateSummary()
        {
            if (IsError)
                return "";
            else
                return $"{Axis}, {Mode}, distance {Distance}{Axis.PhysicalAxisInst.UnitHelper}, speed {Speed}%";
        }

        protected override void CreatePropertiesAllowTemplated()
        {
            PropertiesAllowTemplated.Add(new Property("Axis"));
        }

        public override void RecoverReferenceTypeProperties()
        {
            try
            {
                this.Axis = Service.FindLogicalAxisByHashString(_axisHashString);
            }
            catch
            {
                this.Axis = null;
                ErrorMessage = $"Unable to find axis with hash string {_axisHashString}.";
            }

            base.RecoverReferenceTypeProperties();
        }


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Axis", Axis.HashString, typeof(string));
            info.AddValue("Mode", Mode, typeof(MoveMode));
            info.AddValue("Distance", Distance, typeof(double));
            info.AddValue("Speed", Speed, typeof(int));
        }

        protected override void ChildPerform()
        {
            if (Axis.PhysicalAxisInst.Move(Mode, Speed, Distance) == false)
            {
                throw new Exception($"Unable to move {Axis}, {Axis.PhysicalAxisInst.LastError}");
            }
        }

        public override void Validate()
        {
            if (this.Axis == null)
            {
                ErrorMessage = "The axis can not be null.";
            }
            else if(this.Speed < 1 || this.Speed > 100)
            {
                ErrorMessage = "The speed must be between 1 to 100.";
            }
            else if(this.Distance == 0)
            {
                ErrorMessage = "The distance can not be 0.";
            }
            else
            {
                ErrorMessage = MSG_PASSED;
            }
        }
        
        #endregion
    }
}
;