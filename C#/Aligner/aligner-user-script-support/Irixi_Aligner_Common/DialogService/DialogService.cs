using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Views;
using Irixi_Aligner_Common.DialogService.HomeConfirmDialog;
using Irixi_Aligner_Common.DialogService.InputDialog;

namespace Irixi_Aligner_Common.DialogService
{
    public class DialogService
    {

        private bool? ShowDialog(DialogViewBase vm, Window Owner)
        {
            DialogWindow win = new DialogWindow();
            if (Owner != null)
                win.Owner = Owner;
            win.DataContext = vm;
            win.ShowDialog();
            return (win.DataContext as DialogViewBase).DialogResult;
        }

        public void OpenInputDialog(string Title, string Message, Window Owner, Action<string> CallBack)
        {
            InputDialogViewModel vm = new InputDialogViewModel(Title, Message);
            var ret = ShowDialog(vm, Owner);
            if(ret.HasValue && ret == true)
            {
                CallBack(vm.InputText);
            }
        }

        public void OpenHomeConfirmDialog(Window Owner, Action<bool> CallBack)
        {
            HomeConfirmDialogViewModel vm = new HomeConfirmDialogViewModel("Confirm to Home", "");
            var ret = ShowDialog(vm, Owner);
            if(ret.HasValue)
            {
                CallBack(ret.Value);
            }
        }

        public void ShowYesNoMessage(string message, string title, Action afterYesCallback)
        {
            if (System.Windows.Forms.MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                if(afterYesCallback != null)
                    afterYesCallback.Invoke();
        }

        public void ShowError(string message, string title = "Error", Action afterHideCallback = null)
        {
            System.Windows.Forms.MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            if(afterHideCallback != null)
                afterHideCallback.Invoke();
        }


        public void ShowMessage(string message, string title)
        {
            System.Windows.Forms.MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            System.Windows.Forms.MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public void ShowMessageBox(string message, string title)
        {
            throw new NotImplementedException();
        }
    }
}
