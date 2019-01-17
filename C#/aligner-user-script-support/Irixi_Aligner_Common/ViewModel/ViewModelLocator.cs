/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Irixi_Aligner_Common"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Irixi_Aligner_Common.Configuration.Common;
using Irixi_Aligner_Common.MotionControllers.Base;
using Irixi_Aligner_Common.ViewModel.Alignment;
using Irixi_Aligner_Common.ViewModel.ListBasedUserScript;

namespace Irixi_Aligner_Common.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
                
                SimpleIoc.Default.Register<ViewSystemService>();

                SimpleIoc.Default.Register<ViewFastCentralAlign>();
                SimpleIoc.Default.Register<ViewRotatingProfile>();
                SimpleIoc.Default.Register<ViewCentralAlign>();
                SimpleIoc.Default.Register<ViewProfileND>();
                SimpleIoc.Default.Register<ViewFastND>();
                SimpleIoc.Default.Register<ViewSnakeRouteScan>();
                SimpleIoc.Default.Register<ViewSpiralScan>();
                SimpleIoc.Default.Register<ViewFastRotatingScan>();

                SimpleIoc.Default.Register<ViewListBasedUserScriptEdit>();
                SimpleIoc.Default.Register<ViewContactSensor>();
                SimpleIoc.Default.Register<ViewInternalPowerMeter>();
                SimpleIoc.Default.Register<ViewCylinderController>();
            }
        }

        public ViewSystemService ServiceView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewSystemService>();
            }
        }

        public ViewContactSensor CSSView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewContactSensor>();
            }
        }

        public ViewInternalPowerMeter InternalPowerMeterView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewInternalPowerMeter>();
            }
        }

        public ViewCylinderController CylinderControlView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewCylinderController>();
            }
        }

        public ViewListBasedUserScriptEdit UserScriptEditView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewListBasedUserScriptEdit>();
            }

        }

        #region Views of alignment

        public ViewSpiralScan BlindSearchView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewSpiralScan>();
            }
        }

        public ViewSnakeRouteScan SnakeRouteScanView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewSnakeRouteScan>();
            }
        }

        public ViewFastND FastNDView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewFastND>();
            }
        }

        public ViewFastCentralAlign FastCentralAlignView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewFastCentralAlign>();
            }
        }

        public ViewFastRotatingScan FastRotatingScanView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewFastRotatingScan>();
            }
        }

        public ViewRotatingProfile RotatingProfileView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewRotatingProfile>();
            }
        }

        public ViewProfileND ProfileNDView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewProfileND>();
            }
        }
        
        public ViewCentralAlign CentralAlignView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewCentralAlign>();
            }
        }

        #endregion
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}