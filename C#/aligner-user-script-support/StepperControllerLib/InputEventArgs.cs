using System;

namespace IrixiStepperControllerHelper
{
    public class InputIOEventArgs : EventArgs
    {
        public InputIOEventArgs(int Channel, InputState State)
        {
            this.Channel = Channel;
            this.State = State;
        }

        /// <summary>
        /// Get which input channel has changed
        /// for 3-axis controller, this should be 0 - 7; for 1-axis controller, this should be 0 - 1
        /// </summary>
        public int Channel
        {
            private set;
            get;
        }

        /// <summary>
        /// Get the state of input
        /// for 3-axis controller, this should be 0 - 7; for 1-axis controller, this should be 0 - 1
        /// </summary>
        public InputState State
        {
            private set;
            get;
        }
    }
}
