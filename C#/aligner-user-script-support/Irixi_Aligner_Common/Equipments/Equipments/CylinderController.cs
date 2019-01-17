using Irixi_Aligner_Common.Configuration.Equipments;
using Irixi_Aligner_Common.Equipments.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Irixi;
using IrixiStepperControllerHelper;
using M12.Definitions;

namespace Irixi_Aligner_Common.Equipments.Equipments
{
    public class CylinderController : EquipmentBase
    {
        #region Variables

        #endregion
        
        #region Constructors
        public CylinderController(ConfigurationCylinder Config, IrixiM12 ControllerAttached) : base(Config)
        {
            this.Controller = ControllerAttached;
            this.PedalInputPort = Config.PedalInput;
            this.FiberClampOutputPort = Config.FiberClampOutput;
            this.LensVacuumOutputPort = Config.LensVacuumOutput;
            this.PLCVacuumOutputPort = Config.PLCVacuumOutput;
            this.PODVacuumOutputPort = Config.PODVacuumOutput;

            if (ControllerAttached != null)
            {
                ControllerAttached.OnDigitalInputUpdated += (s, e) =>
                {
                    if(e.Status.Integrated[PedalInputPort] == DigitalIOStatus.ON)
                    {
                        DoPedalTriggerd();
                    }
                };
            }
        }
        #endregion

        #region Properties
        public IrixiM12 Controller
        {
            get;
        }

        public int PedalInputPort
        {
            get;
        }

        public int FiberClampOutputPort
        {
            get;
        }

        public int LensVacuumOutputPort
        {
            get;
        }

        public int PLCVacuumOutputPort
        {
            get;
        }

        public int PODVacuumOutputPort
        {
            get;
        }

        DigitalIOStatus _fiber_clamp_state;
        /// <summary>
        /// Get the state of fiber clamp
        /// </summary>
        /// <value>The FiberClampState property gets/sets the value of the DigitalIOStatus field, _fiber_clamp_state.</value>
        public DigitalIOStatus FiberClampState
        {
            private set
            {
                UpdateProperty(ref _fiber_clamp_state, value);
            }
            get
            {
                return _fiber_clamp_state;
            }
        }

        DigitalIOStatus _lens_vacuum;
        /// <summary>
        /// Get the state of Lens Vacuum
        /// </summary>
        public DigitalIOStatus LensVacuumState
        {
            private set
            {
                UpdateProperty(ref _lens_vacuum, value);
            }
            get
            {
                return _lens_vacuum;
            }
        }


        DigitalIOStatus _plc_vacuum;
        /// <summary>
        /// Get the state of PLC Vacuum
        /// </summary>
        public DigitalIOStatus PlcVacuumState
        {
            private set
            {
                UpdateProperty(ref _plc_vacuum, value);
            }
            get
            {
                return _plc_vacuum;
            }
        }

        DigitalIOStatus _pod_vacuum;
        /// <summary>
        /// Get the state of POD Vacuum
        /// </summary>
        public DigitalIOStatus PodVacuumState
        {
            private set
            {
                UpdateProperty(ref _pod_vacuum, value);
            }
            get
            {
                return _pod_vacuum;
            }
        }

        #endregion

        #region Methods

        private void DoPedalTriggerd()
        {
            this.Controller.SetDOUT(FiberClampOutputPort, 
                this.FiberClampState == DigitalIOStatus.ON ? DigitalIOStatus.OFF : DigitalIOStatus.ON);
        }

        public void SetFiberClampState(DigitalIOStatus State)
        {
            this.Controller.SetDOUT(this.FiberClampOutputPort, State);
            FiberClampState = Controller.ReadDOUT(FiberClampOutputPort);
        }

        public void SetLensVacuumState(DigitalIOStatus State)
        {
            this.Controller.SetDOUT(this.LensVacuumOutputPort, State);
            LensVacuumState = Controller.ReadDOUT(LensVacuumOutputPort);
        }

        public void SetPodVacuumState(DigitalIOStatus State)
        {
            this.Controller.SetDOUT(this.PODVacuumOutputPort, State);
            PodVacuumState = Controller.ReadDOUT(PODVacuumOutputPort);
        }

        public void SetPlcVacuumState(DigitalIOStatus State)
        {
            this.Controller.SetDOUT(this.PLCVacuumOutputPort, State);
            PlcVacuumState = Controller.ReadDOUT(PLCVacuumOutputPort);
        }

        public override bool Init()
        {
            if (this.IsEnabled) // the controller is configured to be disabled in the config file 
            {
                if (this.Controller.IsInitialized)
                {
                    var status = Controller.ReadDOUT();

                    FiberClampState = status.Integrated[FiberClampOutputPort];
                    LensVacuumState = status.Integrated[LensVacuumOutputPort];
                    PlcVacuumState = status.Integrated[PLCVacuumOutputPort];
                    PodVacuumState = status.Integrated[PODVacuumOutputPort];

                    this.IsInitialized = true;
                    return true;
                }
                else
                {
                    this.LastError = "the attached motion controller is not initialized.";
                    return false;
                }
            }
            else
            {
                this.LastError = "it is disabled in the config file.";
                return false;
            }
        }
        #endregion
    }
}
