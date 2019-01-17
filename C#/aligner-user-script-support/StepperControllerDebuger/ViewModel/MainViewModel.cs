using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IrixiStepperControllerHelper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StepperControllerDebuger.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IDisposable
    {
        #region Variables

        IrixiMotionController _controller;
        string _device_sn = "";
        string _conn_prog_msg = "";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                //Code runs in Blend-- > create design time data.
            }
            else
            {
                ThreadPool.SetMinThreads(10, 10);

                this.DeviceSN = GlobalVariables.HidSN;

                _controller = new IrixiMotionController(GlobalVariables.HidSN); // For debug, the default SN of the controller is used.
                //
                // While scanning the controller, report the state to user window
                //
                _controller.OnConnectionStatusChanged += new EventHandler<ConnectionEventArgs>((sender, args) =>
                {
                    switch (args.Event)
                    {
                        case ConnectionEventArgs.EventType.ConnectionSuccess:
                            this.ConnectionStateChangedMessage = "Connected";
                            for (int i = 0; i < _controller.AxisCollection.Count; i++)
                            {
                                _controller.AxisCollection[i].SoftCWLS = 1000000;
                                _controller.AxisCollection[i].MaxSteps = 1000000;
                            }
                            break;

                        case ConnectionEventArgs.EventType.Connecting: // how many times tried to connect to the device is reported
                            this.ConnectionStateChangedMessage = "Connecting to the controller ...";
                            break;

                        case ConnectionEventArgs.EventType.ConnectionLost:
                            this.ConnectionStateChangedMessage = "Lost the connection";
                            break;
                    }

                });

                _controller.OnInputIOStatusChanged += new EventHandler<InputIOEventArgs>((s, e) =>
                {
                    if (e.Channel == 0 && e.State == InputState.Triggered)
                    {
                        _controller.SetGeneralOutput(0, OutputState.Enabled);
                    }
                });


                //
                // Once the report is received, update the UI components.
                //
                _controller.OnReportUpdated += new EventHandler<IrixiStepperControllerHelper.DeviceStateReport>((sender, report) =>
                {
                    //
                    // Nothing to do in this demo
                    // 
                    //
                });

                //_controller.OpenDeviceAsync();
                Open();

                for (int i = 0; i < 5; i++)
                {
                    Task.Factory.StartNew(DoFooWork);
                }
            }
        }

        #endregion

        #region Properties

        public string DeviceSN
        {
            private set
            {
                _device_sn = value;
                RaisePropertyChanged();
            }
            get
            {
                return _device_sn;
            }
        }

        /// <summary>
        /// Get the message about the connection progress
        /// </summary>
        public string ConnectionStateChangedMessage
        {
            private set
            {
                _conn_prog_msg = value;
                RaisePropertyChanged();
            }
            get
            {
                return _conn_prog_msg;
            }
        }

        /// <summary>
        /// Get the instance of the stepper controller class
        /// </summary>
        public IrixiMotionController StepperController
        {
            get
            {
                return _controller;
            }
        }

        #endregion

        #region Methods

        void DoFooWork()
        {
            int n = 0;

            while(true)
            {
                
                n++;
                for (int i = 0; i < 10000; i++)
                {
                    n++;
                }

                Thread.Sleep(10);
            }
        }

        void Open()
        {
           var ret = Task.Factory.StartNew(_controller.Open).Result;
        }

        /// <summary>
        /// Home all axes
        /// </summary>
        void Home()
        {
            Task<bool>[] tasks = new Task<bool>[_controller.TotalAxes];
            bool[] retvals = new bool[_controller.TotalAxes];

            for (int i = 0; i < _controller.TotalAxes; i++)
            {
                tasks[i] = _controller.HomeAsync(i);

                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Home the specified axis
        /// </summary>
        /// <param name="AxisIndex"></param>
        void Home(int AxisIndex)
        {
            _controller.HomeAsync(AxisIndex);
        }
        
        async void Move(CommandStruct args)
        {
            bool success = await _controller.MoveAsync(args.AxisIndex, args.DriveVelocity, args.TotalSteps, args.Mode);
            if (!success)
            {
                //Messenger.Default.Send<NotificationMessage<string>>(
                //    new NotificationMessage<string>(
                //        string.Format("Unable to move, {0}", _controller.LastError),
                //        "Error"));

            }
        }

        /// <summary>
        /// Read the information of the firmware like version, compiled date
        /// </summary>
        void ReadFirmwareInfo()
        {
            var ret = _controller.ReadFWInfo();
            if (ret)
            {
                Messenger.Default.Send(
                    new NotificationMessage<string>(
                        string.Format("{0}", _controller.FirmwareInfo.ToString()),
                        "MSG"));
            }
            else
            {
                Messenger.Default.Send(
                    new NotificationMessage<string>(
                        string.Format("Unable to read firmware info."),
                        "Error"));
            }
        }

        /// <summary>
        /// Read the input status registers of PCA9534s
        /// </summary>
        void ReadPCA9534InputReg()
        {
            var ret = _controller.ReadPCA9534();
            if (ret)
            {
                Messenger.Default.Send(
                    new NotificationMessage<string>(
                        string.Format("{0}", _controller.PCA9534Info.ToString()),
                        "MSG"));
            }
            else
            {
                Messenger.Default.Send(
                    new NotificationMessage<string>(
                        string.Format("Unable to read PCA9534."),
                        "Error"));
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Home all axis
        /// </summary>
        public RelayCommand<int> CommandHome
        {
            get
            {
                return new RelayCommand<int>(axisid =>
                {
                    if (axisid == -1)
                        Home();
                    else
                        Home(axisid);
                });
            }
        }

        /// <summary>
        /// Move the specified axis
        /// </summary>
        public RelayCommand<CommandStruct> CommandMove
        {
            get
            {
                return new RelayCommand<IrixiStepperControllerHelper.CommandStruct>(args =>
                {
                    // about the null, ref to the explain above
                    if (args == null)
                    {
                        Messenger.Default.Send<NotificationMessage<string>>(
                                new NotificationMessage<string>(
                                    string.Format("The move parameter could not be null."),
                                    "Error"));
                    }
                    else if (args.Command == IrixiStepperControllerHelper.EnumCommand.MOVE)
                    {
                        Move(args);
                    }
                    else
                    {
                        // Got the wrong command
                        Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>("The command flag is not Move.", "Error"));
                    }
                });
            }
        }
        
        /// <summary>
        /// Stop the specified axis
        /// </summary>
        public RelayCommand<int> CommandStop
        {
            get
            {
                return new RelayCommand<int>(axisid =>
                {

                    bool success = _controller.Stop(axisid);
                    if (!success)
                    {
                        Messenger.Default.Send<NotificationMessage<string>>(
                            new NotificationMessage<string>(
                                string.Format("Unable to stop, {0}", _controller.LastError),
                                "Error"));
                    }

                });
            }
        }

        public RelayCommand<Tuple<int, bool>> CommandSetMoveDirection
        {
            get
            {
                return new RelayCommand<Tuple<int, bool>>(arg =>
                {
                    bool ret = _controller.ReverseOriginPosition(arg.Item1, arg.Item2);
                });
            }
        }

        public RelayCommand<Tuple<int, OutputState>> CommandSetGeneralOutput
        {
            get
            {
                return new RelayCommand<Tuple<int, OutputState>>(arg =>
                {
                    _controller.SetGeneralOutput(arg.Item1, arg.Item2);
                });
            }
        }

        public RelayCommand CommandReadFWInfo
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ReadFirmwareInfo();
                });
            }
        }

        public RelayCommand CommandReadPCA9534
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ReadPCA9534InputReg();
                });
            }
        }

        public RelayCommand CommandCopySN
        {
            get
            {
                return new RelayCommand(() =>
                {
                    System.Windows.Clipboard.SetDataObject(_controller.SerialNumber);
                });
            }
        }

        #endregion

        public void Dispose()
        {
            _controller.Dispose();
        }
    }
}