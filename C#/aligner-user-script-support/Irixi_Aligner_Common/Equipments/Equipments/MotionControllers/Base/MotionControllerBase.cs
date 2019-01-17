using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Equipments.Base;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.MotionControllers.Base
{
    public class MotionControllerBase<T> : EquipmentBase, IMotionController, IList<T>, IDisposable
        where T: IAxis
    {
        #region Variables

        public event EventHandler OnMoveBegin;

        public event EventHandler OnMoveEnd;

        /// <summary>
        /// Axis list defined in the configuration file.
        /// </summary>
        List<T> listAxis;

        /// <summary>
        /// Lock the count of busy axis while setting or getting the property.
        /// </summary>
        readonly object lockBusyAxesCount = new object();


        string lastError = "";
        
        #endregion Variables

        #region Constructor

        public MotionControllerBase(ConfigPhysicalMotionController Config):base(Config)
        { 
            Model = Config.Model;
            BusyAxesCount = 0;

            listAxis = new List<T>();

            int i = 0;

            // generate axis objects according to the configuration.
            // they'll be bound to the actual motion controller later on while initializing the motion controller.
            foreach (var axisConfig in Config.AxisCollection)
            {
                T phyAxisDesired = (T)Activator.CreateInstance(typeof(T), axisConfig, this);  // new T(axisConfig, this);
                listAxis.Add(phyAxisDesired);

                i++;
            }
        }

        #endregion Constructor

        #region Properties

        public MotionControllerType Model { private set; get; }

        /// <summary>
        /// The property indicates that how many axes are moving.
        /// If it is 0, fire the event #OnMoveBegin before moving, and fire the event #OnMoveEnd after moving,
        /// This feature is especially used to tell #SystemService whether I(Motion Controller) am busy or not,
        /// I'll be added to #BusyComponent list once the first axis was moving and be removed once the last axis was stopped.
        /// In order to execute #Stop command ASAP, #SystemService only stops the components which are in the #BusyComponent list.
        /// </summary>
        public int BusyAxesCount { private set; get; }


        public override string HashString
        {
            get
            {
                return HashGenerator.GetHashSHA256(DeviceClass.ToString());
            }
        }

        #endregion Properties

        #region Methods

        public override bool Init()
        {
            if (this.IsEnabled)
            {
                var ret = InitProcess();
                if (ret == true)
                    this.IsInitialized = true;

                return ret;
            }
            else
            {
                this.LastError = "the controller is disabled.";
                return false;
            }
        }
        
        public IAxis GetAxisByName(string AxisName)
        {
            var phyAxis = listAxis.Where(a => a.AxisName == AxisName).First();
            return phyAxis;
        }

        public bool Home(IAxis Axis)
        {
            bool ret = false;

            if (!this.IsEnabled) // the controller is configured to be disabled in the config file
            {
                Axis.LastError = "the controller is disabled.";
            }
            else if (!this.IsInitialized)   // the controller is not initialized
            {
                Axis.LastError = "the controller is not initialized.";
            }
            else if (!Axis.IsEnabled)   // the axis moved is disabled in the config file
            {
                Axis.LastError = "the axis is disabled.";
            }
            else
            {
                if (BusyAxesCount <= 0)
                    OnMoveBegin?.Invoke(this, new EventArgs());

                IncreaceBusyAxesCount();
                ret = HomeProcess(Axis);
                DecreaceBusyAxesCount();

                if (BusyAxesCount <= 0)
                    OnMoveEnd?.Invoke(this, new EventArgs());
            }

            return ret;
        }

        public bool Move(IAxis Axis, MoveMode Mode, int Speed, int Distance)
        {
            bool ret = false;

            if (!this.IsEnabled)
            {
                Axis.LastError = "the controller is disabled.";
            }
            if (!this.IsInitialized)
            {
                Axis.LastError = "the controller is not initialized.";
            }
            else if (!Axis.IsEnabled)
            {
                Axis.LastError = "the axis is disabled.";
            }
            else if (!Axis.IsHomed)
            {
                Axis.LastError = "the axis is not homed.";
            }
            else
            {
                if (BusyAxesCount <= 0)
                    OnMoveBegin?.Invoke(this, new EventArgs());

                IncreaceBusyAxesCount();

                ret = MoveProcess(Axis, Mode, Speed, Distance);

                DecreaceBusyAxesCount();

                if (BusyAxesCount <= 0)
                    OnMoveEnd?.Invoke(this, new EventArgs());
            }

            return ret;
        }

        public bool MoveWithTrigger(IAxis Axis, MoveMode Mode, int Speed, int Distance, int Interval, int Channel)
        {
            throw new NotImplementedException();
        }

        public bool MoveWithInnerADC(IAxis Axis, MoveMode Mode, int Speed, int Distance, int Interval, int Channel)
        {
            throw new NotImplementedException();
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Customized process of initialization
        /// </summary>
        /// <returns></returns>
        protected virtual bool InitProcess()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Customized process to home
        /// </summary>
        /// <param name="Axis">The axis to home</param>
        /// <returns></returns>
        protected virtual bool HomeProcess(IAxis Axis)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Customized process to move
        /// </summary>
        /// <param name="Axis">The axis to move</param>
        /// <param name="Mode">Rel/Abs</param>
        /// <param name="Speed">0 ~ 100 in percent</param>
        /// <param name="Distance">The distance to move in steps</param>
        /// <returns></returns>
        protected virtual bool MoveProcess(IAxis Axis, MoveMode Mode, int Speed, int Distance)
        {
            throw new NotImplementedException();
        }

        public sealed override string ToString()
        {
            return $"*{this.Model}@{this.Port}*";
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion Methods

        #region Private Methods

        private void IncreaceBusyAxesCount()
        {
            lock (lockBusyAxesCount)
            {
                BusyAxesCount++;
            }
        }

        private void DecreaceBusyAxesCount()
        {
            lock (lockBusyAxesCount)
            {
                BusyAxesCount--;
            }
        }

        #endregion Private Methods

        #region Implementation of IList

        public int Count => ((IList<T>)listAxis).Count;

        public bool IsReadOnly => ((IList<T>)listAxis).IsReadOnly;

        public T this[int index] { get => ((IList<T>)listAxis)[index]; set => ((IList<T>)listAxis)[index] = value; }

        public int IndexOf(T item)
        {
            return ((IList<T>)listAxis).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)listAxis).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)listAxis).RemoveAt(index);
        }

        public void Add(T item)
        {
            ((IList<T>)listAxis).Add(item);
        }

        public void Clear()
        {
            ((IList<T>)listAxis).Clear();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)listAxis).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)listAxis).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((IList<T>)listAxis).Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)listAxis).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)listAxis).GetEnumerator();
        }

        #endregion
    }
}