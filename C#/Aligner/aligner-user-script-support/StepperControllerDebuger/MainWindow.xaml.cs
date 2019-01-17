using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Data;

namespace StepperControllerDebuger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
            Messenger.Default.Register<NotificationMessage<string>>(this, new Action<NotificationMessage<string>>(msg =>
            {
                switch(msg.Notification)
                {
                    case "Error":
                        MessageBox.Show(msg.Content, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;

                    case "MSG":
                        MessageBox.Show(msg.Content, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
            }));
        }
    }
}
