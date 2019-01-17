using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using USBHIDDRIVER.USB;

namespace IrixiStepperControllerHelper
{
    public class IrixiMotionController : INotifyPropertyChanged, IDisposable
    {
        #region Constant Definition

        const string VID = "vid_0483";
        const string PID = "pid_574e";

        /// <summary>
        /// HidReport ID 1: report contains device status
        /// </summary>
        const int REPORT_ID_DEVICESTATE = 0x1;

        const int REPORT_ID_AXISSTATE = 0x2;

        /// <summary>
        /// HidReport ID 10: report contains firmware information
        /// </summary>
        const int REPORT_ID_FACTINFO = 0xA;
        

        /// <summary>
        /// The total steps which is used to acceleration and deceleration
        /// </summary>
        const int ACC_DEC_STEPS = 2000;

        /// <summary>
        /// The maximum drive veloctiy
        /// The real velocity is Velocity_Set(%) * MAX_VELOCITY
        /// </summary>
        const int MAX_VELOCITY = 15000;

        #endregion

        #region Variables

        /// <summary>
        /// Implement INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The event rasise while the hid report updated
        /// </summary>
        public event EventHandler<DeviceStateReport> OnReportUpdated;

        /// <summary>
        /// The event raises while the connection status changed
        /// </summary>
        public event EventHandler<ConnectionEventArgs> OnConnectionStatusChanged;

        /// <summary>
        /// The event raises while the status of the input IO changed
        /// </summary>
        public event EventHandler<InputIOEventArgs> OnInputIOStatusChanged;
        
        /// <summary>
        /// lock the usb port while any thread is transferring the data
        /// </summary>
        SemaphoreSlim lockUSBPort = new SemaphoreSlim(1, 1);
        
        HIDUSBDevice hidPort;

        bool _is_connected = false; // whether the contoller is connected
        string lastError = string.Empty, deviceSN = string.Empty;
        byte commandOrder = 0;

        CancellationTokenSource ctsObtainDeviceState;
        CancellationToken ctObtainDeviceState;
        ManualResetEvent pauseObtainDeviceStateTask;
        Task taskObtainDeviceState;

        /// <summary>
        /// the time of last operation
        /// </summary>
        static DateTime timeLastOperation = DateTime.MinValue;

        /// <summary>
        /// how many axes are busy
        /// </summary>
        int busyAxes = 0;
        
        #endregion

        #region Constructors

