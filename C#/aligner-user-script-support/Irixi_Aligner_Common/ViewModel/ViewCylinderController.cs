
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Equipments.Equipments;

namespace Irixi_Aligner_Common.ViewModel
{
    public class ViewCylinderController : ViewModelBase
    {
        #region Variables

        SystemService service = null;

        #endregion

        #region Constructors

        public ViewCylinderController():base()
        {
            service = ServiceLocator.Current.GetInstance<ViewSystemService>().Service;
            this.Controller = service.CylinderController;
        }

        #endregion

        #region Properties

        public CylinderController Controller { get; }

        #endregion

        #region Commands

        public RelayCommand CommandToggleFiberClampState
        {
            get
            {
                return new RelayCommand(() =>
                {
                    service.ToggleFiberClampState();
                });
            }
        }

        public RelayCommand CommandToggleLensVacuumState
        {
            get
            {
                return new RelayCommand(() =>
                {
                    service.ToggleLensVacuumState();
                });
            }
        }

        public RelayCommand CommandTogglePlcVacuumState
        {
            get
            {
                return new RelayCommand(() =>
                {
                    service.TogglePlcVacuumState();
                });
            }
        }

        public RelayCommand CommandTogglePodVacuumState
        {
            get
            {
                return new RelayCommand(() =>
                {
                    service.TogglePodVacuumState();
                });
            }
        }

        #endregion
    }
}
