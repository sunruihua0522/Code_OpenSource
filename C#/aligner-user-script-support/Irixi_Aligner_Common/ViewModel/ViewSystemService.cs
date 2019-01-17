using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.ViewModel
{
    public class ViewSystemService : ViewModelBase
    {
       public ViewSystemService()
        {
            this.Service = new SystemService();
        }

        public SystemService Service { get; private set; }

        #region Commands

        public RelayCommand<IAxis> CommandHome
        {
            get
            {
                return new RelayCommand<IAxis>(axis =>
                {
                    Service.Home(axis);
                });
            }
        }

        public RelayCommand CommandHomeAllAxes
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DialogService.DialogService ds = new DialogService.DialogService();
                    ds.OpenHomeConfirmDialog(null, ret =>
                    {
                        if (ret == true)
                        {
                            Service.MassHome();
                        }
                    });
                });
            }
        }

        public RelayCommand<MassMoveArgs> CommandMassMove
        {
            get
            {
                return new RelayCommand<MassMoveArgs>(args =>
                {
                    Service.MassMoveLogicalAxis(args);
                });
            }
        }

        public RelayCommand CommandStart
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
                });
            }
        }

        public RelayCommand CommandStop
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
                });
            }
        }

        #endregion Commands
    }
}
