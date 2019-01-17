using Irixi_Aligner_Common.Configuration.Base;

namespace Irixi_Aligner_Common.Configuration.Equipments
{
    public class ConfigurationPDFrontEnd : ConfigurationBase
    {
        /// <summary>
        /// The values of the sampling resistors to convert voltage to current.
        /// </summary>
        public double[] SamplingRes { get; set; }
    }
}
