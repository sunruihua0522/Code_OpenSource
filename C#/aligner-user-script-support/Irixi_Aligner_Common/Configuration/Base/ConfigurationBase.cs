using System;

namespace Irixi_Aligner_Common.Configuration.Base
{
    public class ConfigurationBase
    {
        public bool Enabled { get; set; }
        public Guid DeviceClass { get; set; }
        public string Desc { get; set; }
        public string Port { get; set; }
        public int BaudRate { get; set; }

        /// <summary>
        /// The caption is mostly used in the instrument list of the alignment editor.
        /// <para>the instrument will not be shown in the list if the caption left empty in the config file.</para>
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// The icon is used in the MainWindow for it the create control panel automatically.
        /// <para>the panel will not be created if the property is not defined in the config file.</para>
        /// </summary>
        public string Icon { get; set; }
        public string Comment { get; set; }
    }
}
