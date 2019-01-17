using Irixi_Aligner_Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Irixi_Aligner_Common.UserScript
{
    public class UserScriptManager : ObservableRangeCollection<IUserScript>
    {
        #region Variables

        /// <summary>
        /// The folder to save the script file.
        /// </summary>
        const string FOLDER = "UserScript";

        /// <summary>
        /// The extension of the script file.
        /// </summary>
        const string EXT = "script";

        #endregion

        #region Constructors

        public UserScriptManager(SystemService Service)
        {
            this.Service = Service;
        }

        #endregion

        #region Properties

        private SystemService Service { get; }

        /// <summary>
        /// Get whether the list of the script has changed.
        /// If not, clean the list without warning the user.
        /// </summary>
        public bool HasModified { get; private set; }

        #endregion

        #region Override Methods of the base class

        protected override void InsertItem(int index, IUserScript item)
        {
            //item.Order = index;
            ((UserScriptBase)item).Service = this.Service;
            base.InsertItem(index, item);

            var ubase = item as UserScriptBase;
            ubase.PropertyChanged += (s, e) =>
            {
                this.HasModified = true;
            };

            RefreshOrder();

            HasModified = true;
        }

        public override void RemoveRange(IEnumerable<IUserScript> collection)
        {
            base.RemoveRange(collection);

            RefreshOrder();

            HasModified = true;
        }

        #endregion

        #region Private Methods

        void RefreshOrder()
        {
            for (int i = 0; i < this.Count; i++)
                this[i].Order = i;
        }

        string GetFullPath(string FileName)
        {
            return string.Join("\\", FOLDER, FileName) + "." + EXT;
        }

        bool CheckDirExist()
        {
            return Directory.Exists(FOLDER);
        }



        #endregion

        public bool CheckScriptFileExist(string FileName)
        {
            return File.Exists(GetFullPath(FileName));
        }
        
        /// <summary>
        /// Enum all script files.
        /// </summary>
        /// <returns></returns>
        public string[] GetScriptList()
        {
            if (CheckDirExist())
            {
                List<string> files = new List<string>();
                DirectoryInfo info = new DirectoryInfo(FOLDER);
                foreach (var file in info.GetFiles())
                {
                    if (file.Extension == $".{EXT}")
                    {
                        files.Add(Path.GetFileNameWithoutExtension(file.FullName));
                    }
                }

                return files.ToArray();
            }
            else;
            {
                return new string[] { };
            }
        }

        public void Save(string FileName)
        {
            List<IUserScript> lst = new List<IUserScript>();
            foreach (var item in this)
            {
                lst.Add(item);
            }

            BinaryFormatter formatter = new BinaryFormatter();

            if (CheckDirExist() == false)
                Directory.CreateDirectory(FOLDER);

            using (Stream fs = File.Open(GetFullPath(FileName), FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, lst);
            }

            HasModified = false;
        }

        public void Load(string FileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (Stream fs = File.Open(GetFullPath(FileName), FileMode.Open))
            {
                var obj = formatter.Deserialize(fs);
                var lst = (List<IUserScript>)obj;

                this.Clear();

                lst.ToList().ForEach(cmd =>
                {
                    cmd.Service = this.Service;
                    cmd.RecoverReferenceTypeProperties();
                    this.Add(cmd);

                });
            }

            HasModified = false;
        }
        
        /// <summary>
        /// Reset the status of all commands.
        /// </summary>
        public void ResetStatus()
        {
            this.ToList().ForEach(i => i.Reset());
        }

        public void RunRange(int StartIndex, int EndIndex)
        {

            // validate each command
            bool isError = false;

            for(int i = StartIndex; i <= EndIndex; i++)
            { 
                this[i].Validate();
                isError |= this[i].IsError;
            }

            if (isError)
                throw new Exception("Some of the commands in the user script list are error.");

            // run the command
            for (int i = StartIndex; i <= EndIndex; i++)
            {
                try
                {
                    this[i].Perform();
                }
                catch(Exception ex)
                {
                    throw new Exception($"Error occured while executing the command {i}, {ex.Message}");
                }
            }
        }

        public void Run(int StartIndex)
        {
            RunRange(StartIndex, this.Count - 1);
           
        }
    }
}