        public IrixiMotionController(string DeviceSN = "")
        {
            // Generate the instance of the state report object
            HidReport = new DeviceStateReport();
            FirmwareInfo = new FimwareInfo();
            PCA9534Info = new PCA9534Info();
            TotalAxes = -1;
            SerialNumber = DeviceSN;
            AxisCollection = new ObservableCollection<Axis>();

            //BindingOperations.EnableCollectionSynchronization(AxisCollection, _lock);
            hidPort = new HIDUSBDevice(VID, PID, DeviceSN);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Get the last error information
        /// </summary>
        public string LastError
        {
            private set
            {
                UpdateProperty<string>(ref lastError, value);
            }
            get
            {
                return lastError;
            }
        }

        /// <summary>
        /// Get the number of axes that the HID Controller supports
        /// </summary>
        public int TotalAxes
        {
            private set;
            get;
        }

        /// <summary>
        /// Get the serial number of HID stepper controller
        /// </summary>
        public string SerialNumber
        {
            private set
            {
                UpdateProperty<string>(ref deviceSN, value);
            }
            get
            {
                return deviceSN;
            }
        }

        /// <summary>
        /// Get whether the HID Controller is connected
        /// </summary>
        public bool IsConnected
        {
            private set
            {
                UpdateProperty<bool>(ref _is_connected, value);
            }
            get
            {
                return _is_connected;
            }
        }

        /// <summary>
        /// Get the state report from the HID Controller
        /// </summary>
        public DeviceStateReport HidReport
        {
            private set;
            get;
        }

        /// <summary>
        /// Get the infomation of the firmware which consists of verion and compiled date
        /// </summary>
        public FimwareInfo FirmwareInfo
        {
            private set;
            get;
        }

        public PCA9534Info PCA9534Info
        {
            private set;
            get;
        }

        /// <summary>
        /// Get the axis collection instance of the device
        /// </summary>
        public ObservableCollection<Axis> AxisCollection
        {
            private set;
            get;
        }


        #endregion

        #region Public Methods
        /// <summary>
        /// Read the controllers' serial number and output as a string list
        /// </summary>
        /// <returns></returns>
        public static string[] GetDevicesList()
        {
            HIDUSBDevice hid = new HIDUSBDevice(PID, VID);
            return hid.GetDevicesList().ToArray();
        }

        /// <summary>
        /// Open the controller
        /// </summary>
        public bool Open()
        {
            this.IsConnected = false;

            try
            {
                OnConnectionStatusChanged?.Invoke(this, new ConnectionEventArgs(ConnectionEventArgs.EventType.Connecting));

                if (hidPort.ConnectDevice())
                {
                    // to realize the mechanism of timeout, save the time when the initialization process is started
                    DateTime _init_start_time = DateTime.Now;

                    // Wait the first report from HID device in order to get the 'TotalAxes'
                    do
                    {
                        Request(EnumCommand.REQ_SYSTEM_STATE, out byte[] buff);

                        if (buff != null)
                        {
                            HidReport.Parse(buff);
                            TotalAxes = HidReport.TotalAxes;
                        }

                        // Don't check it so fast, the interval of two adjacent report is normally 20ms but not certain
                        Thread.Sleep(100);

                        // check whether the initialization process is timeout
                        if ((DateTime.Now - _init_start_time).TotalSeconds > 5)
                        {
                            break;
                        }


                    } while (TotalAxes <= 0);

                    // TotalAxes <= 0 indicates that no axis was found within 5 seconds, exit initialization process
                    if (this.TotalAxes > 0)
                    {

                        // the total number of axes returned, generate the instance of each axis
                        this.HidReport.AxisStateCollection.Clear();

                        // create the axis collection according the TotalAxes property in the hid report
                        for (int i = 0; i < this.TotalAxes; i++)
                        {
                            // generate axis state object to the controller report class
                            this.HidReport.AxisStateCollection.Add(new AxisStateReport()
                            {
                                AxisIndex = i
                            });

                            // generate axis control on the user window
                            this.AxisCollection.Add(new Axis()
                            {
                                // set the properties to the default value
                                MaxSteps = 0,
                                SoftCCWLS = 0,
                                SoftCWLS = 0,
                                PosAfterHome = 0,
                                MaxSpeed = MAX_VELOCITY,
                                AccelerationSteps = ACC_DEC_STEPS

                            });
                        }

                        // read the firmware info
                        if (ReadFWInfo())
                        {
                            // start to read the hid report repeatedly
                            StartObtainHidReport();

                            this.IsConnected = true;

                            // initialize this.HidReport property on UI thread
                            OnConnectionStatusChanged?.Invoke(this, new ConnectionEventArgs(ConnectionEventArgs.EventType.ConnectionSuccess));
                        }
                        else
                        {
                            Close();

                            lastError = "cannot get the firmware information";
                            OnConnectionStatusChanged?.Invoke(this, new ConnectionEventArgs(ConnectionEventArgs.EventType.ConnectionFailure, LastError));
                        }
                    }
                    else
                    {
                        LastError = "cannot get the total axes";
                        OnConnectionStatusChanged?.Invoke(this, new ConnectionEventArgs(ConnectionEventArgs.EventType.ConnectionFailure, LastError));
                    }
                }
                else
                {
                    this.LastError = "unable to open the usb port to connect to the controller";
                    OnConnectionStatusChanged?.Invoke(this, new ConnectionEventArgs(ConnectionEventArgs.EventType.ConnectionFailure, LastError));
                }
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
            }

            return IsConnected;
        }

        /// <summary>
        /// Open the controller asynchronously
        /// </summary>
        /// <returns></returns>
        public Task<bool> OpenAsync()
        {
            return Task.Run<bool>(() =>
            {
                return Open();

            });
        }

        /// <summary>
        /// Close the controller
        /// </summary>
        public void Close()
        {
            if (this.IsConnected == true)
            {
                StopObtainHidReport();
                hidPort.DisconnectDevice();
            }
        }
        
        /// <summary>
        /// Read firmware information
        /// after the information is returned, get the detail from FirmwareInfo property
        /// </summary>
        /// <returns></returns>
        public bool ReadFWInfo()
        {
            Request(EnumCommand.REQ_FIRMWARE_INFO, out byte[] buff);

            if (buff == null)
            {
                return false;
            }
            else
            {
                return FirmwareInfo.Parse(buff);
            }
        }
       
        /// <summary>
        /// read the value of PCA9534
        /// </summary>
        /// <returns></returns>
        public bool ReadPCA9534()
        {
            Request(EnumCommand.REQ_READ9534_STA, out byte[] buff);

            if (buff == null)
            {
                return false;
            }
            else
            {
                return PCA9534Info.Parse(buff);
            }
        }
        
        /// <summary>
        /// Home the specified axis synchronously
        /// </summary>
        /// <param name="AxisIndex">The axis index, this parameter should be 0 ~ 2</param>
        /// <returns></returns>
        public bool Home(int AxisIndex)
        {
            bool ret = false;

            if (AxisIndex >= this.HidReport.TotalAxes)
            {
                this.LastError = string.Format("The param of axis index if error.");
                return false;
            }
            // if the controller is not connected, return
            else if (!this.IsConnected)
            {
                this.LastError = string.Format("The controller is not connected.");
                return false;
            }

            // If the axis is busy, return.
            if (this.HidReport.AxisStateCollection[AxisIndex].IsRunning)
            {
                this.LastError = string.Format("Axis {0} is busy.", AxisIndex);
                return false;
            }

            PauseObtainHidReport();

            // start to home process
            try
            {
                if(SendHomeCommand(AxisIndex, out byte commandOrderExpected))
                {
                    if(WaitForLongtimeOperation(AxisIndex, commandOrderExpected, out AxisStateReport axisState))
                    {
                        if(axisState.IsHomed)
                        {
                            ret = true;
                        }
                        else
                        {
                            lastError = string.Format("error code {0:d}", axisState.Error);
                        }
                    }
                    else
                    {
                        lastError = "timeout to home the axis";
                    }
                }
                else
                {
                    lastError = "unable to send home command";
                }
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
            }
            finally
            {
                ResumeObtainHidReport();
            }

            return ret;
        }

        /// <summary>
        /// Home the speicified axis asynchronously
        /// </summary>
        /// <param name="Axis"></param>
        /// <returns></returns>
        public Task<bool> HomeAsync(int AxisIndex)
        {
            return Task.Run<bool>(() =>
            {
                return Home(AxisIndex);
            });
        }

        /// <summary>
        /// Move the specified axis synchronously
        /// </summary>
        /// <param name="AxisIndex"></param>
        /// <param name="Velocity"></param>
        /// <param name="Position"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        public bool Move(int AxisIndex, int Velocity, int Position, MoveMode Mode)
        {
            bool ret = false;

            // check if the axis is running
            if (AxisCollection[AxisIndex].IsBusy)
            {
                LastError = string.Format("axis {0} is busy", AxisIndex);
                return false;
            }
            else
                AxisCollection[AxisIndex].IsBusy = true;

            int _curr_pos = this.HidReport.AxisStateCollection[AxisIndex].AbsPosition;   // Get current ABS position
            int _pos_aftermove = 0;

            if (AxisIndex >= this.TotalAxes)
            {
                this.LastError = string.Format("axis index if error");
                goto _done;
            }
            // if the controller is not connected, return
            else if (!this.IsConnected)
            {
                this.LastError = string.Format("the controller is not connected");
                goto _done;
            }

            // If the axis is not homed, return.
            if (this.HidReport.AxisStateCollection[AxisIndex].IsHomed == false)
            {
                this.LastError = string.Format("axis {0} is not homed", AxisIndex);
                goto _done;
            }

            // If the axis is busy, return.
            if (this.HidReport.AxisStateCollection[AxisIndex].IsRunning)
            {
                this.LastError = string.Format("axis {0} is busy", AxisIndex);
                goto _done;
            }

            if (Velocity < 1 || Velocity > 100)
            {
                this.LastError = string.Format("the velocity should be 1 ~ 100");
                goto _done;
            }

            //
            // Validate the parameters restricted in the config file
            //
            // MaxDistance > 0
            if (this.AxisCollection[AxisIndex].MaxSteps < 0)
            {
                this.LastError = string.Format("the max steps is error");
                goto _done;
            }


            // SoftCWLS > SoftCCWLS
            if (this.AxisCollection[AxisIndex].SoftCCWLS > this.AxisCollection[AxisIndex].SoftCWLS)
            {
                this.LastError = string.Format("the soft cw limitation is less then the soft-ccw limitation");
                goto _done;
            }

            // SoftCCWLS <= MaxDistance
            if (this.AxisCollection[AxisIndex].SoftCWLS > this.AxisCollection[AxisIndex].MaxSteps)
            {
                this.LastError = string.Format("the soft cw limitation is less then the maximum steps");
                goto _done;
            }

            // SoftCCWLS <= PosAfterHome <= SoftCWLS
            if ((this.AxisCollection[AxisIndex].SoftCCWLS > this.AxisCollection[AxisIndex].PosAfterHome) ||
            (this.AxisCollection[AxisIndex].PosAfterHome > this.AxisCollection[AxisIndex].SoftCWLS))
            {
                this.LastError = string.Format("The value of the PosAfterHome exceeds the soft limitaion.");
                goto _done;
            }


            //
            // Validate the position after moving,
            // if the position exceeds the soft limitation, do not move
            //
            if (Mode == MoveMode.ABS)
            {
                if (Position < this.AxisCollection[AxisIndex].SoftCCWLS || Position > this.AxisCollection[AxisIndex].SoftCWLS)
                {
                    this.LastError = string.Format("The target position is out of range.");
                    goto _done;
                }
                else
                {
                    _pos_aftermove = Position;

                    Position = Position - _curr_pos;
                }
            }
            else // rel positioning
            {
                _pos_aftermove = _curr_pos + Position;

                if (Position > 0) // CW
                {
                    // if (_pos_aftermove > this.AxisCollection[AxisIndex].SoftCWLS)
                    if (_pos_aftermove > this.AxisCollection[AxisIndex].MaxSteps)
                    {
                        this.LastError = string.Format("The position you are going to move exceeds the soft CW limitation.");
                        goto _done;
                    }
                }
                else // CCW
                {
                    // if (_pos_aftermove < this.AxisCollection[AxisIndex].SoftCCWLS)
                    if (_pos_aftermove < 0)
                    {
                        this.LastError = string.Format("The position you are going to move exceeds the soft CCW limitation.");
                        goto _done;
                    }
                }
            }

            DateTime moveStart = DateTime.Now;

            PauseObtainHidReport();

            try
            {
                // No need to move
                if (Position == 0)
                {
                    ret = true;
                    goto _done;
                }

                if (SendMoveCommand(AxisIndex, AxisCollection[AxisIndex].AccelerationSteps, Velocity, Position, out byte commandOrderExpected))
                {
                    if (WaitForLongtimeOperation(AxisIndex, commandOrderExpected, out AxisStateReport axisState))
                    {
                        if (axisState.Error != 0)
                        {
                            LastError = string.Format("error code {0:d}", axisState.Error);
                            goto _done;
                        }
                        else
                        {
                            ret = true;
                        }
                    }
                    else
                    {
                        lastError = "timeout to wait move operation done.";
                        goto _done;
                    }
                }
                else
                {
                    lastError = "unable to send move command";
                }


                //Trace.WriteLine(string.Format("{0:mm:ss.ffffff}\tMOVE Command took {1}ms !", DateTime.Now, (DateTime.Now - moveStart).TotalMilliseconds));

                
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
            finally
            {
                ResumeObtainHidReport();
            }

            _done:

            Thread.Sleep(5);

            AxisCollection[AxisIndex].IsBusy = false;
            return ret;
        }

        /// <summary>
        /// Move the speified axis asynchronously
        /// </summary>
        /// <param name="AxisIndex"></param>
        /// <param name="Acceleration"></param>
        /// <param name="Velocity"></param>
        /// <param name="Distance"></param>
        /// <param name="Direction"></param>
        /// <returns></returns>
        public Task<bool> MoveAsync(int AxisIndex, int Velocity, int Distance, MoveMode Mode)
        {
            return Task.Run(() =>
            {
                return Move(AxisIndex, Velocity, Distance, Mode);
            });
        }
        
        /// <summary>
        /// Stop the movement immediately
        /// </summary>
        /// <param name="AxisIndex">-1: Stop all axis; Otherwise, stop the specified axis</param>
        /// <returns></returns>
        public bool Stop(int AxisIndex)
        {
            // if the controller is not connected, return
            if (!this.IsConnected)
            {
                this.LastError = string.Format("The controller is not connected.");
                return false;
            }

            try
            {
                CommandStruct cmd = new CommandStruct()
                {
                    Command = EnumCommand.STOP,
                    AxisIndex = AxisIndex
                };
                hidPort.WriteData(cmd.ToBytes());

                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Reverse the CW and CCW position (Mechanical Origin Position)
        /// </summary>
        /// <param name="AxisIndex"></param>
        /// <param name="IsReverse">True to reverse, False to default setting</param>
        /// <returns></returns>
        public bool ReverseOriginPosition(int AxisIndex, bool IsReverse)
        {
            // if the controller is not connected, return
            if (!this.IsConnected)
            {
                this.LastError = string.Format("The controller is not connected.");
                return false;
            }

            try
            {
                CommandStruct cmd = new CommandStruct()
                {
                    Command = EnumCommand.REVERSE,
                    AxisIndex = AxisIndex,
                    IsReversed = IsReverse
                };
                hidPort.WriteData(cmd.ToBytes());

                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Get the state of specified output port
        /// </summary>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public OutputState GetGeneralOutputState(int Channel)
        {
            int axis_id = Channel / 2;
            int port = Channel % 2;

            if (port == 0)
                return this.HidReport.AxisStateCollection[axis_id].OUT_A;
            else
                return this.HidReport.AxisStateCollection[axis_id].OUT_B;
        }

        /// <summary>
        /// Set the state of the general output I/O
        /// </summary>
        /// <param name="Channel">This should be 0 to 7</param>
        /// <param name="State">OFF/ON</param>
        /// <returns></returns>
        public bool SetGeneralOutput(int Channel, OutputState State)
        {
            // if the controller is not connected, return
            if (!this.IsConnected)
            {
                this.LastError = string.Format("The controller is not connected.");
                return false;
            }

            try
            {
                CommandStruct cmd = new CommandStruct()
                {
                    Command = EnumCommand.GENOUT,
                    AxisIndex = Channel / 2,        // calculate axis index by channel
                    GenOutPort = Channel % 2,    // calculate port by channel
                    GenOutState = State

                };
                hidPort.WriteData(cmd.ToBytes());

                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Flip the specified output port 
        /// </summary>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public bool ToggleGeneralOutput(int Channel)
        {
            // if the controller is not connected, return
            if (!this.IsConnected)
            {
                this.LastError = string.Format("The controller is not connected.");
                return false;
            }

            try
            {
                // flip the state of output port
                OutputState state = GetGeneralOutputState(Channel);
                if (state == OutputState.Disabled)
                    state = OutputState.Enabled;
                else
                    state = OutputState.Disabled;

                CommandStruct cmd = new CommandStruct()
                {
                    Command = EnumCommand.GENOUT,
                    AxisIndex = Channel / 2,        // calculate axis index by channel
                    GenOutPort = Channel % 2,    // calculate port by channel
                    GenOutState = state

                };
                hidPort.WriteData(cmd.ToBytes());

                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                return false;
            }
        }

        public void Dispose()
        {
            if (this.IsConnected == true)
            {
                StopObtainHidReport();
                hidPort.DisconnectDevice();
            }
        }

        #endregion

        #region Private Methods

        void StartObtainHidReport()
        {
            if (taskObtainDeviceState == null || taskObtainDeviceState.IsCompleted)
            {
                ctsObtainDeviceState = new CancellationTokenSource();
                ctObtainDeviceState = ctsObtainDeviceState.Token;

                pauseObtainDeviceStateTask = new ManualResetEvent(true);

                var progressHandler = new Progress<byte[]>(value =>
                {
                    var report = HidReport.Clone() as DeviceStateReport;
                    this.HidReport.Parse(value);
                    OnReportUpdated?.Invoke(this, HidReport);

                    for (int i = 0; i < HidReport.AxisStateCollection.Count; i++)
                    {
                        if (this.HidReport.AxisStateCollection[i].IN_A != report.AxisStateCollection[i].IN_A)
                            OnInputIOStatusChanged?.Invoke(this, new InputIOEventArgs(i * 2, HidReport.AxisStateCollection[i].IN_A));

                        if (this.HidReport.AxisStateCollection[i].IN_B != report.AxisStateCollection[i].IN_B)
                            OnInputIOStatusChanged?.Invoke(this, new InputIOEventArgs(i * 2 + 1, HidReport.AxisStateCollection[i].IN_B));
                    }
                });
                var progress = progressHandler as IProgress<byte[]>;

                taskObtainDeviceState = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        DateTime start = DateTime.Now;
                        //Trace.WriteLine(string.Format("{0:mm:ss.ffffff}\tObtain HID report ...", start));

                        Request(EnumCommand.REQ_SYSTEM_STATE, out byte[] buff);

                        if (buff != null)
                        {
                            progress.Report(buff);
                        }

                        //Trace.WriteLine(string.Format("{0:mm:ss.ffffff}\tHID report is received, takes {1:F6}ms", DateTime.Now, (DateTime.Now - start).TotalMilliseconds));

                        /*
                         * Delay some seconds after the last operation
                         */
                        Thread.Sleep(200);
                        pauseObtainDeviceStateTask.WaitOne();

                        while((DateTime.Now - timeLastOperation).TotalMilliseconds < 500)
                        {
                            Thread.Sleep(200);
                            ;
                        }

                        ctObtainDeviceState.ThrowIfCancellationRequested();
                    }
                }, TaskCreationOptions.LongRunning);

                //await taskObtainDeviceState;
            }
        }

        void PauseObtainHidReport()
        {
            // ensure the task is running
            if (taskObtainDeviceState != null && !taskObtainDeviceState.IsCompleted)
            {
                if(Interlocked.Increment(ref busyAxes) == 1)
                    pauseObtainDeviceStateTask.Reset();
            }
        }

        void ResumeObtainHidReport()
        {
            // ensure the task is running
            if (taskObtainDeviceState != null && !taskObtainDeviceState.IsCompleted)
            {
                if (Interlocked.Decrement(ref busyAxes) == 0)
                {
                    pauseObtainDeviceStateTask.Set();
                    timeLastOperation = DateTime.Now;
                }
            }
        }

        void StopObtainHidReport()
        {
            if (taskObtainDeviceState != null && !taskObtainDeviceState.IsCompleted)
            {
                ctsObtainDeviceState.Cancel();
                while (!taskObtainDeviceState.IsCompleted)
                {
                    Thread.Sleep(50);
                }
            }
        }

        bool SendHomeCommand(int AxisIndex, out byte CommandOrderExpected)
        {
            bool ret = false;
            CommandOrderExpected = 0;

            lockUSBPort.Wait();
            try
            {
                CommandOrderExpected = ++commandOrder;

                CommandStruct cmd = new CommandStruct()
                {
                    Command = EnumCommand.HOME,
                    CommandOrder = CommandOrderExpected,
                    AxisIndex = AxisIndex
                };

                ret = hidPort.WriteData(cmd.ToBytes());
            }
            catch
            {
                ret = false;
            }
            finally
            {
                lockUSBPort.Release();
            }

            return ret;
        }

        bool SendMoveCommand(int AxisIndex, int Acceleration, int DriverVelocityPercent, int TotalSteps, out byte CommandOrderExpected)
        {
            bool ret = false;
            CommandOrderExpected = 0;

            lockUSBPort.Wait();
            try
            {
                CommandOrderExpected = ++commandOrder;

                CommandStruct cmd = new CommandStruct()
                {
                    Command = EnumCommand.MOVE,
                    CommandOrder = (byte)CommandOrderExpected,
                    AxisIndex = AxisIndex,
                    AccSteps = Acceleration,
                    DriveVelocity = DriverVelocityPercent * this.AxisCollection[AxisIndex].MaxSpeed / 100,
                    TotalSteps = TotalSteps
                };

                ret = hidPort.WriteData(cmd.ToBytes());
            }
            catch
            {
                ret = false;
            }
            finally
            {
                lockUSBPort.Release();
            }

            return ret;
        }

        bool Request(EnumCommand Command, out byte[] Buffer)
        {
            bool ret = false;
            Buffer = null;

            lockUSBPort.Wait();

            //Trace.WriteLine(string.Format("Thread {0}, Enter request ...", Thread.CurrentThread.ManagedThreadId));
            try
            {
                CommandStruct cmd = new CommandStruct()
                {
                    Command = Command,
                };

                if (hidPort.WriteData(cmd.ToBytes()))
                {
                    Buffer = hidPort.ReadData();
                    if (Buffer != null)
                    {
                        ret = true;
                    }
                }
            }
            catch
            {
                ret = false;
            }
            finally
            {
                lockUSBPort.Release();
                //Trace.WriteLine(string.Format("Thread {0}, Exit request ...", Thread.CurrentThread.ManagedThreadId));
            }

            return ret;
        }

        bool RequestAxisState(int AxisIndex, out AxisStateReport AxisState)
        {
            bool ret = false;
            AxisState = new AxisStateReport();
            lockUSBPort.Wait();


            //Trace.WriteLine(string.Format("Thread {0}, Enter request axis {1} ...", Thread.CurrentThread.ManagedThreadId, AxisIndex));
            try
            {
                CommandStruct cmd = new CommandStruct()
                {
                    Command = EnumCommand.REQ_AXIS_STATE,
                    AxisIndex = AxisIndex
                };

                if (hidPort.WriteData(cmd.ToBytes()))
                {
                    var buff = hidPort.ReadData();
                    if (buff != null)
                    {
                        if(buff[0] == REPORT_ID_AXISSTATE)
                        {
                            var buffAvailable = new byte[buff.Length - 1];
                            Buffer.BlockCopy(buff, 1, buffAvailable, 0, buffAvailable.Length);
                            AxisState.Parse(buffAvailable);

                            ret = true;
                        }
                    }
                }
            }
            catch
            {
                ret = false;
            }
            finally
            {
                lockUSBPort.Release();
            }
            //Trace.WriteLine(string.Format("Thread {0}, Exit request ...", Thread.CurrentThread.ManagedThreadId));
            return ret;
        }

        bool WaitForLongtimeOperation(int AxisIndex, byte CommandOrderExpected, out AxisStateReport AxisState)
        {
            bool timeout = false;
            int lastPosition = 0;
            DateTime reqStart = DateTime.Now;

            do
            {

                if (RequestAxisState(AxisIndex, out AxisState))
                {
                    var axisStateInReport = HidReport.AxisStateCollection[AxisIndex];
                    axisStateInReport.IsRunning = AxisState.IsRunning;
                    axisStateInReport.IsHomed = AxisState.IsHomed;
                    axisStateInReport.AbsPosition = AxisState.AbsPosition;
                    axisStateInReport.Error = AxisState.Error;

                    OnReportUpdated?.Invoke(this, HidReport);

                    // Check whether the home process is alive
                    if (lastPosition != AxisState.AbsPosition)
                    {
                        reqStart = DateTime.Now;
                        lastPosition = AxisState.AbsPosition;
                    }

                    //Trace.WriteLine(string.Format("{0:mm:ss.ffffff}\tCommandOrder in Report {1}, Desired {2} ...", DateTime.Now, AxisState.CommandOrder, CommandOrderExpected));

                    if (AxisState.CommandOrder == CommandOrderExpected)
                    {
                        // the operation is done
                        break;
                    }
                }

                // check if it's timeout
                if ((DateTime.Now - reqStart).TotalSeconds > 5)
                {
                    timeout = true;
                    break;
                }

                Thread.Sleep(20);

            } while (true);

            if (timeout)
                return false;
            else
                return true;
        }

        #endregion

        #region Events
        
        #endregion

        #region RaisePropertyChangedEvent
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        /// <param name="PropertyName"></param>
        protected void UpdateProperty<T>(ref T OldValue, T NewValue, [CallerMemberName]string PropertyName = "")
        {
            if (object.Equals(OldValue, NewValue))  // To save resource, if the value is not changed, do not raise the notify event
                return;

            OldValue = NewValue;                // Set the property value to the new value
            OnPropertyChanged(PropertyName);    // Raise the notify event
        }

        protected void OnPropertyChanged([CallerMemberName]string PropertyName = "")
        {
            //PropertyChangedEventHandler handler = PropertyChanged;
            //if (handler != null)
            //    handler(this, new PropertyChangedEventArgs(PropertyName));
            //RaisePropertyChanged(PropertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        }

        #endregion
    }
}
