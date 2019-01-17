using System.Windows;
using GalaSoft.MvvmLight;

namespace Irixi_Aligner_Common.DialogService
{
    public class DialogViewBase : ViewModelBase
    {
        public DialogViewBase(string Title, string Message)
        {
            this.DialogTitle = Title;
            this.Message = Message;
        }

        public bool? DialogResult { get; private set; }

        public string Message { get; private set; }

        public string DialogTitle { get; private set; }

        public void CloseDialogWithResult(Window Dialog, bool? Result)
        {
            DialogResult = Result;
            Dialog.DialogResult = Result;
        }
    }
}
