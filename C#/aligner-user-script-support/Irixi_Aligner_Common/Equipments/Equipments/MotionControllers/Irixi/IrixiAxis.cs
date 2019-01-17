using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Base;
using System;

namespace Irixi_Aligner_Common.MotionControllers.Irixi
{
    public class IrixiAxis : AxisBase
    {

        #region Constructors
        public IrixiAxis(ConfigPhysicalAxis Config, IMotionController Controller) : base(Config, Controller)
        {

        }
        #endregion

        #region Properties

        public override string AxisName
        {
            get => base.AxisName;
            set
            {
                base.AxisName = value;
                this.OnBoardCH = Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// The channel of board this axis belongs to.
        /// </summary>
        public int OnBoardCH { get; set; }


        /// <summary>
        /// Get whether the drive direction is reversed (CCWL is set to the zero point)
        /// </summary>
        public bool ReverseDriveDirecton { private set; get; }

        #endregion

        #region Private Methods

        internal override void Setup(ConfigPhysicalAxis Config, IMotionController Controller)
        {
            base.Setup(Config, Controller);

            if (Config.ReverseDriveDirection.HasValue)
            {
                this.ReverseDriveDirecton = Config.ReverseDriveDirection.Value;
            }
            else
            {
                this.ReverseDriveDirecton = false;
            }
        }

        #endregion
    }
}
