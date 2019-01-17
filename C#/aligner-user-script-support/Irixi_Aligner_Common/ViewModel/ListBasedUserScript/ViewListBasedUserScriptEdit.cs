using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Irixi_Aligner_Common.UserScript;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Irixi_Aligner_Common.ViewModel.ListBasedUserScript
{
    public class ViewListBasedUserScriptEdit : ViewModelBase
    {
        #region Variables

        string _selectedScriptName;

        #endregion

        public ViewListBasedUserScriptEdit()
        {
            ServiceView = ServiceLocator.Current.GetInstance<ViewSystemService>();
            this.Manager = ServiceView.Service.ListBasedScriptManager;
            this.SelectedCommandsInGrid = new ObservableCollection<IUserScript>();

            ScriptFileList = new ObservableCollection<string>();

            ReloadScriptFileList();
            
        }

        #region Properties

        private ViewSystemService ServiceView { get; }

        public List<object> CommandList
        {
            get
            {
                var list = new List<object>();

                var nspace = "Irixi_Aligner_Common.UserScript.Implementation";

                var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsClass && t.Namespace == nspace
                        select t;

                q.ToList().ForEach(t =>
                {
                    try
                    {
                        var o = (IUserScript)Activator.CreateInstance(t);

                        list.Add(new { o.Name, ObjectType = t, Usage = o.Usage });
                    }
                    catch (Exception ex)
                    {

                    }
                });

                return list;
            }
        }

        public dynamic SelectedCommand { get; set; }

        public UserScriptManager Manager { get; }

        public ObservableCollection<IUserScript> SelectedCommandsInGrid { get; }

        public ObservableCollection<string> ScriptFileList { get; }

        public string SelectedScriptFile
        {
            get
            {
                return _selectedScriptName;
            }
            set
            {
                try
                {
                    if (value != _selectedScriptName)
                    {
                        if(Manager.Count > 0 && Manager.HasModified)
                        {
                            var ret = MessageBox.Show("The script has been modified, are you sure to clean it without saving?", "Warning", 
                                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                            if (ret == MessageBoxResult.No)
                            {
                                return;
                            }
                        }

                        Manager.Load(value);
                        _selectedScriptName = value;
                    }
                    
                }
                catch(Exception ex)
                {
                    ServiceView.Service.ShowErrorMessageBox($"Unable to load script {value}, {ex.Message}");
                    _selectedScriptName = null;
                }
                finally
                {
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void ReloadScriptFileList()
        {
            ScriptFileList.Clear();

            Manager.GetScriptList().ToList().ForEach(i => ScriptFileList.Add(i));
        }

        #endregion

        #region Command

        public RelayCommand AddNewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedCommand == null)
                        MessageBox.Show("No user command selected.");
                    else
                    {
                        var cmd = (IUserScript)Activator.CreateInstance(SelectedCommand.ObjectType);
                        Manager.Add(cmd);
                    }
                });
            }
        }

        public RelayCommand<int> InsertNewCommand
        {
            get
            {
                return new RelayCommand<int>(index =>
                {
                    if (SelectedCommand == null)
                        MessageBox.Show("No user command selected.");
                    else
                    {
                        var cmd = (IUserScript)Activator.CreateInstance(SelectedCommand.ObjectType);
                        Manager.Insert(index, cmd);
                    }
                });
            }
        }

        public RelayCommand RemoveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Manager.RemoveRange(SelectedCommandsInGrid);
                });
            }
        }

        public RelayCommand OpenNewEditorCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if(Manager.Count > 0 && Manager.HasModified)
                    {
                        var ret = MessageBox.Show("The script has been modified, are you sure to clean it without saving?", "Warning", 
                            MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                        if (ret == MessageBoxResult.No)
                            return;
                            
                    }

                    Manager.Clear();

                });
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (this.SelectedScriptFile == null || this.SelectedScriptFile == "")
                        MessageBox.Show("No script selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        Manager.Save(SelectedScriptFile);
                    }
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
                    ds.OpenInputDialog("Save Script", "Input a name for the script list.", null, name =>
                    {
                        if(Manager.CheckScriptFileExist(name) == true)
                        {
                            if (MessageBox.Show($"The script {name} has existed, are you sure to overwrite?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                            {
                                return;
                            }
                        }

                        Manager.Save(name);

                        ReloadScriptFileList();

                        this.SelectedScriptFile = name;
                        

                    });
                    
                });
            }
        }

        public RelayCommand<IUserScript> StartCommand
        {
            get
            {
                return new RelayCommand<IUserScript>(cmdSelected =>
                {
                    try
                    {
                        var msg = "";

                        if (cmdSelected == null)
                            msg = "Are you sure to run the script?";
                        else
                            msg = $"Are you sure to run the script from {cmdSelected.Order}?";

                        var ret = MessageBox.Show(msg, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (ret == MessageBoxResult.No)
                            return;

                        Manager.ResetStatus();

                        if (cmdSelected == null)
                            Manager.Run(0);
                        else
                            Manager.Run(cmdSelected.Order);
                    }
                    catch(Exception ex)
                    {
                        ServiceView.Service.SetMessage(Message.MessageType.Error, ex.Message);
                        ServiceView.Service.ShowErrorMessageBox(ex.Message);
                    }
                });
            }
        }

        public RelayCommand<IUserScript> StartOneLineCommand
        {
            get
            {
                return new RelayCommand<IUserScript>(cmdSelected =>
                {
                    try
                    {
                        Manager.ResetStatus();

                        if (cmdSelected == null)
                        {
                            new DialogService.DialogService().ShowError("No command selected.");
                        }
                        else
                            Manager.RunRange(cmdSelected.Order, cmdSelected.Order);
                    }
                    catch (Exception ex)
                    {
                        ServiceView.Service.SetMessage(Message.MessageType.Error, ex.Message);
                        ServiceView.Service.ShowErrorMessageBox(ex.Message);
                    }
                });
            }
        }


        #endregion
    }
}
