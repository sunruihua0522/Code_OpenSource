using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Irixi_Aligner_Common
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// <see cref="https://dontpaniclabs.com/blog/post/2013/11/14/dynamic-splash-screens-in-wpf/"/>
    /// </summary>
    public partial class Splash : Window
    {

        public Splash()
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            txtVersionInfo.Text = string.Format("v{0}", version);
        }

        public void ShowMessage(string Message)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtMessage.Text = Message;
            });
        }


        public void LoadComplete()
        {
            Dispatcher.InvokeShutdown();
        }
    }
}
