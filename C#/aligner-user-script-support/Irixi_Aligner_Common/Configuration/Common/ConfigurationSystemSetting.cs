using Irixi_Aligner_Common.Configuration.Equipments;
using Irixi_Aligner_Common.Configuration.MotionController;

namespace Irixi_Aligner_Common.Configuration.Common
{

    public class ConfigurationSystemSetting
    {
        /// <summary>
        /// Get wether record the log or not
        /// </summary>
        public bool LogEnabled { get; set; }

        public ConfigurationCylinder Cylinder { get; set; }

        public ConfigurationInternalPowerMeter[] InternalPowerMeter { get; set; }

        public ConfigurationContactSensor[] ContactSensor { get; set; }

        public ConfigurationKeithley2400[] Keithley2400s { get; set; }

        public ConfigurationNewport2832C[] Newport2832Cs { get; set; }

        /// <summary>
        /// Layout of physical motion controllers
        /// </summary>
        public ConfigPhysicalMotionController[] PhysicalMotionControllers { get; set; }

        /// <summary>
        /// Logical layout of the aligner
        /// </summary>
        public ConfigLogicalMotionComponent[] LogicalMotionComponents{ get; set; }

       
    }
}
