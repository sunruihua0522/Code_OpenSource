using CommonServiceLocator;
using GalaSoft.MvvmLight;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Equipments.Equipments.Instruments;

namespace Irixi_Aligner_Common.ViewModel
{
    public class ViewContactSensor : ViewModelBase
    {

        #region Variables

        SystemService service = null;

        #endregion

        #region Constructors

        public ViewContactSensor():base()
        {
            service = ServiceLocator.Current.GetInstance<ViewSystemService>().Service;
            
            try
            {
                this.CSS1 = service.CSS[0];
            }
            catch
            {
                service.SetMessage(Message.MessageType.Error,
                    $"Unable to find the instance of the CSS1.");
            }

            try
            {
                this.CSS2 = service.CSS[1];
            }
            catch
            {
                service.SetMessage(Message.MessageType.Error,
                    $"Unable to find the instance of the CSS2.");
            }
        }

        #endregion

        /// <summary>
        /// Get the instance of the CSS.
        /// </summary>
        public ContactSensor CSS1
        {
            get;
        }

        public ContactSensor CSS2 { get; }
        
    }
}
