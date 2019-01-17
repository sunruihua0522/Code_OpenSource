using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace Irixi_Aligner_Common.DialogService.InputDialog
{
    public class InputDialogViewModel : DialogViewBase
    {

        public InputDialogViewModel(string Title, string Message) : base(Title, Message)
        {
            this.InputText = "";
        }

        public string InputText { get; set; }

        public RelayCommand<object> OkCommand
        {
            get
            {
                return new RelayCommand<object>(window =>
                {
                    if(InputText != null && InputText != "")
                        CloseDialogWithResult(window as Window, true);
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
