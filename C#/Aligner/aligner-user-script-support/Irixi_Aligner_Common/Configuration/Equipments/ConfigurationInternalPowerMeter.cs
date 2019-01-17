using Irixi_Aligner_Common.Configuration.Base;

namespace Irixi_Aligner_Common.Configuration.Equipments
{
    public class ConfigurationInternalPowerMeter : ConfigurationBase
    {
        public string AttachedADCChannel { get; set; }

        public double BackgroundNoise { get; set; }

        public ConfigurationPDFrontEnd PDFrontEnd { get; set; }
    }
}
