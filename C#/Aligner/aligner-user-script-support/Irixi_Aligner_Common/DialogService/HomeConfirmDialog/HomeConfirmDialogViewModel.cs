using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace Irixi_Aligner_Common.DialogService.HomeConfirmDialog
{
    public class HomeConfirmDialogViewModel : DialogViewBase
    {
        public HomeConfirmDialogViewModel(string Title, string Message) : base(Title, Message)
        {
            Random rd = new Random((int)DateTime.Now.Ticks);
            RandomCode = (rd.NextDouble() * 10000).ToString("F0");
            
        }

        public string RandomCode { get; private set; }

        public string InputCode { get; set; }

        public RelayCommand<object> OKCommand
        {
            get
            {
                return new RelayCommand<object>(window =>
                {
                    if(InputCode != null && InputCode != "")
                    {
                        if(InputCode == RandomCode)
                            CloseDialogWithResult(window as Window, true);
                    }
                });
            }
        }

        public RelayCommand<object> CancelCommand
        {
            get
            {
                return new RelayCommand<object>(window =>
                {
                    CloseDialogWithResult(window as Window, false);
                });
            }
        }

    }
}
