using M12.Base;
using M12.Commands.Alignment;
using M12.Commands.Analog;
using M12.Commands.General;
using M12.Commands.IO;
using M12.Commands.Memory;
using M12.Commands.Motion;
using M12.Definitions;
using M12.Exceptions;
using M12.Excpections;
using M12.Interfaces;
using M12.Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace M12
{
    public class Controller : IDisposable
    {
        #region Definitions

        /// <summary>
        /// The timeout value of the read method in millisecond.
        /// </summary>
        const int DEFAULT_READ_TIMEOUT = 5000;

        /// <summary> 
        /// The timeout value of the wait method of the long-duration-operation in millisecond such as home, move.
        /// </summary>
        const int DEFAULT_WAIT_BUSY_TIMEOUT = 30000;

        /// <summary>
        /// The timeout value of the waiting of alignment process.
        /// </summary>
        const int DEFAULT_WAIT_ALIGNMENT_TIMEOUT = 60000;

        #endregion

        #region Public Delegate Definitions

        public event EventHandler<UnitState> OnUnitStateUpdated;

        #endregion

        #region Variables

        SerialPort port;

        readonly object lockController = new object();

        IProgress<UnitState> unitStateChangedProgress;

        #endregion

        #region Constructors

        public Controller(string PortName, int Baudrate)
        {
            port = new SerialPort(PortName, Baudrate, Parity.None, 8, StopBits.One);

            // unit state updated and raise the 
            unitStateChangedProgress = new Progress<UnitState>(state =>
            {
                OnUnitStateUpdated?.Invoke(this, state);
            });
        }

        #endregion

        #region Properties

        public bool IsOpened
        {
            get
            {
                return port.IsOpen;
            }
        }

        #endregion

        #region User APIs

        public SystemLastError GetLastError()
        {
            RxPackage package;

            lock (lockController)
            {
                Send(new CommandGetLastError());
                Read(out package, CancellationToken.None);
            }
            var err = new SystemLastError(package.Payload);
            return err;
        }

        /// <summary>
        /// Get the system information including maximum unit, firmware version.
        /// </summary>
        /// <returns></returns>
        public SystemInformation GetSystemInfo()
        {
            RxPackage package;

            lock (lockController)
            {
                Send(new CommandGetSystemInfo());
                Read(out package, CancellationToken.None);
            }

            var sys_info = new SystemInformation(package.Payload);
            return sys_info;
        }

        /// <summary>
        /// Get the state of the system including IsEmergencyButtonPressed, IsBusy, etc.
        /// </summary>
        /// <returns></returns>
        public SystemState GetSystemState()
        {
            RxPackage package;

            lock (lockController)
            {
                Send(new CommandGetSystemState());
                Read(out package, CancellationToken.None);
            }
            
            var state = new SystemState(package.Payload);
            return state;

        }

        /// <summary>
        /// Get the settings of the specified channel including Mode, IsFlipDir, etc.
        /// </summary>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        public UnitSettings GetUnitSettings(UnitID UnitID)
        {
            RxPackage package;

            lock (lockController)
            {
                Send(new CommandGetUnitSettings(UnitID));
                Read(out package, CancellationToken.None);
            }

            var settings = new UnitSettings(package.Payload);
            return settings;
        }

        /// <summary>
        /// Get the state of the specified channel including IsHomed, IsBusy, Error, Position, etc.
        /// </summary>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        public UnitState GetUnitState(UnitID UnitID)
        {
            RxPackage package;

            lock (lockController)
            {
                Send(new CommandGetUnitState(UnitID));
                Read(out package, CancellationToken.None);
            }

            var state = new UnitState(package.Payload);
            return state;
        }

        /// <summary>
        /// Set the general acceleration of the specified channel.
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="AccelerationSteps"></param>
        public void SetAccelerationSteps(UnitID UnitID, ushort AccelerationSteps)
        {
            if (AccelerationSteps <= GlobalDefinition.MAX_ACC_STEPS)
            {
                lock (lockController)
                {
                    Send(new CommandSetAccSteps(UnitID, AccelerationSteps));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("the acceleration is too large.");
            }
        }

        /// <summary>
        /// Set the mode of the specified channel.
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="Settings"></param>
        public void SetMode(UnitID UnitID, UnitSettings Mode)
        {
            if (Mode != null)
            {
                lock (lockController)
                {
                    Send(new CommandSetMode(UnitID, Mode));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("the settings object can not be null.");
            }
        }

        /// <summary>
        /// Enable or disable the specified CSS interrupt.
        /// NOTE: The CSS INT will be disabled automatically after it's triggered, so it should
        /// be enabled again if you want to it work the next time.
        /// </summary>
        /// <param name="Css"></param>
        /// <param name="IsEnabled"></param>
        public void SetCSSEnable(CSSCH Css, bool IsEnabled)
        {
            lock(lockController)
            {
                Send(new CommandSetCSSEnable(Css, IsEnabled));
            }
        }

        /// <summary>
        /// Set the threshold of the CSS interrupt.
        /// </summary>
        /// <param name="Css"></param>
        /// <param name="LVth">The voltage of the low threshold in mV</param>
        /// <param name="HVth">The voltage of the high threshold in mV</param>
        public void SetCSSThreshold(CSSCH Css, ushort LVth, ushort HVth)
        {
            lock (lockController)
            {
                Send(new CommandSetCSSThreshold(Css, LVth, HVth));
            }
        }

        /// <summary>
        /// Set the status of the specified digital output port.
        /// </summary>
        /// <param name="Channel">DOUT1 to DOUT8</param>
        /// <param name="Status">OFF:0; ON:1</param>
        public void SetDOUT(DigitalOutput Channel, DigitalIOStatus Status)
        {
            lock (lockController)
            {
                Send(new CommandSetDOUT(Channel, Status));
            }
        }

        /// <summary>
        /// Read the status of all the digital output ports.
        /// </summary>
        /// <returns></returns>
        public DigitalOutputStatus ReadDOUT()
        {
            RxPackage package;

            lock(lockController)
            {
                Send(new CommandReadDOUT());
                Read(out package, CancellationToken.None);
            }

            var status = new DigitalOutputStatus(package.Payload);
            return status;
        }

        /// <summary>
        /// Read the status of the specified digital output port.
        /// </summary>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public DigitalIOStatus ReadDOUT(DigitalOutput Channel)
        {
            var stat = ReadDOUT();
            return stat.Integrated[(int)Channel - 1];
        }

        /// <summary>
        /// Read the status of the digital input IOs.
        /// </summary>
        /// <returns></returns>
        public DigitalInputStatus ReadDIN()
        {
            RxPackage package;

            lock (lockController)
            {
                Send(new CommandReadDIN());
                Read(out package, CancellationToken.None);
            }

            var input = new DigitalInputStatus(package.Payload);
            return input;
        }

        /// <summary>
        /// Read the values of the specified channels of the inner ADC.
        /// </summary>
        /// <param name="ChannelEnabled">Concat multiple channels with `|` operator.</param>
        /// <returns></returns>
        public double[] ReadADC(ADCChannels ChannelEnabled)
        {
            RxPackage package;

            lock (lockController)
            {
                Send(new CommandReadADC(ChannelEnabled));
                Read(out package, CancellationToken.None);
            }

            ADCValues adc = new ADCValues(ChannelEnabled, package.Payload);

            // convert adc raw-data to mV.
            List<double> valConv = new List<double>();
            foreach (var v in adc.Values)
                valConv.Add(ConvertADCRawTomV(v));

            return valConv.ToArray();
        }

        /// <summary>
        /// Save the ENV of the specified Unit to the flash.
        /// </summary>
        /// <param name="UnitID"></param>
        public void SaveUnitENV(UnitID UnitID)
        {
            lock (lockController)
            {
                Send(new CommandSaveUnitENV(UnitID));
            }

            Thread.Sleep(100);
        }

        #region Motion Control

        /// <summary>
        /// Home the specified channel.
        /// </summary>
        /// <param name="UnitID"></param>
        public void Home(UnitID UnitID, ushort Acc = 500, byte LowSpeed = 5, byte HighSpeed = 5)
        {
            Errors err = Errors.ERR_NONE;

            // check arguments
            if (LowSpeed > 100)
                LowSpeed = 100;
            else if (LowSpeed == 0)
                LowSpeed = 1;

            if (HighSpeed > 100)
                HighSpeed = 100;
            else if (HighSpeed == 0)
                HighSpeed = 1;

            lock (lockController)
            {
                Send(new CommandHome(UnitID, Acc, LowSpeed, HighSpeed));
            }

            try
            {
                err = WaitByUnitState(UnitID, 500);
            }
            catch (TimeoutException ex)
            {
                lock (lockController)
                {
                    Send(new CommandStop(UnitID));
                }
                throw ex;
            }

            if (err != Errors.ERR_NONE)
                throw new UnitErrorException(UnitID, err);
        }

        /// <summary>
        /// Move the specified channel.
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="Steps"></param>
        /// <param name="Speed"></param>
        /// <param name="ct"></param>
        public void Move(UnitID UnitID, int Steps, byte Speed)
        {
            Errors err = Errors.ERR_NONE;

            // check arguments
            if (Speed > 100)
                Speed = 100;
            else if (Speed == 0)
                Speed = 1;

            lock (lockController)
            {
                Send(new CommandMove(UnitID, Steps, Speed));
            }

            try
            {
                err = WaitByUnitState(UnitID, 5);
            }
            catch(TimeoutException ex)
            {
                lock(lockController)
                {
                    Send(new CommandStop(UnitID));
                }
                throw ex;
            }

            if (err != Errors.ERR_NONE)
                throw new UnitErrorException(UnitID, err);
        }

        /// <summary>
        /// Move the specified channel and capture the ADC value.
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="Steps"></param>
        /// <param name="Speed"></param>
        /// <param name="TriggerInterval"></param>
        /// <param name="ct"></param>
        public void MoveTriggerADC(UnitID UnitID, int Steps, byte Speed, ushort TriggerInterval)
        {
            Errors err = Errors.ERR_NONE;

            // check arguments
            if (Speed > 100)
                Speed = 100;
            else if (Speed == 0)
                Speed = 1;

            lock (lockController)
            {
                Send(new CommandMoveTriggerADC(UnitID, Steps, Speed, TriggerInterval));
            }

            try
            {
                err = WaitByUnitState(UnitID, 5);
            }
            catch (TimeoutException ex)
            {
                lock (lockController)
                {
                    Send(new CommandStop(UnitID));
                }
                throw ex;
            }

            if (err != Errors.ERR_NONE)
                throw new UnitErrorException(UnitID, err);
        }

        /// <summary>
        /// Stop the moving channel.
        /// </summary>
        /// <param name="UnitID"></param>
        public void Stop(UnitID UnitID)
        {
            lock (lockController)
            {
                Send(new CommandStop(UnitID));
            }
        }

        #endregion

        #region Trigger Source Config

        /// <summary>
        /// Config the ADC Trigger.
        /// </summary>
        /// <param name="ChannelEnabled"></param>
        public void ConfigADCTrigger(ADCChannels ChannelEnabled)
        {
            lock (lockController)
            {
                Send(new CommandConfigADCTrigger(ChannelEnabled));
            }
        }

        #endregion

        #region Alignment

        public void StartFast1D(UnitID Unit, int Range, ushort Interval, byte Speed, ADCChannels AnalogCapture, out List<Point2D> ScanResults)
        {
            List<Point2D> fakeList = null;

            StartFast1D(Unit, Range, Interval, Speed, AnalogCapture, out ScanResults, 0, out fakeList);
        }

        /// <summary>
        /// Perform the fast-1D alignment.
        /// </summary>
        /// <param name="Unit"></param>
        /// <param name="Range"></param>
        /// <param name="Interval"></param>
        /// <param name="Speed"></param>
        /// <param name="AnalogCapture"></param>
        /// <param name="ScanResults"></param>
        public void StartFast1D(UnitID Unit, int Range, ushort Interval, byte Speed, ADCChannels AnalogCapture, out List<Point2D> ScanResults, ADCChannels AnalogCapture2, out List<Point2D> ScanResults2)
        {
            if(GlobalDefinition.NumberOfSetBits((int)AnalogCapture) != 1) 
                throw new ArgumentException($"specify ONLY one channel allowed for the analog capture.");

            if(GlobalDefinition.NumberOfSetBits((int)AnalogCapture2) > 1)
                throw new ArgumentException($"specify ONLY one channel allowed for the analog capture 2, or disable it.");

            ScanResults = new List<Point2D>();

            ScanResults2 = null;
            if(AnalogCapture2 != 0)
                ScanResults2 = new List<Point2D>();


            ConfigADCTrigger(AnalogCapture | AnalogCapture2);

            ClearMemory();

            MoveTriggerADC(Unit, Range, Speed, Interval);

            // read sampling points from the memory.
            var adcValues = ReadMemoryAll();

            int x = 0;
            int indexOfAdcValues = 0;

            while (true)
            {
                if (indexOfAdcValues > (adcValues.Count - 1))
                    ScanResults.Add(new Point2D(x, 0));
                else
                    ScanResults.Add(new Point2D(x, adcValues[indexOfAdcValues++]));

                if (AnalogCapture2 != 0)
                {
                    if (indexOfAdcValues > (adcValues.Count - 1))
                        ScanResults2.Add(new Point2D(x, 0));
                    else
                        ScanResults2.Add(new Point2D(x, adcValues[indexOfAdcValues++]));
                }

                x += Interval;

                if (Math.Abs(x) >= Math.Abs(Range))
                {
                    break;
                }
            }
            
            // ran too fast, some ADC value missed.
            if (ScanResults.Count * Interval < Range)
                throw new ADCSamplingPointMissException((int)Math.Ceiling((decimal)Range / Interval),  ScanResults.Count);
        }

        /// <summary>
        /// Perform the blind-search.
        /// </summary>
        /// <param name="HorizontalArgs">The arguments of the horizontal axis.</param>
        /// <param name="VerticalArgs">The arguments of the vertical axis.</param>
        /// <param name="AdcUsed">Note: only one ADC channel can be used to sample.</param>
        /// <param name="ScanResults">Return the intensity-to-position points.</param>
        public void StartBlindSearch(BlindSearchArgs HorizontalArgs, BlindSearchArgs VerticalArgs, ADCChannels AdcUsed, out List<Point3D> ScanResults)
        {
            //! The memory is cleared automatically, you don't have to clear it manually.

            // check argments.
            if(GlobalDefinition.NumberOfSetBits((int)AdcUsed) != 1)
                throw new ArgumentException($"specify ONLY one channel of the ADC to capture the analog signal.");

            if (HorizontalArgs.Gap < HorizontalArgs.Interval)
                throw new ArgumentException($"the trigger interval of {HorizontalArgs.Unit} shoud be less than the value of the gap.");

            if (VerticalArgs.Gap < VerticalArgs.Interval)
                throw new ArgumentException($"the trigger interval of {VerticalArgs.Unit} shoud be less than the value of the gap.");

            ScanResults = new List<Point3D>();

            ConfigADCTrigger(AdcUsed);

            lock (lockController)
            {
                Send(new CommandBlindSearch(HorizontalArgs, VerticalArgs));
            }
            
            var err = WaitBySystemState(200, 120000);

            if (err.Error != Errors.ERR_NONE)
                throw new SystemErrorException(err);
            else
            {

                //var values = new List<double>(new double[100000]);
                // read the sampling points from the memory.
                var adcValues = ReadMemoryAll();

                int indexOfAdcValues = 0;
                int cycle = 0;
                double x = 0, y = 0;
                BlindSearchArgs activeParam = null;
                int moveDirection = 1;

                // rebuild the relationship between the position and the intensity.
                while (true)
                {
                    var seq = cycle % 4;

                    switch (seq)
                    {
                        case 0:     // move horizontal axis (x) to positive direction (right)
                            activeParam = HorizontalArgs;
                            moveDirection = 1;
                            break;

                        case 1:
                            activeParam = VerticalArgs;
                            moveDirection = 1;
                            break;

                        case 2:
                            activeParam = HorizontalArgs;
                            moveDirection = -1;
                            break;

                        case 3:
                            activeParam = VerticalArgs;
                            moveDirection = -1;
                            break;
                    }

                    var steps = moveDirection * (activeParam.Gap * (cycle / 2 + 1));

                    if (Math.Abs(steps) > activeParam.Range)
                        break;
                    
                    // for the horizontal axis.
                    if(activeParam == HorizontalArgs)
                    {
                        var originPos = x;

                        while(true)
                        {
                            if(indexOfAdcValues > (adcValues.Count - 1))
                                ScanResults.Add(new Point3D(x, y, 0));
                            else
                                ScanResults.Add(new Point3D(x, y, adcValues[indexOfAdcValues++]));

                            x += moveDirection * activeParam.Interval;

                            if (Math.Abs(x - originPos) >= Math.Abs(steps))
                            {
                                x = originPos + steps;
                                break;
                            }
                        }
                    }
                    else if(activeParam == VerticalArgs)
                    {
                        var originPos = y;

                        while (true)
                        {
                            if (indexOfAdcValues > (adcValues.Count - 1))
                                ScanResults.Add(new Point3D(x, y, 0));
                            else
                                ScanResults.Add(new Point3D(x, y, adcValues[indexOfAdcValues++]));

                            y += moveDirection * activeParam.Interval;

                            if (Math.Abs(y - originPos) >= Math.Abs(steps))
                            {
                                y = originPos + steps;
                                break;
                            }
                        }
                    }
                    
                    cycle++;

                }

                // output debug data.
                StringBuilder sb = new StringBuilder();
                foreach(var point in ScanResults)
                {
                    sb.Append($"{point.X}\t{point.Y}\t{point.Z}\r\n");
                }

                if (ScanResults.Count > adcValues.Count)
                    throw new ADCSamplingPointMissException(ScanResults.Count, adcValues.Count);
            }
        }

        #endregion  

        #region Memory Operation

        /// <summary>
        /// Get the length of the valid memory.
        /// </summary>
        /// <returns></returns>
        public UInt32 GetMemoryLength()
        {
            RxPackage package;

            lock (lockController)
            {
                Send(new CommandGetMemoryLength());
                Read(out package, CancellationToken.None);
            }

            using (MemoryStream stream = new MemoryStream(package.Payload))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadUInt32();
                }
            }
        }

        

        /// <summary>
        /// Read ADC values from the memory.
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="LengthToRead"></param>
        /// <returns></returns>
        public List<double> ReadMemory(uint Offset, uint LengthToRead)
        {
           List<double> raw = new List<double>();

            RxPackage package;

            lock (lockController)
            {
                Send(new CommandReadMemory(Offset, LengthToRead));


                // continue to read all blocks.
                while (true)
                {
                    Read(out package, CancellationToken.None);

                    MemoryBlock block = new MemoryBlock(package.Payload);

                    if (block.Length == 0) // the last block has received.
                        break;
                    else
                    {
                        foreach(var val in block.Values)
                        {
                            raw.Add(ConvertADCRawTomV(val));
                        }
                    }

                }
            }

            return raw;
        }

        /// <summary>
        /// Read all ADC values.
        /// </summary>
        /// <returns></returns>
        public List<double> ReadMemoryAll()
        {
            var len = GetMemoryLength();
            return ReadMemory(0, len);
        }

        /// <summary>
        /// Free the memory.
        /// </summary>
        public void ClearMemory()
        {
            lock (lockController)
            {
                Send(new CommandClearMemory());
            }
        }

        #endregion

        #endregion

        #region Private Methods

        public void Open()
        {
            if (port.IsOpen == false)
                port.Open();
        }

        public void Close()
        {
            if (this.port != null && this.port.IsOpen)
                port.Close();
        }

        /// <summary>
        /// Send binary data to serial port.
        /// </summary>
        /// <param name="Command"></param>
        void Send(ICommand Command)
        {
            port.DiscardInBuffer();
            port.DiscardOutBuffer();

            var data = Command.ToArray();

            port.Write(data, 0, data.Length);
        }
        
        /// <summary>
        /// Read package from UART port synchronously with specified timeout value.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <param name="Package"></param>
        /// <returns></returns>
        public void Read(out RxPackage Package, CancellationToken cancellationToken, int Timeout = DEFAULT_READ_TIMEOUT)
        {
            Package = new RxPackage();

            DateTime start = DateTime.Now;
            while (true)
            {
                var span = DateTime.Now - start;
                if (span.TotalMilliseconds > Timeout)
                    throw new TimeoutException("timeout to read package from the serial port.");

                if (cancellationToken != CancellationToken.None && cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException("operation has cancelled.");

                // if one or more bytes are received, push to RxPackageParser.
                if (port.BytesToRead > 0)
                {
                    var data = (byte)port.ReadByte();
                    Package.AddData(data);
                    if (Package.IsPackageFound)
                        break;
                }
            }

            if (Package.IsPassCRC == false)
            {
                throw new Exception("incorrect CRC value.");
            }
        }

        /// <summary>
        /// Convert the raw-data of the ADC to mV.
        /// </summary>
        /// <param name="AdcVal"></param>
        /// <returns></returns>
        private double ConvertADCRawTomV(short AdcVal)
        {
            return AdcVal / 32768.0 * GlobalDefinition.ADCVref;
        }

        /// <summary>
        /// Waiting for accomplishment of the time-consuming operations by the state of the specified Unit.
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="ct"></param>
        /// <param name="LoopInterval"></param>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        public Errors WaitByUnitState(UnitID UnitID, int LoopInterval = 100, int Timeout = DEFAULT_WAIT_BUSY_TIMEOUT)
        {
            DateTime startTime = DateTime.Now;
            UnitState state = null;

            while(true)
            {
                state = GetUnitState(UnitID);

                var curr_time = DateTime.Now;
                
                // raise event after the unit state updated.
                unitStateChangedProgress.Report(state);
                
                if (state.IsBusy == false)
                    break;

                if ((curr_time - startTime).TotalMilliseconds > Timeout)
                    throw new TimeoutException("it's timeout to wait the long-duration-operation.");

                Thread.Sleep(LoopInterval);
                
            }

            return state.Error;
        }

        /// <summary>
        /// Waiting for accomplishment of the time-consuming operations by the state of the entire system.
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="LoopInterval"></param>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        public SystemLastError WaitBySystemState(int LoopInterval = 100, int Timeout = DEFAULT_WAIT_BUSY_TIMEOUT)
        {
            DateTime startTime = DateTime.Now;
            SystemState state = null;

            while (true)
            {
                state = GetSystemState();
                if (state.IsSystemBusy == false)
                    break;

                //if (ct != CancellationToken.None && ct.IsCancellationRequested)
                //    return new SystemLastError(UnitID.INVALID, Errors.ERR_OPERATION_CANCELLED);

                if ((DateTime.Now - startTime).TotalMilliseconds > Timeout)
                    return new SystemLastError(UnitID.INVALID, Errors.ERR_TIMEOUT);

                Thread.Sleep(LoopInterval);

            }

            var err = GetLastError();
            return err;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Controller() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion

        #region Events



        #endregion
    }
}
