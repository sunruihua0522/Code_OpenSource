
using Irixi_Aligner_Common.Configuration.Layout;
using Irixi_Aligner_Common.Message;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Irixi_Aligner_Common.Configuration.Common
{
    public class ConfigManager
    {
        const string FILE_SYSTEMSETTING = @"Configuration\system_setting.json";
        const string FILE_PROFILE = @"Configuration\profile_motorized_stages.json";
        const string FILE_LAYOUT = @"Configuration\layout.json";
        const string FILE_DEFAULTLAYOUT = @"Configuration\defaultlayout.json";


        public ConfigManager()
        {
            #region Read the configration of motion controller

            try
            {
                var json_string = File.ReadAllText(FILE_SYSTEMSETTING, Encoding.UTF8);

                // Convert to object 
                this.ConfSystemSetting = 
                    JsonConvert.DeserializeObject<ConfigurationSystemSetting>(json_string);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine("Unable to load config file {0}, {1}", FILE_SYSTEMSETTING, ex.Message, LogHelper.LogType.ERROR);

                throw new Exception(ex.Message);
            }

            #endregion

            #region Read the profile of Suruage stages
            try
            {

                var json_string = File.ReadAllText(FILE_PROFILE);

                // Convert to object 
                this.ProfileManager = JsonConvert.DeserializeObject<MotorizedStagesProfileManager>(json_string);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine("Unable to load config file {0}, {1}", FILE_PROFILE, ex.Message, LogHelper.LogType.ERROR);

                throw new Exception(ex.Message);
            }
            #endregion

            #region read layout
            string layout_file = "";
            try
            {
                if (StaticVariables.IsLoadDefaultLayout)
                {
                    // load default layout
                    layout_file = FILE_DEFAULTLAYOUT;
                }
                else
                {
                    // load last layout
                    layout_file = FILE_LAYOUT;
                }
                

                var json_string = File.ReadAllText(layout_file);

                // Convert to object
                this.ConfWSLayout = JsonConvert.DeserializeObject<LayoutManager>(json_string);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine("Unable to load config file {0}, {1}", layout_file, ex.Message, LogHelper.LogType.ERROR);

                //throw new Exception(ex.Message);
            }

            #endregion

            #region Bind actuator profile to physical axis
            bool allfound = true;
            foreach(var cfgMC in ConfSystemSetting.PhysicalMotionControllers)
            {
                foreach(var cfgAxis in cfgMC.AxisCollection)
                {
                    var profile = this.ProfileManager.FindProfile(cfgAxis.Vendor, cfgAxis.Model);

                    if (profile == null)
                    {
                        LogHelper.WriteLine("Unable to find the motorized stage profile of vendor:{0}/model:{1}", cfgAxis.Vendor, cfgAxis.Model, LogHelper.LogType.ERROR);

                        allfound = false;
                    }
                    else
                    {
                        cfgAxis.SetProfile(profile.Clone() as MotorizedStageProfile);
                    }
                }
            }

            if(allfound == false)
            {
                throw new Exception("Some of motorized stage profiles are not found.");
            }


            #endregion
        }

        public ConfigurationSystemSetting ConfSystemSetting { get; }

        public MotorizedStagesProfileManager ProfileManager { get; }

        public LayoutManager ConfWSLayout { get; set; }

        /// <summary>
        /// Save the layout of document group
        /// </summary>
        /// <param name="layout"></param>
        public void SaveLayout(dynamic layout)
        {
            string json_str = JsonConvert.SerializeObject(layout);

            using (FileStream fs = File.Open(FILE_LAYOUT, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter wr = new StreamWriter(fs))
                {
                    wr.Write(json_str);
                    wr.Close();
                }

                fs.Close();
            }
        }
    }
}
