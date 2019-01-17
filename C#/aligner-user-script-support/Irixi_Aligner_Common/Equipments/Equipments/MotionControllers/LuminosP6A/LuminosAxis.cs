using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.Message;
using Irixi_Aligner_Common.MotionControllers.Base;
using Zaber;

namespace Irixi_Aligner_Common.MotionControllers.Luminos
{
    public class LuminosAxis : AxisBase
    {
        #region Constructors

        public LuminosAxis(ConfigPhysicalAxis Config, IMotionController Controller) :base(Config, Controller)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the instance of zaber conversation object
        /// </summary>
        public Conversation ZaberConversation { private set; get; }

        #endregion

        #region Methods

        /// <summary>
        /// Set the zaber conversation
        /// </summary>
        /// <param name="cs"></param>
        public void RegisterZaberConversation(Conversation cs)
        {
            this.ZaberConversation = cs;
            GetCurrentState();
        }

        /// <summary>
        /// read the current state of the stage
        /// include IsHomed, MaxSteps, AbsPosition
        /// </summary>
        private void GetCurrentState()
        {
            // read home state
            DeviceModes mode =  (DeviceModes)ZaberConversation.Request(Command.ReturnSetting, (int)Command.SetDeviceMode).Data;
            this.IsHomed = ((mode & DeviceModes.HomeStatus) == DeviceModes.HomeStatus);

            // read current position
            if(int.TryParse(ZaberConversation.Request(Command.ReturnSetting, (int)Command.SetCurrentPosition).Data.ToString(), out int pos))
            {
                this.AbsPosition = pos;
            }
            else
            {
                this.AbsPosition = -1;
            }

            // read max position
            if (int.TryParse(ZaberConversation.Request(Command.ReturnSetting, (int)Command.SetMaximumPosition).Data.ToString(), out int max_steps))
            {
                this.SCWL = max_steps;
                this.UnitHelper.ChangeMaxSteps(max_steps);

                // re-init the unit helper due to the max-steps is read from the hardware controller
                // var prev = this.UnitHelper.Clone() as RealworldDistanceUnitHelper;
                // this.UnitHelper = new RealworldDistanceUnitHelper(max_steps, prev.MaxStroke, prev.Unit, prev.Digits);

                LogHelper.WriteLine("{0} Max Steps was read from flash, the new value is {1}", this, max_steps, LogHelper.LogType.NORMAL);
            }
            else
            {
                // keep the value set in the config file
            }


            // read maximum speed
            if (int.TryParse(ZaberConversation.Request(Command.ReturnSetting, (int)Command.SetTargetSpeed).Data.ToString(), out int target_speed))
            {
                this.MaxSpeed = target_speed;
                LogHelper.WriteLine("{0} Max Speed was read from flash, the new value is {1}", this, target_speed, LogHelper.LogType.NORMAL);
            }
            else
            {
                // keep the value set in the config file
            }

        }

        #endregion
    }
}
