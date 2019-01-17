using Irixi_Aligner_Common.Configuration.Base;

namespace Irixi_Aligner_Common.Configuration.Equipments
{
    public class ConfigurationCylinder : ConfigurationBase
    {
        public int PedalInput { get; set; }
        public int FiberClampOutput { get; set; }
        public int LensVacuumOutput { get; set; }
        public int PLCVacuumOutput { get; set; }
        public int PODVacuumOutput { get; set; }
    }
}
