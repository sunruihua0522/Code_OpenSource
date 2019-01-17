using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Irixi_Aligner_Common.Alignment;
using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Irixi_Aligner_Common.ViewModel.Base
{
    /// <summary>
    /// Make sure that the class is generated after the generation of the system service class.
    /// </summary>
    public class ViewBaseAlignment<T> : ViewModelBase where T: IAlignmentArgsProfile, new()
    {
        #region Variables

        protected string _selectedProfile;
        ObservableCollection<string> _profiles = new ObservableCollection<string>();

        #endregion

        #region Propeties

        public SystemService Service
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewSystemService>().Service;
            }
        }

        public virtual IAlignmentHandler Handler => throw new NotImplementedException();

        public virtual IAlignmentArgs Arguments => throw new NotImplementedException();

        public virtual ObservableCollection<string> Profiles
        {
            get
            {
                RefreshProfiles();
                return _profiles;
            }
        }

        public virtual string SelectedProfile
        {
            set
            {
                if (value != null && value != "")
                {
                    _selectedProfile = value;

                    try
                    {
                        var profile = AlignmentProfileManager.Load<T>(Arguments, SelectedProfile);
                        profile.ToArgsInstance(Service, Arguments);
                    }
                    catch (Exception ex)
                    {
                        Service.ShowErrorMessageBox($"Unable to load the profile {_selectedProfile}, {ex.Message}");
                    }
                }
            }
            get
            {
                return _selectedProfile;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refresh the list of the preset profiles.
        /// </summary>
        private void RefreshProfiles()
        {
            var oldProfile = this.SelectedProfile;

            _profiles.Clear();

            var profiles = AlignmentProfileManager.LoadProfileList(Arguments);
            if (profiles != null)
            {
                foreach (var p in profiles)
                    _profiles.Add(p);
            }

            SelectedProfile = oldProfile;
        }

        #endregion

        #region Commands

        public RelayCommand StartCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Service.StartToAutoAlign(Handler);
                });
            }
        }

        public RelayCommand StopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Service.Stop();
                });
            }
        }

        public RelayCommand<string> SaveCommand
        {
            get
            {
                return new RelayCommand<string>(filename =>
                {
                    AlignmentProfileManager.Save<T>(Arguments, SelectedProfile);

                });
            }
        }

        public RelayCommand SaveAsCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
                        DialogService.DialogService ds = new DialogService.DialogService();

                    try
                    {
                        ds.OpenInputDialog("Preset Profile Save", "Please input the preset profile name.", null, name =>
                        {
                            if (AlignmentProfileManager.CheckExistance(Arguments, name))
                            {
                                if (MessageBox.Show($"The profile {name} has existed, are you sure to overwrite?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                                {
                                    return;
                                }
                            }

                            AlignmentProfileManager.Save<T>(Arguments, name);

                            RefreshProfiles();
                        });
                    }
                    catch(Exception ex)
                    {
                        ds.ShowError(ex.Message);
                    }
                });
            }
        }

        #endregion
    }
}
