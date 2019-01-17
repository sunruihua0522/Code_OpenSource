namespace Irixi_Aligner_Common.Configuration.MotionController
{
    public class ConfigLogicalMotionComponent
    {
        string _displayname = "undefined";
        /// <summary>
        /// Get the name of the aligner shown on the window which is defined in the json file
        /// </summary>
        public string Caption
        {
            set
            {
                _displayname = value;
            }
            get
            {
                return _displayname;
            }
        }

        /// <summary>
        /// Get which icon should be displayed on the caption and the show/hide button
        /// </summary>
        public string Icon
        {
            get;
            set;
        }

        /// <summary>
        /// Get whether the logical motion component is used to align the production
        /// </summary>
        public bool IsAligner
        {
            get;
            set;
        }

        /// <summary>
        /// Get the array of logical axis defined in the json file
        /// </summary>
        public ConfigLogicalAxis[] LogicalAxisArray { set; get; }

    }
}
