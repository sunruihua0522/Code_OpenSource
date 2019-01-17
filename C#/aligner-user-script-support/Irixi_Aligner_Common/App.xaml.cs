using System;
using System.Windows;

namespace Irixi_Aligner_Common
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The argument string of starting application with default layout
        /// </summary>
        const string kARG_DEFAULTLAYOUT = "--defaultlayout";

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // check startup arguments
                if (e.Args.Length > 0)
                {
                    string arg = e.Args[0];

                    if (arg.ToLower() == kARG_DEFAULTLAYOUT)
                        StaticVariables.IsLoadDefaultLayout = true;
                }

                base.OnStartup(e);

                //DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}