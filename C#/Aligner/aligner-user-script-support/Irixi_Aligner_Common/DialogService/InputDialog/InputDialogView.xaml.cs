using System.IO;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;

namespace Irixi_Aligner_Common.DialogService.InputDialog
{
    /// <summary>
    /// Interaction logic for InputDialogView.xaml
    /// </summary>
    public partial class InputDialogView : UserControl
    {
        public InputDialogView()
        {
            InitializeComponent();
        }

        private void txtInput_Validate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null)
            {
                e.IsValid = (e.Value.ToString().IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
                e.ErrorContent = "The name contains invalid chars.";
            }
        }
    }
}
