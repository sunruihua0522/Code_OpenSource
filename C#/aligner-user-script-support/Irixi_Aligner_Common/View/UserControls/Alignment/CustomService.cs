using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.PropertyGrid;

namespace Irixi_Aligner_Common.UserControls.AlignmentFunc
{
    public interface ICustomService
    {
        void BeginUpdate();
        void EndUpdate();
    }

    public class CustomService : ServiceBase, ICustomService
    {

        public PropertyGridControl GridControl
        {
            get { return (PropertyGridControl)GetValue(GridControlProperty); }
            set { SetValue(GridControlProperty, value); }
        }

        public static readonly DependencyProperty GridControlProperty =
            DependencyProperty.Register("GridControl", typeof(PropertyGridControl), typeof(CustomService), new PropertyMetadata(null));

        public void BeginUpdate()
        {
            Dispatcher.Invoke(new Action(() => {
                if (this.GridControl != null)
                {
                    this.GridControl.BeginDataUpdate();
                }
            }));
        }

        public void EndUpdate()
        {
            Dispatcher.Invoke(new Action(() => {
                if (this.GridControl != null)
                {
                    this.GridControl.EndDataUpdate();
                }
            }));
        }
    }
}
