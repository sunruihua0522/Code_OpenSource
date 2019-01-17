using Irixi_Aligner_Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Irixi_Aligner_Common.Alignment
{
    public class AlignmentProfileManager
    {
        private const string PROFILE_FOLDER = "AlignmentProfiles";

        #region Constructors

        #endregion

        #region Properties
            

        #endregion

        #region Methods
        
        /// <summary>
        /// Load the existed profiles.
        /// </summary>
        public static string[] LoadProfileList(IAlignmentArgs Args)
        {
            var path = string.Join("\\", PROFILE_FOLDER, Args.PresetProfileFolder);

            if (Directory.Exists(path))
            {
                List<string> list = new List<string>();

                DirectoryInfo info = new DirectoryInfo(path);
                foreach (var file in info.GetFiles())
                {
                    if (file.Extension == ".json")
                    {
                        list.Add(Path.GetFileNameWithoutExtension(file.FullName));
                    }
                }

                return list.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check whether the profile is existed.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool CheckExistance(IAlignmentArgs Args, string Name)
        {
            var path = string.Join("\\", PROFILE_FOLDER, Args.PresetProfileFolder);
            var filename = string.Join("\\", path, $"{Name}.json");

            if (!Directory.Exists(path))
                return false;
            else if (!File.Exists(path))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Load the specifed profile.
        /// </summary>
        /// <param name="Name">The name of the profile</param>
        /// <returns></returns>
        public static IAlignmentArgsProfile Load<T>(IAlignmentArgs Args, string Name) where T : IAlignmentArgsProfile, new()
        {

            if (Name != "")
            {
                var path = string.Join("\\", PROFILE_FOLDER, Args.PresetProfileFolder, $"{Name}.json");

                if (File.Exists(path))
                {
                    var jsonstr = File.ReadAllText(path);

                    var profile = JsonConvert.DeserializeObject<T>(jsonstr);

                    // validate the content of profile by hash string
                    if (profile.Validate())
                        return profile;
                    else
                        throw new Exception("the checksum is error.");
                }
                else
                {
                    throw new FileNotFoundException($"the profile {Name} was not found.");
                }
            }
            else
            {
                throw new Exception("the name of the profile can not be empty.");
            }
        }

        /// <summary>
        /// Save the profile.
        /// </summary>
        /// <param name="Name"></param>
        public static void Save<T>(IAlignmentArgs Args, string Name) where T : IAlignmentArgsProfile, new()
        {
            if (Name == null || Name == "")
                throw new ArgumentException("the name of profile can not be empty.");

            // check the file name
            if (Name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new InvalidDataException("the name of profile contains invalid chars.");

            // validate the arguments, save is not allowed if some of the arguments are in error format.
            Args.Validate();


            // create the full path
            var path = string.Join("\\", PROFILE_FOLDER, Args.PresetProfileFolder);
            var filename = string.Join("\\", path, $"{Name}.json");

            // if the directory does not exist, create it.
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            var profile = new T();
            profile.FromArgsInstance(Args);

            var jsonstr = JsonConvert.SerializeObject(profile);
            File.WriteAllText(filename, jsonstr, new UTF8Encoding());
        }
       
        #endregion
    }
}
