using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Base;
using M12.Base;
using M12.Definitions;
using System;

namespace Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12
{
    public class IrixiM12Axis : AxisBase
    {
        #region Variables

        private int _maxSpeed;

        #endregion

        #region Constructors

        public IrixiM12Axis(ConfigPhysicalAxis Config, IMotionController Controller) : base(Config, Controller)
        {

        }

        #endregion

        #region Properties

        public UnitInformation Info { get; set; }

        public UnitSettings Settings { get; set; }

        public UnitState State { get; set; }

        public override string AxisName
        {
            get => base.AxisName;
            set
            {
                base.AxisName = value;
                this.ID = (UnitID)(Enum.Parse(typeof(UnitID), value));
            }
        }

        /// <summary>
        /// Get or set the maximum speed in percent.
        /// <para>Note that the value of this property will limited to the range 1 - 100</para>
        /// </summary>
        public override int MaxSpeed
        {
            get => _maxSpeed;
            protected set
            {
                if (value > 100)
                    _maxSpeed = 100;
                else if (value == 0)
                    _maxSpeed = 1;
                else
                    _maxSpeed = value;
            }
        }

        public UnitID ID { get; set; }

        #endregion
    }
}
