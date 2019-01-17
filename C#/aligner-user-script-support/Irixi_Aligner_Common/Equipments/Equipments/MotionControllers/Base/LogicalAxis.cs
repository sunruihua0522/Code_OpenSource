using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Interfaces;
using System;

namespace Irixi_Aligner_Common.MotionControllers.Base
{
    public class LogicalAxis : ViewModelBase, IHashable
    {
        #region Variables
        /*
         * Why dont we call the home/move.. functions of the physical axis directly?
         * 
         * The system status must be maintained such as the `system busy` flag, and all
         * axes must be coordinated such as how many axes can be moved together.
         * 
         * So when and how to operate the axis must be handled by the system service, the
         * logical axis raises these events and the system service implements the details.
         * 
         */
        public event EventHandler OnHomeRequested;
        public event EventHandler<AxisMoveArgs> OnMoveRequested;
        public event EventHandler<AxisCruiseArgs> OnCuriseRequested;
        public event EventHandler OnStopRequsted;

        IAxis physicalAxis;

        #endregion

        #region Constructors

        public LogicalAxis(ConfigLogicalAxis Config, string ParentComponentName)
        {
            this.Config = Config;
            this.AxisName = Config.DisplayName;
            this.ParentName = ParentComponentName;
            this.MoveArgs = new AxisMoveArgs()
            {
                AxisCaption = AxisName
            };

            this.MoveArgsTemp = new AxisMoveArgs();
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Get the configuration of logical axis
        /// </summary>
        public ConfigLogicalAxis Config { private set; get; }
        
        /// <summary>
        /// Get the name display on the window
        /// <see cref="Irixi_Aligner_Common.Configuration.MotionController.ConfigLogicalAxis.DisplayName"/>
        /// </summary>
        public string AxisName { private set; get; }
        
        /// <summary>
        /// Get the name of parent aliger
        /// </summary>
        public string ParentName { private set; get; }
        
        /// <summary>
        /// Get the instance of physical axis
        /// </summary>
        public IAxis PhysicalAxisInst
        { 
            get
            {
                return physicalAxis;
            }
            set
            {
                physicalAxis = value;
                MoveArgs.LogicalAxisHashString = HashString;
                MoveArgs.Unit = value.UnitHelper.ToString();
            }
        }
        
        /// <summary>
        /// Get or set the arguments to move the physical axis, which is also bound to the window
        /// </summary>
        public AxisMoveArgs MoveArgs { get; set; }
        
        /// <summary>
        /// The property is specically used for the mass move function
        /// <see cref="Irixi_Aligner_Common.Classes.SystemService.MassMoveLogicalAxis(MassMoveArgs)"/>
        /// </summary>
        public AxisMoveArgs MoveArgsTemp { get; set; }

        public string HashString
        {
            get
            {
                return PhysicalAxisInst.HashString;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods

        public void ToggleMoveMode()
        {
            PhysicalAxisInst.ToggleMoveMode();
            if (PhysicalAxisInst.IsAbsMode)
                MoveArgs.Mode = MoveMode.ABS;
            else
                MoveArgs.Mode = MoveMode.REL;
        }

        public override string ToString()
        {
            return string.Format("{0}@{1}", Config.DisplayName, ParentName);
        }

        #endregion

        #region Commands

        public RelayCommand Home
        {
            get
            {
                return new RelayCommand(() =>
                {
                    OnHomeRequested?.Invoke(this, new EventArgs());

                });
            }
        }
        
        public RelayCommand<AxisMoveArgs> Move
        {
            get
            {
                return new RelayCommand<AxisMoveArgs>(arg =>
                {
                    OnMoveRequested?.Invoke(this, arg);
                });
            }
        }
        
        public RelayCommand Stop
        {
            get
            {
                return new RelayCommand(() =>
                {
                    OnStopRequsted?.Invoke(this, new EventArgs());
                });
            }
        }

        public RelayCommand<AxisCruiseArgs> Curise
        {
            get
            {
                return new RelayCommand<AxisCruiseArgs>(args =>
                {
                    OnCuriseRequested?.Invoke(this, args);
                });
            }
        }
                
        #endregion
    }
}
