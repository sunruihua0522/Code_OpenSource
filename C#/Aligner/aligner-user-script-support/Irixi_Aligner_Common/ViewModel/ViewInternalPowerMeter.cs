using CommonServiceLocator;
using GalaSoft.MvvmLight;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Equipments.Equipments.Instruments;
using System.Linq;

namespace Irixi_Aligner_Common.ViewModel
{
    public class ViewInternalPowerMeter : ViewModelBase
    {
        #region Variables

        SystemService service = null;
        private InternalPowerMeter _ipm1;
        private InternalPowerMeter _ipm2;

        #endregion

        public ViewInternalPowerMeter():base()
        {
            service = ServiceLocator.Current.GetInstance<ViewSystemService>().Service;

            var ipmList = service.InternalPowerMeter.Cast<InternalPowerMeter>().ToList();
            
            try
            {
                this.IPM1 = ipmList[0];
            }
            catch
            {
                service.SetMessage(Message.MessageType.Error,
                    $"Unable to find the instance of the Internal PowerMeter1.");
            }

            try
            {
                this.IPM2 = ipmList[1];
            }
            catch
            {
                service.SetMessage(Message.MessageType.Error,
                    $"Unable to find the instance of the Internal PowerMeter2.");
            }
        }

        public InternalPowerMeter IPM1
        {
            get
            {
                return _ipm1;
            }
            private set
            {
                _ipm1 = value;
                RaisePropertyChanged();
            }
        }

        public InternalPowerMeter IPM2
        {
            get
            {
                return _ipm2;
            }
            private set
            {
                _ipm2 = value;
                RaisePropertyChanged();
            }
        }
    }
}
