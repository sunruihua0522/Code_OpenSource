using System;

namespace Irixi_Aligner_Common.Configuration.MotionController
{
    public class ConfigLogicalAxis
    {
        /// <summary>
        /// Get or set which motion contoller is used
        /// </summary>
        public Guid DeviceClass { get; set; }

        /// <summary>
        /// Get or set the physical axis name
        /// </summary>
        public string AxisName { get; set; }

        /// <summary>
        /// Get the name displayed on the window
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Get the order of home action
        /// </summary>
        public int HomeOrder { get; set; }

        /// <summary>
        /// Get or set the caption of the CCW move button (Negative direction).
        /// </summary>
        public string CCWButtonCaption { get; set; }

        /// <summary>
        /// Get or set the caption of the CW move button (Positive direction).
        /// </summary>
        public string CWButtonCaption { get; set; }

        /// <summary>
        /// Switch the move direction of the CW/CCW buttons.
        /// </summary>
        public bool SwitchButtonDirection { get; set; }

        /// <summary>
        /// Get or set whether the cruise is allowed or not.
        /// </summary>
        public bool AllowCruise { get; set; }
    }
}
