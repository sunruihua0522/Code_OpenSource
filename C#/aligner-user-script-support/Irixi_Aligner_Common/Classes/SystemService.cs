using GalaSoft.MvvmLight.Messaging;
using Irixi_Aligner_Common.Alignment.CentralAlign;
using Irixi_Aligner_Common.Alignment.FastCentralAlign;
using Irixi_Aligner_Common.Alignment.FastND;
using Irixi_Aligner_Common.Alignment.FastRotatingScan;
using Irixi_Aligner_Common.Alignment.ProfileND;
using Irixi_Aligner_Common.Alignment.RotatingProfile;
using Irixi_Aligner_Common.Alignment.SnakeRouteScan;
using Irixi_Aligner_Common.Alignment.SpiralScan;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Configuration.Common;
using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Equipments.Base;
using Irixi_Aligner_Common.Equipments.BaseClass;
using Irixi_Aligner_Common.Equipments.Equipments;
using Irixi_Aligner_Common.Equipments.Equipments.Instruments;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using Irixi_Aligner_Common.Equipments.Instruments;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.Message;
using Irixi_Aligner_Common.MotionControllers.Base;
using Irixi_Aligner_Common.MotionControllers.Irixi;
using Irixi_Aligner_Common.MotionControllers.Luminos;
using Irixi_Aligner_Common.UserScript;
using M12.Base;
using M12.Definitions;
using M12.Excpections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Irixi_Aligner_Common.Classes
{
    public class SystemService : INotifyPropertyChanged, IDisposable
    {
        #region Variables

        private MessageItem lastMessage = null;
        private SystemState sysState = SystemState.IDLE;

        private bool isInitialized = false;
        private readonly DateTime initStartTime;

        private readonly EquipmentCollection<LogicalAxis> listLogicalAxis;
        private readonly EquipmentCollection<LogicalAxis> listLogicalAxisInAligner;
        private readonly EquipmentCollection<LogicalMotionComponent> listLogicalMotionComponent;
        private readonly EquipmentCollection<LogicalMotionComponent> listLogicalAligner;
        private readonly EquipmentCollection<InstrumentBase> listAvaliableInstrument;
        private readonly EquipmentCollection<InstrumentBase> listDefinedInstrument;
        private readonly EquipmentCollection<InternalPowerMeter> listInternalPowerMeter;

        /// <summary>
        /// lock while set or get this.State
        /// </summary>
        private readonly object lockSystemStatus = new object();

        /// <summary>
        /// Output put initialization messages
        /// </summary>
        public event EventHandler<string> InitProgressChanged;

        /// <summary>
        /// Background task
        /// </summary>
        PauseTokenSource bgTaskPTS;
        List<Task> bgTaskList;

        #endregion Variables

        #region Constructor

        public SystemService()
        {
            initStartTime = DateTime.Now;

            //ThreadPool.SetMinThreads(50, 50);

            // read version from AssemblyInfo.cs
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            SysVersion = version.ToString();

            // force to enable the log, otherwise the initial message could not be recored
            LogHelper.LogEnabled = true;

            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n");
            sb.Append("> =================================================================\r\n");
            sb.Append("> =             100G+ All-Purspose Alignment System               =\r\n");
            sb.Append("> =                Copyright (C) 2017 - 2019 Irixi                =\r\n");
            sb.Append("> =================================================================\r\n");
            LogHelper.WriteLine(sb.ToString());

            SetMessage(MessageType.Normal, "System startup ...");

            SetMessage(MessageType.Normal, $"Application Version {version}");

            // read the configuration from the file named SystemCfg.json
            // the file is located in \Configuration
            SystemSettings = new ConfigManager();

            PositionPresetProfileManager = new PositionPresetManager();

            ListBasedScriptManager = new UserScriptManager(this);

            // whether output the log
            LogHelper.LogEnabled = SystemSettings.ConfSystemSetting.LogEnabled;

            State = SystemState.BUSY;

            // initialize the properties
            BusyComponents = new List<IStoppable>();

            PhysicalMotionControllerCollection = new Dictionary<Guid, IMotionController>();

            listLogicalAxis = new EquipmentCollection<LogicalAxis>();
            listLogicalAxisInAligner = new EquipmentCollection<LogicalAxis>();
            listLogicalMotionComponent = new EquipmentCollection<LogicalMotionComponent>();
            listLogicalAligner = new EquipmentCollection<LogicalMotionComponent>();
            listAvaliableInstrument = new EquipmentCollection<InstrumentBase>();
            listDefinedInstrument = new EquipmentCollection<InstrumentBase>();
            listInternalPowerMeter = new EquipmentCollection<InternalPowerMeter>();

            SpiralScanHandler = new SpiralScan(new SpiralScanArgs());
            SnakeRouteScanHandler = new SnakeRouteScan(new SnakeRouteScanArgs());
            FastRotatingScanHandler = new FastRotatingScan(new FastRotatingScanArgs());
            FastNDHandler = new FastND(new FastNDArgs());
            ProfileNDHandler = new ProfileND(new ProfileNDArgs());
            RotatingScanHandler = new RotatingScan(new RotatingScanArgs());
            FastCentralAlignHandler = new FastCentralAlign(new FastCentralAlignArgs());
            CentralAlignHandler = new CentralAlign(new CentralAlignArgs());

            #region Create objects of equipment available in system.
            
            /*
             * enumerate all physical motion controllers defined in the config file,
             * and create the instance of the motion controller class.
             */
            foreach (var conf in SystemSettings.ConfSystemSetting.PhysicalMotionControllers)
            {
                IMotionController motion_controller = null;

                switch (conf.Model)
                {
                    case MotionControllerType.LUMINOS_P6A:
                        motion_controller = new LuminosP6A(conf);
                        break;

                    case MotionControllerType.IRIXI_EE0017:
                        motion_controller = new IrixiEE0017(conf);

                        ((IrixiEE0017)motion_controller).OnMessageReported += ((sender, message) =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                SetMessage(MessageType.Normal, $"{sender} {message}");
                            });
                        });
                        break;

                    case MotionControllerType.IRIXI_M12:
                        motion_controller = new IrixiM12(conf);
                        break;

                    default:
                        SetMessage(MessageType.Error, $"Unknown motion controller {conf.Model}.");
                        break;
                }

                motion_controller.OnMoveBegin += PhysicalMotionController_OnMoveBegin;
                motion_controller.OnMoveEnd += PhysicalMotionController_OnMoveEnd;

                // Add the controller to the dictionary<Guid, Controller>
                if (motion_controller != null)
                {
                    this.PhysicalMotionControllerCollection.Add(motion_controller.DeviceClass, motion_controller);
                }
            }

            // create the instance of the Logical Motion Components
            foreach (var cfgLMC in SystemSettings.ConfSystemSetting.LogicalMotionComponents)
            {
                LogicalMotionComponent logicalMotionComp = new LogicalMotionComponent(cfgLMC.Caption, cfgLMC.Icon, cfgLMC.IsAligner);

                foreach (var cfgLogicalAxis in cfgLMC.LogicalAxisArray)
                {
                    // new logical axis object will be added to the Logical Motion Component
                    LogicalAxis LAxis = new LogicalAxis(cfgLogicalAxis, cfgLMC.Caption);

                    LAxis.OnHomeRequested += LogicalAxis_OnHomeRequsted;
                    LAxis.OnMoveRequested += LogicalAxis_OnMoveRequsted;
                    LAxis.OnStopRequsted += LogicalAxis_OnStopRequsted;
                    LAxis.OnCuriseRequested += LAxis_OnCuriseRequested;

                    // bind the physical axis instance to logical axis
                    var phyAxis = FindPhysicalAxisByLogicalAxisConfig(LAxis);
                    if (phyAxis != null)
                    {
                        LAxis.PhysicalAxisInst = phyAxis;
                        logicalMotionComp.Add(LAxis);
                        listLogicalAxis.Add(LAxis);

                        // if the logical motion controller is aligner, added to the logical aligner list.
                        if (logicalMotionComp.IsAligner)
                            listLogicalAxisInAligner.Add(LAxis);
                    }
                    else
                    {
                        SetMessage(MessageType.Error, $"Unable to find the matching axis of {LAxis}.");
                    }
                }

                listLogicalMotionComponent.Add(logicalMotionComp);


                if (logicalMotionComp.IsAligner)
                    listLogicalAligner.Add(logicalMotionComp);
            }

            // create the instance of the cylinder controller
            try
            {
                IrixiM12 controller = PhysicalMotionControllerCollection[Guid.Parse(SystemSettings.ConfSystemSetting.Cylinder.Port)] as IrixiM12;
                CylinderController = new CylinderController(SystemSettings.ConfSystemSetting.Cylinder, controller);
            }
            catch(Exception ex)
            {
                SetMessage(MessageType.Error, $"Unable to create the object of the cylinder controller, {ex.Message}");
                CylinderController = null;
            }

            #region Measurement Instrument Creation

            // create the instance of the contact sensor.
            List<ContactSensor> listCSS = new List<ContactSensor>();
            foreach (var config in SystemSettings.ConfSystemSetting.ContactSensor)
            {
                try
                {
                    IrixiM12 _m12 = PhysicalMotionControllerCollection[Guid.Parse(config.Port)] as IrixiM12;
                    var css = new ContactSensor(config, _m12);

                    listCSS.Add(css);
                    listDefinedInstrument.Add(css);
                }
                catch (Exception ex)
                {
                    SetMessage(MessageType.Error, $"Unable to create the object of the {config.Caption}, {ex.Message}");
                }
            }
            this.CSS = listCSS.ToArray();
            
            // create the instance of the internal powermeters.
            foreach (var config in SystemSettings.ConfSystemSetting.InternalPowerMeter)
            {
                try
                {
                    IrixiM12 m12 = PhysicalMotionControllerCollection[Guid.Parse(config.Port)] as IrixiM12;

                    var frontend = new PDFrontEnd(config.PDFrontEnd);
                    var inst = new InternalPowerMeter(config, m12, frontend);

                    listDefinedInstrument.Add(inst);
                    listInternalPowerMeter.Add(inst);
                }
                catch (Exception ex)
                {
                    SetMessage(MessageType.Error, $"Unable to create the instance of the PD Front-End, {ex.Message}");
                }
            }

            // create instance of the keithley 2400
            foreach (var cfg in SystemSettings.ConfSystemSetting.Keithley2400s)
            {
                if (cfg.Enabled)
                    listDefinedInstrument.Add(new Keithley2400(cfg));
            }

            // create instance of the newport 2832C
            foreach (var cfg in SystemSettings.ConfSystemSetting.Newport2832Cs)
            {
                if (cfg.Enabled)
                    listDefinedInstrument.Add(new Newport2832C(cfg));
            }


            #endregion

            #endregion

            #region Create background tasks

            bgTaskList = new List<Task>();
            bgTaskPTS = new PauseTokenSource();

            // Create the background tasks for M12 to get analog input values and DIN status.
            foreach(var item in PhysicalMotionControllerCollection)
            {
                var controller = item.Value;
                if(controller is IrixiM12)
                {
                    ADCChannels adcEnabled = 0;
                    var config = controller.Config as ConfigPhysicalMotionController;
                    if(config.ANAEnabled != null && config.ANAEnabled.Length > 0)
                    {
                        
                        foreach(var str in config.ANAEnabled)
                        {
                            if(Enum.TryParse<ADCChannels>(str, out ADCChannels ch))
                            {
                                adcEnabled |= ch;
                            }
                            else
                            {
                                SetMessage(MessageType.Error, $"Unable to parse ADC Channel {str} defined in {controller}.");
                            }
                        }
                    }

                    bgTaskList.Add(CreateM12BackgroundTask((IrixiM12)controller, adcEnabled));
                }
            }

            #endregion
        }

        #endregion Constructor

        #region Events

        private void LogicalAxis_OnHomeRequsted(object sender, EventArgs args)
        {
            var s = sender as LogicalAxis;
            Home(s.PhysicalAxisInst);
        }

        private void LogicalAxis_OnMoveRequsted(object sender, AxisMoveArgs args)
        {
            var s = sender as LogicalAxis;
            MoveLogicalAxis(s, args);
        }

        private void LAxis_OnCuriseRequested(object sender, AxisCruiseArgs e)
        {
            var s = sender as LogicalAxis;
            CruiseLogcialAxis(s, e);
        }

        private void LogicalAxis_OnStopRequsted(object sender, EventArgs args)
        {
            var s = sender as LogicalAxis;
            s.PhysicalAxisInst.Stop();
        }

        private void PhysicalMotionController_OnMoveBegin(object sender, EventArgs args)
        {
            var imbusy = (IStoppable)sender;
            RegisterBusyComponent(imbusy);
        }

        private void PhysicalMotionController_OnMoveEnd(object sender, EventArgs args)
        {
            var imfree = (IStoppable)sender;
            UnregisterBusyComponent(imfree);
        }

        #endregion Events

        #region Properties

        public string SysVersion { get; }

        public ConfigManager SystemSettings { get; }

        public PositionPresetManager PositionPresetProfileManager { get; }

        public UserScriptManager ListBasedScriptManager { get; }

        /// <summary>
        /// Get or set the list of the busy devices/processes, this list is used to stop the busy devices or processes such as alignment process, user-process, etc.
        /// </summary>
        private List<IStoppable> BusyComponents { get; }

        /// <summary>
        /// Does the system service have been initialized ?
        /// it's mainly used to set the enabled property of UI elements.
        /// </summary>
        public bool IsInitialized
        {
            private set
            {
                isInitialized = value;
                UpdateProperty(ref isInitialized, value);
            }
            get
            {
                return isInitialized;
            }
        }

        /// <summary>
        /// Get the system status
        /// </summary>
        public SystemState State
        {
            private set
            {
                UpdateProperty(ref sysState, value);
            }
            get
            {
                return sysState;
            }
        }

        /// <summary>
        /// Get the instance of the Cylinder Controller Class
        /// </summary>
        public CylinderController CylinderController
        {
            get;
        }

        /// <summary>
        /// Get the instance of the contact sensors.
        /// </summary>
        public ContactSensor[] CSS
        {
            get;
        }
        /// <summary>
        /// Get the instanceof the M12-based analog detector.
        /// </summary>
        public ICollectionView InternalPowerMeter
        {
            get
            {
                return CollectionViewSource.GetDefaultView(listInternalPowerMeter);
            }
        }

        /// <summary>
        /// Get the instance collection of the Motion Controller Class
        /// </summary>
        public Dictionary<Guid, IMotionController> PhysicalMotionControllerCollection
        {
            get;
        }

        /// <summary>
        /// Get the list of the overall logical axes defined in the system setting file.
        /// this list enable users to operate each axis independently without knowing which physical motion controller it belongs to
        /// </summary>
        public ICollectionView LogicalAxisCollection
        {
            get
            {
                return CollectionViewSource.GetDefaultView(listLogicalAxis);
            }
        }

        /// <summary>
        /// Get the collection contains the logical axes those belong to the logical aligner
        /// </summary>
        public ICollectionView LogicalAxisInAlignerCollection
        {
            get
            {
                return CollectionViewSource.GetDefaultView(listLogicalAxisInAligner);
            }
        }

        /// <summary>
        /// Get the collection of the logical motion components, this property should be used to generate the motion control panel for each aligner
        /// </summary>
        public ICollectionView LogicalMotionComponentCollection
        {
            get
            {
                return CollectionViewSource.GetDefaultView(listLogicalMotionComponent);
            }
        }

        /// <summary>
        /// Get the logical motion components which is marked as logical aligner
        /// </summary>
        public ICollectionView LogicalAlignerCollection
        {
            get
            {
                return CollectionViewSource.GetDefaultView(listLogicalAligner);
            }
        }

        /// <summary>
        /// Get the view of the collection of the instruments defined in the config file.
        /// </summary>
        public ICollectionView CollectionViewDefinedInstruments
        {
            get
            {
                return CollectionViewSource.GetDefaultView(listDefinedInstrument);
            }
        }

        /// <summary>
        /// Get the view of the collection of the instruments of those initialized successfully.
        /// </summary>
        public ICollectionView MeasurementInstrumentCollection
        {
            get
            {
                return CollectionViewSource.GetDefaultView(listAvaliableInstrument);
            }
        }

        /// <summary>
        /// Set or get the last message.
        /// </summary>
        public MessageItem LastMessage
        {
            private set
            {
                lastMessage = value;
            }
            get
            {
                return lastMessage;
            }
        }

        /// <summary>
        /// Get the collection of messages.
        /// </summary>
        public MessageHelper MessageCollection { get; } = new MessageHelper();

        #endregion Properties

        #region Alignment Handlers

        public IAlignmentHandler SpiralScanHandler { get; }

        public IAlignmentHandler SnakeRouteScanHandler { get; }

        public IAlignmentHandler FastRotatingScanHandler { get; }

        public IAlignmentHandler FastNDHandler { get; }

        public IAlignmentHandler FastCentralAlignHandler { get; }

        public IAlignmentHandler ProfileNDHandler { get; }
        
        public IAlignmentHandler RotatingScanHandler { get; }

        public IAlignmentHandler CentralAlignHandler { get; }

        #endregion Alignment Function Args

        #region Background tasks

        /// <summary>
        /// Create the task to read optical power from M12.
        /// </summary>
        /// <returns></returns>
        private Task CreateM12BackgroundTask(IrixiM12 Controller, ADCChannels ADCEnabled)
        {
            IProgress<double[]> progressANInput
                = new Progress<double[]>();

            ((Progress<double[]>)progressANInput).ProgressChanged += (s, e) =>
            {
                if (e != null)
                {
                    int adcChUsed = 0;
                    var chDefined = Enum.GetValues(typeof(ADCChannels));

                    foreach (var item in chDefined)
                    {
                        if (ADCEnabled.HasFlag((ADCChannels)item))
                        {
                            Controller.UpdateAnalogValue((ADCChannels)item, e[adcChUsed]);
                            adcChUsed++;
                        }
                    }
                }
                else
                {

                }
            };

            IProgress<DigitalInputStatus> progressDINChanged
                = new Progress<DigitalInputStatus>();

            ((Progress<DigitalInputStatus>)progressDINChanged).ProgressChanged += (s, e) =>
            {
                if (e != null)
                {
                    Controller.UpdateDINStatus(e);
                }
                else
                {

                }
            };

            return new Task(() =>
            {
                while (true)
                {
                    if (Controller == null || Controller.IsInitialized == false)
                        break;


                    // read analog input value.
                    if (ADCEnabled > 0)
                    {
                        try
                        {
                            var r = Controller.ReadAN(ADCEnabled);
                            progressANInput.Report(r);
                        }
                        catch
                        {
                            progressANInput.Report(null);
                        }
                    }

                    // read digital input status
                    try
                    {
                        var s = Controller.ReadDIN();
                        progressDINChanged.Report(s);
                    }
                    catch (Exception ex)
                    {
                        progressDINChanged.Report(null);
                    }
                }

                if (bgTaskPTS.IsPaused)
                    bgTaskPTS.Token.WaitWhilePausedAsync().Wait();

                Thread.Sleep(100);

                if (bgTaskPTS.IsPaused)
                    bgTaskPTS.Token.WaitWhilePausedAsync().Wait();
            });
        }

        /// <summary>
        /// Start all background tasks.
        /// </summary>
        private void StartBackgroundTasks()
        {
           foreach (var t in bgTaskList)
                t.Start();
        }

        /// <summary>
        /// Stop all background tasks.
        /// </summary>
        private void PauseBackgroundTasks()
        {
            bgTaskPTS.IsPaused = true;
        }

        private void ResumeBackgroundTasks()
        {
            bgTaskPTS.IsPaused = false;
        }
        
        #endregion

        #region Private Methods
        
        private void SetSystemState(SystemState State)
        {
            lock (lockSystemStatus)
            {
                this.State = State;
            }
        }

        private SystemState GetSystemState()
        {
            lock (lockSystemStatus)
            {
                return this.State;
            }
        }

        /// <summary>
        /// Indicates which components should be stoped if #STOP button clicked
        /// </summary>
        /// <param name="BusyStaff"></param>
        private void RegisterBusyComponent(IStoppable BusyStaff)
        {
            if (!BusyComponents.Contains(BusyStaff))
                BusyComponents.Add(BusyStaff);
        }

        /// <summary>
        /// Remove the busy component
        /// </summary>
        /// <param name="BusyStaff"></param>
        private void UnregisterBusyComponent(IStoppable BusyStaff)
        {
            if (BusyComponents.Contains(BusyStaff))
                BusyComponents.Remove(BusyStaff);
        }

        /// <summary>
        /// Register the physical axis to the physical instance property of the logical axis
        /// </summary>
        /// <param name="ParentAligner">which logical aligner belongs to</param>
        /// <param name="LAxis"></param>
        /// <returns></returns>
        private IAxis FindPhysicalAxisByLogicalAxisConfig(LogicalAxis LAxis)
        {
            // find the physical motion controller by the device class
            if (this.PhysicalMotionControllerCollection.ContainsKey(LAxis.Config.DeviceClass))
            {
                // find the axis in the specified controller by the axis name
                // and bind the physical axis to the logical axis
                var phyAxis = this.PhysicalMotionControllerCollection[LAxis.Config.DeviceClass].GetAxisByName(LAxis.Config.AxisName);

                if (phyAxis == null) // if the physical axis was not found
                {
                    return null;
                }
                else
                {
                    return phyAxis;
                }
            }
            else // the controller with the specified DevClass does not exist
            {
                return null;
            }
        }

        /// <summary>
        /// Start running user program
        /// </summary>
        private void Start()
        {
            //TODO here we should judge whether the auto-program is paused or not
            // if the auto-program is not worked and an auto-program has been selected, run it;
            // otherwise, continue to run the last paused auto-program
        }

        /// <summary>
        /// Stop the moving axes or stop running the user program
        /// </summary>
        public void Stop()
        {
            List<Task> tasks = new List<Task>();

            foreach (var item in BusyComponents)
            {
                tasks.Add(Task.Run(() =>
                {
                    item.Stop();
                }));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ex)
            {
                SetMessage(MessageType.Error, "Error(s) occurred while stopping objects: ");
                foreach (var item in ex.InnerExceptions)
                {
                    SetMessage(MessageType.Error, $"*{item.Source}* >>> {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Start a specified alignment process asynchronously, all alignment process will be started by this common function
        /// </summary>
        /// <param name="Handler"></param>
        public async void StartToAutoAlign(IAlignmentHandler Handler)
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);

                SetMessage(MessageType.Normal, $"Start running {Handler}...");

                // stop the background tasks.
                PauseBackgroundTasks();
                Thread.Sleep(100);

                // to calculate time costs
                DateTime alignStarts = DateTime.Now;

                // add alignement class to busy components list
                BusyComponents.Add(Handler);

                try
                {
                    if (Handler.Args == null)
                    {
                        SetMessage(MessageType.Error, $"{Handler} Error, the argument can not be null.");

                        PostErrorMessage(this.LastMessage.Message);
                    }
                    else
                    {
                        // validate the parameters
                        Handler.Args.Validate();

                        // pause the auto-fetching process of instrument
                        //AlignHandler.PauseInstruments();

                        // run actual alignment process
                        await Task.Run(() =>
                        {
                            Handler.Start();
                        });
                    }
                }
                catch (ADCSamplingPointMissException)
                {
                    SetMessage(MessageType.Warning, $"{Handler} warning, some sampling points were missed, try to reduce the speed or increase the value of the sampling interval.");
                }
                catch (Exception ex)
                {
                    SetMessage(MessageType.Error, $"{Handler} error, {ex.Message}");
                    PostErrorMessage(this.LastMessage.Message);
                }
                //finally
                //{
                //    try
                //    {
                //        AlignHandler.ResumeInstruments();
                //    }
                //    catch (Exception ex)
                //    {
                //        //LastMessage = new MessageItem(MessageType.Error, string.Format("Unable to resume auto-fetching process of {0}, {1}", AlignHandler.Args.Instrument, ex.Message));
                //    }
                //}

                SetMessage(MessageType.Normal, $"{Handler} complete, costs {(DateTime.Now - alignStarts).TotalSeconds}s");

                BusyComponents.Remove(Handler);

                // start background tasks.
                ResumeBackgroundTasks();

                SetSystemState(SystemState.IDLE);
            }
        }

        /// <summary>
        /// Post the error message to the UI
        /// </summary>
        /// <param name="Message"></param>
        private void PostErrorMessage(string Message)
        {
            Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>(
                        this,
                        Message,
                        "ERROR"));
        }

        /// <summary>
        /// Output init messages
        /// </summary>
        /// <param name="Message"></param>
        private void PostInitMessage(string Message)
        {
            InitProgressChanged?.Invoke(this, Message);
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Initialize all devices in the system
        /// </summary>
        public async void Init()
        {
            List<Task<bool>> _tasks = new List<Task<bool>>();
            List<IEquipment> _equipments = new List<IEquipment>();

            SetSystemState(SystemState.BUSY);

            #region Initialize motion controllers

            PostInitMessage("Initializing motion controllers ...");

            // initialize all motion controllers simultaneously
            foreach (var controller in this.PhysicalMotionControllerCollection.Values)
            {
                if (controller.IsEnabled)
                {
                    _equipments.Add(controller);
                    _tasks.Add(Task.Run(() =>
                    {
                        return controller.Init();
                    }));

                    SetMessage(MessageType.Normal, $"{controller} Initializing ...");

                    // prevent UI from halt
                    await Task.Delay(10);
                }
            }

            // wait until all axes are initialized
            // once a task is done, display the message on the screen
            while (_tasks.Count > 0)
            {
                Task<bool> t = await Task.WhenAny(_tasks);
                int id = _tasks.IndexOf(t);
                var controller = _equipments[id];

                if (t.IsFaulted)
                {
                    if (t.Exception != null)
                    {
                        foreach (var ex in t.Exception.Flatten().InnerExceptions)
                        {
                            SetMessage(MessageType.Error, $"{controller} Initialization is failed, {ex.Message}");
                        }
                    }
                    else
                    {
                        SetMessage(MessageType.Error, $"{controller} Initialization is failed, unable to get the inner exceptions.");
                    }
                }
                else if (t.Result)
                    SetMessage(MessageType.Good, $"{controller} Initialization is completed.");
                else
                    SetMessage(MessageType.Error, $"{controller} Initialization is failed, {controller.LastError}");

                _tasks.RemoveAt(id);
                _equipments.RemoveAt(id);
            }

            #endregion Initialize motion controllers

            #region Clear the task conllections to inititalize the other equipments

            _tasks.Clear();
            _equipments.Clear();

            #endregion Clear the task conllections to inititalize the other equipments

            /*
             * The following process is initializing the equipments that are based on
             * the motion controller.
             *
             * For example, in practice, the cylinder controller is attached to the Irixi motion controllers,
             * if the corresponding motion controller is not available, the cylinder controller is not available.
             */

            #region Initialize the other equipments

            PostInitMessage("Initializing cylinder controller ...");

            // initialize the cylinder controller
            _tasks.Add(Task.Factory.StartNew(this.CylinderController.Init));
            _equipments.Add(this.CylinderController);
            SetMessage(MessageType.Normal, $"{this.CylinderController} Initializing ...");
            

            PostInitMessage("Initializing measurement instruments ...");

            // initizlize the measurement instruments defined in the config file
            // the instruments initialized successfully will be added to the collection #ActiveInstrumentCollection
            foreach (var device in listDefinedInstrument)
            {
                //_tasks.Add(Task.Factory.StartNew(instr.Init));
                _tasks.Add(Task.Factory.StartNew(device.Init));
                _equipments.Add(device);
                SetMessage(MessageType.Normal, $"{device} Initializing ...");
            }

            while (_tasks.Count > 0)
            {
                int idFinishedTask = 0;
                Task<bool> t = await Task.WhenAny(_tasks);
                idFinishedTask = _tasks.IndexOf(t);
                var equipment = _equipments[idFinishedTask];

                if (t.IsFaulted)
                {
                    if (t.Exception != null)
                    {
                        foreach (var ex in t.Exception.Flatten().InnerExceptions)
                        {
                            SetMessage(MessageType.Error, $"{equipment} Initialization is failed, {ex.Message}");
                        }
                    }
                    else
                    {
                        SetMessage(MessageType.Error, $"{equipment} Initialization is failed, unable to get the inner exceptions.");
                    }
                }
                else if (t.Result)
                {
                    SetMessage(MessageType.Good, $"{equipment} Initialization is completed.");

                    // add the instruments which are initialized successfully to the acitve collection
                    if (equipment is InstrumentBase)
                    {
                        if(equipment.Config.Caption != null && equipment.Config.Caption != "")
                            listAvaliableInstrument.Add((InstrumentBase)_equipments[idFinishedTask]);
                    }
                }
                else
                    SetMessage(MessageType.Error, $"{equipment} Initialization is failed, {equipment.LastError}");

                _tasks.RemoveAt(idFinishedTask);
                _equipments.RemoveAt(idFinishedTask);
            }

            //ActiveInstrumentCollection.DisableNotifications();

            #endregion Initialize the other equipments

            // Start background tasks.
            StartBackgroundTasks();

            SetSystemState(SystemState.IDLE);

            SetMessage(MessageType.Normal,
                $"The initialization has finished, costs {(DateTime.Now - initStartTime).TotalSeconds.ToString("F2")}s");
        }

        /// <summary>
        /// Move the specified axis with specified args
        /// </summary>
        /// <param name="Axis"></param>
        /// <param name="Args"></param>
        public async void MoveLogicalAxis(LogicalAxis Axis, AxisMoveArgs Args)
        {
            if (GetSystemState() != SystemState.BUSY)
            {
                SetSystemState(SystemState.BUSY);

                SetMessage(MessageType.Normal, $"{Axis} Move with argument {Args} ...");

                var ret = await Task.Run(() =>
                {
                    return Axis.PhysicalAxisInst.Move(Args.Mode, Args.Speed, Args.Distance);
                });

                if (ret == false)
                {
                    SetMessage(MessageType.Error, $"{Axis} Unable to move, {Axis.PhysicalAxisInst.LastError}");

                    PostErrorMessage(this.LastMessage.Message);
                }
                else
                {
                    SetMessage(MessageType.Normal,
                        $"{Axis}, the final position is " +
                        $"{Axis.PhysicalAxisInst.AbsPosition}/{Axis.PhysicalAxisInst.UnitHelper.AbsPosition}{Axis.PhysicalAxisInst.UnitHelper.Unit}");
                }

                SetSystemState(SystemState.IDLE);
            }
            else
            {
                SetMessage(MessageType.Warning, "System is busy");
            }
        }

        /// <summary>
        /// Cruise the specified axis.
        /// </summary>
        /// <param name="Axis"></param>
        /// <param name="Args"></param>
        public async void CruiseLogcialAxis(LogicalAxis Axis, AxisCruiseArgs Args)
        {
            var paxis = Axis.PhysicalAxisInst;

            if (GetSystemState() != SystemState.BUSY)
            {
                SetSystemState(SystemState.BUSY);

                SetMessage(MessageType.Normal, $"{Axis} Start to cruise ...");

                var ret = await Task.Run(() =>
                {
                    

                    // convert the speed selection to the numeric speed.
                    int speed = 0;
                    switch(Args.Speed)
                    {
                        case AxisCruiseArgs.CruiseSpeed.FAST:
                            speed = 100;
                            break;

                        case AxisCruiseArgs.CruiseSpeed.MID:
                            speed = 50;
                            break;

                        case AxisCruiseArgs.CruiseSpeed.SLOW:
                            speed = 10;
                            break;
                    }

                    // restrict the cruise speed to the value in the config file.
                    speed = paxis.MaxSpeed * speed / 100;
     
                    // start to move
                    bool opRet = false;
                    double distance = 0;
                    switch(Args.Direction)
                    {
                        case AxisCruiseArgs.CruiseDirection.CCW:
                            distance = paxis.UnitHelper.AbsPosition;
                            opRet = paxis.Move(MoveMode.REL, speed, -distance);
                            break;

                        case AxisCruiseArgs.CruiseDirection.CW:
                            distance = paxis.UnitHelper.ConvertStepsToDistance(paxis.SCWL) - paxis.UnitHelper.AbsPosition;
                            opRet = paxis.Move(MoveMode.REL, speed, distance);
                            break;
                    }

                    return opRet;
                });

                if (ret == false)
                {
                    SetMessage(MessageType.Error, $"{Axis} Unable to cruise, {paxis.LastError}");

                    PostErrorMessage(this.LastMessage.Message);
                }
                else
                {
                    SetMessage(MessageType.Normal,
                        $"{Axis} The final position is " +
                        $"{paxis.AbsPosition}/{paxis.UnitHelper.AbsPosition}{paxis.UnitHelper.Unit}");
                }

                SetSystemState(SystemState.IDLE);
            }
            else
            {
                SetMessage(MessageType.Warning, "System is busy");
            }
        }

        /// <summary>
        /// Move a set of logical axes with the specified order
        /// </summary>
        /// <param name="AxesGroup"></param>
        public async void MassMoveLogicalAxis(MassMoveArgs Args)
        {
            /*
             *  Operation Sequence
             *  1. find logical motion component
             *  2. check logical axis
             *  3. get move sequence
             *  4. move
             */

            if (GetSystemState() != SystemState.BUSY)
            {
                SetSystemState(SystemState.BUSY);

                #region Find the LMC by the hash string

                LogicalMotionComponent lmc;
                try
                {
                    lmc = listLogicalMotionComponent.FindItemByHashString(Args.LogicalMotionComponent);
                }
                catch (Exception ex)
                {
                    SetMessage(MessageType.Error, $"Unable to run the mass-move process, {ex}");
                    return;
                }

                #endregion Find the LMC by the hash string

                #region Validate arguments and arrange the logical to ready to move

                List<LogicalAxis> axesToMove = new List<LogicalAxis>();

                if (Args.Count != lmc.Count)
                {
                    SetMessage(MessageType.Error, $"The axis count is different between argument and LMC.");
                    return;
                }
                else
                {
                    foreach (var perArg in Args)
                    {
                        LogicalAxis la;
                        try
                        {
                            la = lmc.FindAxisByHashString(perArg.LogicalAxisHashString);
                            if (perArg.IsMoveable)
                            {
                                // Only the moveable axis will be added
                                la.MoveArgsTemp = perArg.Clone() as AxisMoveArgs;
                                axesToMove.Add(la);
                            }
                        }
                        catch (Exception ex)
                        {
                            SetMessage(MessageType.Error, $"Unable to run the mass-move process, {ex}");
                            return;
                        }
                    }
                }

                #endregion Validate arguments and arrange the logical to ready to move

                #region Get move order

                var moveOrder = Args.GetDistinctMoveOrder();
                if (moveOrder == null)
                {
                    SetMessage(MessageType.Error, $"Unable to get the list of the move order");
                    return;
                }

                #endregion Get move order

                // generate a list which contains the movement tasks
                // this is used by the Task.WhenAll() function
                List<Task<bool>> movingTasks = new List<Task<bool>>();
                List<LogicalAxis> movingAxes = new List<LogicalAxis>();

                SetMessage(MessageType.Normal, "Mass-Move is running ...");

                foreach (var order in moveOrder)
                {
                    // clear the previous tasks
                    movingTasks.Clear();
                    movingAxes.Clear();

                    var ready2move = axesToMove.Where(a => a.MoveArgsTemp.MoveOrder == order).Select(b => b).ToArray();
                    if (ready2move.Any())
                    {
                        foreach (var la in ready2move)
                        {
                            SetMessage(MessageType.Normal, $"Start to move axis {la} ...");

                            var t = new Task<bool>(() =>
                            {
                                return la.PhysicalAxisInst.Move(
                                    la.MoveArgsTemp.Mode,
                                    la.MoveArgsTemp.Speed,
                                    la.MoveArgsTemp.Distance);
                            });
                            t.Start();
                            movingTasks.Add(t);
                            movingAxes.Add(la);
                        }

                        // if no axes to be moved, move to the next loop
                        if (movingTasks.Count > 0)
                        {
                            while (movingTasks.Count > 0)
                            {
                                // wait until all the axes are moved
                                Task<bool> t = await Task.WhenAny(movingTasks);
                                int id = movingTasks.IndexOf(t);

                                if (t.Result)
                                    SetMessage(MessageType.Normal, $"{movingAxes[id]} has moved to the position.");
                                else
                                    SetMessage(MessageType.Error, $"{movingAxes[id]} was moved incorrectly, {movingAxes[id].PhysicalAxisInst.LastError}");

                                movingTasks.RemoveAt(id);
                                movingAxes.RemoveAt(id);
                            }
                        }
                    }
                }
            }

            SetMessage(MessageType.Good, "Mass-Move Done.");

            SetSystemState(SystemState.IDLE);
        }

        /// <summary>
        /// Home the specified axis
        /// </summary>
        /// <param name="Axis"></param>
        public async void Home(IAxis Axis)
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                bool ret = await Task.Run<bool>(() => Axis.Home());
                SetSystemState(SystemState.IDLE);
            }
        }

        /// <summary>
        /// home all axes in system
        /// </summary>
        public async void MassHome()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                int _present_order = 0;
                int _homed_cnt = 0;
                int _total_axis = listLogicalAxis.Count;
                List<Task<bool>> _tasks = new List<Task<bool>>();
                List<LogicalAxis> _axis_homing = new List<LogicalAxis>();

                SetSystemState(SystemState.BUSY);

                // update UI immediately
                await Task.Delay(50);

                // Loop Home() function of each axis
                do
                {
                    //SetMessage(MessageType.Normal, "The present homing order is {0}", _present_order);

                    _axis_homing.Clear();
                    _tasks.Clear();
                    // find the axes which are to be homed in current stage
                    foreach (var axis in listLogicalAxis)
                    {
                        if (axis.Config.HomeOrder == _present_order)
                        {
                            SetMessage(MessageType.Normal, $"{axis} Start to home ...");

                            var t = new Task<bool>(() =>
                            {
                                return axis.PhysicalAxisInst.Home();
                            });
                            t.Start();
                            _tasks.Add(t);
                            _axis_homing.Add(axis);

                            // update UI immediately
                            await Task.Delay(10);
                        }
                    }

                    if (_tasks.Count <= 0)
                    {
                        //SetMessage(MessageType.Warning, "There are not axes to be homed in this order");
                    }
                    else
                    {
                        while (_tasks.Count > 0)
                        {
                            Task<bool> t = await Task.WhenAny(_tasks);
                            int id = _tasks.IndexOf(t);

                            if (t.Result)
                                SetMessage(MessageType.Good, $"{_axis_homing[id]} Home is completed.");
                            else
                                SetMessage(MessageType.Error, $"{_axis_homing[id]} Home is failed, {_axis_homing[id].PhysicalAxisInst.LastError}");

                            _tasks.RemoveAt(id);
                            _axis_homing.RemoveAt(id);

                            // save the sum of homed axes in order to check if all axes have been homed
                            _homed_cnt++;
                        }
                    }

                    // Move to next order
                    _present_order++;
                } while (_homed_cnt < _total_axis);

                SetMessage(MessageType.Good, "Mass Home is completed");

                SetSystemState(SystemState.IDLE);
            }
            else
            {
                SetMessage(MessageType.Warning, "System is busy");
            }
        }

        /// <summary>
        /// Toggle the move mode between ABS and REL
        /// </summary>
        /// <param name="Axis">The instance of physical axis</param>
        public void ToggleAxisMoveMode(IAxis Axis)
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                Axis.ToggleMoveMode();
                SetSystemState(SystemState.IDLE);
            }
        }

        /// <summary>
        /// Find the instrument by its hash string
        /// </summary>
        /// <param name="hashstring"></param>
        /// <returns></returns>
        public IInstrument FindInstrumentByHashString(string hashstring)
        {
            return listAvaliableInstrument.FindItemByHashString(hashstring);
        }

        /// <summary>
        /// Find the logical motion component by its hash string
        /// </summary>
        /// <param name="hashstring"></param>
        /// <returns></returns>
        public LogicalMotionComponent FindLogicalMotionComponentByHashString(string hashstring)
        {
            return listLogicalMotionComponent.FindItemByHashString(hashstring);
        }

        /// <summary>
        /// Find the logical axis by its hash string
        /// </summary>
        /// <param name="hashstring"></param>
        /// <returns></returns>
        public LogicalAxis FindLogicalAxisByHashString(string hashstring)

        {
            return listLogicalAxis.FindItemByHashString(hashstring);
        }
        
        public void FiberClampON()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                this.CylinderController.SetFiberClampState(DigitalIOStatus.ON);
                LogHelper.WriteLine("Fiber Clamp is opened", LogHelper.LogType.NORMAL);
                SetSystemState(SystemState.IDLE);
            }
        }

        public void FiberClampOFF()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                this.CylinderController.SetFiberClampState(DigitalIOStatus.OFF);

                LogHelper.WriteLine("Fiber Clamp is closed", LogHelper.LogType.NORMAL);
                SetSystemState(SystemState.IDLE);
            }
        }

        public void ToggleFiberClampState()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                if (this.CylinderController.FiberClampState == DigitalIOStatus.OFF)
                {
                    this.CylinderController.SetFiberClampState(DigitalIOStatus.ON);
                    LogHelper.WriteLine("Fiber Clamp is opened", LogHelper.LogType.NORMAL);
                }
                else
                {
                    this.CylinderController.SetFiberClampState(DigitalIOStatus.OFF);
                    LogHelper.WriteLine("Fiber Clamp is closed", LogHelper.LogType.NORMAL);
                }
                SetSystemState(SystemState.IDLE);
            }
        }

        public void LensVacuumON()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                this.CylinderController.SetLensVacuumState(DigitalIOStatus.ON);
                LogHelper.WriteLine("Lens Vacuum is opened", LogHelper.LogType.NORMAL);
                SetSystemState(SystemState.IDLE);
            }
        }

        public void LensVacuumOFF()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                this.CylinderController.SetLensVacuumState(DigitalIOStatus.OFF);
                LogHelper.WriteLine("Lens Vacuum is closed", LogHelper.LogType.NORMAL);
                SetSystemState(SystemState.IDLE);
            }
        }

        public void ToggleLensVacuumState()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                if (this.CylinderController.LensVacuumState == DigitalIOStatus.OFF)
                {
                    this.CylinderController.SetLensVacuumState(DigitalIOStatus.ON);
                    LogHelper.WriteLine("Lens Vacuum is opened", LogHelper.LogType.NORMAL);
                }
                else
                {
                    this.CylinderController.SetLensVacuumState(DigitalIOStatus.OFF);
                    LogHelper.WriteLine("Lens Vacuum is closed", LogHelper.LogType.NORMAL);
                }
                SetSystemState(SystemState.IDLE);
            }
        }

        public void PlcVacuumON()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                this.CylinderController.SetPlcVacuumState(DigitalIOStatus.ON);
                LogHelper.WriteLine("PLC Vacuum is opened", LogHelper.LogType.NORMAL);
                SetSystemState(SystemState.IDLE);
            }
        }

        public void PlcVacuumOFF()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                this.CylinderController.SetPlcVacuumState(DigitalIOStatus.OFF);
                LogHelper.WriteLine("PLC Vacuum is closed", LogHelper.LogType.NORMAL);
                SetSystemState(SystemState.IDLE);
            }
        }

        public void TogglePlcVacuumState()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                if (this.CylinderController.PlcVacuumState == DigitalIOStatus.OFF)
                {
                    this.CylinderController.SetPlcVacuumState(DigitalIOStatus.ON);
                    LogHelper.WriteLine("PLC Vacuum is opened", LogHelper.LogType.NORMAL);
                }
                else
                {
                    this.CylinderController.SetPlcVacuumState(DigitalIOStatus.OFF);
                    LogHelper.WriteLine("PLC Vacuum is closed", LogHelper.LogType.NORMAL);
                }
                SetSystemState(SystemState.IDLE);
            }
        }

        public void PodVacuumON()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                this.CylinderController.SetPodVacuumState(DigitalIOStatus.ON);
                LogHelper.WriteLine("POD Vacuum is opened", LogHelper.LogType.NORMAL);
                SetSystemState(SystemState.IDLE);
            }
        }

        public void PodVacuumOFF()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                this.CylinderController.SetPodVacuumState(DigitalIOStatus.OFF);
                LogHelper.WriteLine("POD Vacuum is opened", LogHelper.LogType.NORMAL);
                SetSystemState(SystemState.IDLE);
            }
        }

        public void TogglePodVacuumState()
        {
            if (GetSystemState() == SystemState.IDLE)
            {
                SetSystemState(SystemState.BUSY);
                if (this.CylinderController.PodVacuumState == DigitalIOStatus.OFF)
                {
                    this.CylinderController.SetPodVacuumState(DigitalIOStatus.ON);
                    LogHelper.WriteLine("POD Vacuum is opened", LogHelper.LogType.NORMAL);
                }
                else
                {
                    this.CylinderController.SetPodVacuumState(DigitalIOStatus.OFF);
                    LogHelper.WriteLine("POD Vacuum is opened", LogHelper.LogType.NORMAL);
                }
                SetSystemState(SystemState.IDLE);
            }
        }

        public void SetMessage(MessageType MsgType, string Message)
        {
            var msg = new MessageItem(MsgType, Message);
            MessageCollection.Add(msg);
            this.lastMessage = msg;
        }

        public void ShowErrorMessageBox(string Message)
        {
            SetMessage(MessageType.Error, Message);
            PostErrorMessage(Message);
        }

        public void Dispose()
        {
            // dispose motion controllers
            foreach (var ctrl in this.PhysicalMotionControllerCollection)
            {
                ctrl.Value.Dispose();
            }

            // dispose keithley2400s
            foreach (var k2400 in listAvaliableInstrument)
            {
                k2400.Dispose();
            }
        }

        #endregion Public Methods

        #region Raise Property Changed Event

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        /// <param name="PropertyName"></param>
        protected void UpdateProperty<T>(ref T OldValue, T NewValue, [CallerMemberName]string PropertyName = "")
        {
            //if (object.Equals(OldValue, NewValue))  // To save resource, if the value is not changed, do not raise the notify event
            //    return;

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