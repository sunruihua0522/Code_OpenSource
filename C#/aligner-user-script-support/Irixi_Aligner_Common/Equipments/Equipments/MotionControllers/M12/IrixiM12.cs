using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Base;
using M12;
using M12.Base;
using M12.Commands.Alignment;
using M12.Definitions;
using M12.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12
{
    public class M12AnalogInputUpdatedArgs : EventArgs
    {
        public M12AnalogInputUpdatedArgs(ADCChannels Channel, double Value)
        {
            this.InputChannel = Channel;
            this.AnalogValue = Value;
        }

        public ADCChannels InputChannel { get; }
        public double AnalogValue { get; }
    }

    public class M12DINUpdatedArgs : EventArgs
    {
        public M12DINUpdatedArgs(DigitalInputStatus Status)
        {
            this.Status = Status;
        }

        public DigitalInputStatus Status { get; }
    }

    public class IrixiM12 : MotionControllerBase<IrixiM12Axis>
    {
        #region Variables

        Controller _m12;

        public event EventHandler<M12AnalogInputUpdatedArgs> OnAnalogInputValueUpdated;

        public event EventHandler<M12DINUpdatedArgs> OnDigitalInputUpdated;

        #endregion

        #region Constructors

        public IrixiM12(ConfigPhysicalMotionController Config) : base(Config)
        {
            _m12 = new Controller(Config.Port, Config.BaudRate);

            _m12.OnUnitStateUpdated += _m12_OnUnitStateUpdated;
        }

        #endregion

        #region Methods

        protected override bool InitProcess()
        {
            SystemInformation info;

            try
            {
                _m12.Open();
                info = _m12.GetSystemInfo();
            }
            catch (Exception ex)
            {
                LastError = $"Unable to connect the M12, {ex.Message}";
                return false;
            }

            for (int i = 0; i < this.Count; i++)
            {
                var phyAxis = this[i] as IrixiM12Axis;
                var index = (int)phyAxis.ID - 1;

                if (info.UnitFwInfo[index].FirmwareVersion.Major >= 1)
                {
                    try
                    {
                        var state = _m12.GetUnitState(phyAxis.ID);
                        var settings = _m12.GetUnitSettings(phyAxis.ID);

                        phyAxis.State = state;
                        phyAxis.Settings = settings;
                        phyAxis.Info = info.UnitFwInfo[index];

                        // get current state, if it's initialized, no home needed anymore.
                        phyAxis.IsHomed = state.IsHomed;
                        phyAxis.AbsPosition = state.AbsPosition;
                    }
                    catch (Exception ex)
                    {
                        phyAxis.LastError = $"Unable to get read the information from the controller, {ex.Message}";
                    }
                }
                else
                {
                    phyAxis.LastError = $"Unable to convert the name of the axis {phyAxis.AxisName} the unit ID.";
                }
            }

            return true;
        }

        protected override bool HomeProcess(IAxis Axis)
        {
            bool ret = false;
            var phyAxis = Axis as IrixiM12Axis;

            // lock the axis
            if (phyAxis.Lock())
            {
                try
                {
                    // start to home
                    _m12.Home(phyAxis.ID);

                    Thread.Sleep(500);

                    phyAxis.ClearRelPosition();

                    phyAxis.IsHomed = true;
                    ret = true;

                    
                }
                catch (Exception ex)
                {
                    phyAxis.LastError = $"{ex.Message}";
                    ret = false;
                }
                finally
                {
                    // release the axis
                    phyAxis.Unlock();
                }
            }
            else
            {
                phyAxis.LastError = "axis is busy";
                ret = false;
            }

            return ret;
        }

        protected override bool MoveProcess(IAxis Axis, MoveMode Mode, int Speed, int Steps)
        {
            bool ret = false;
            var axis = Axis as IrixiM12Axis;

            int stepToMove = 0;

            if (axis.IsHomed == false)
            {
                axis.LastError = "the axis is not homed";
                return false;
            }

            if (axis.Lock())
            {
                try
                {
                    // calculate how many steps to move to the target position, this is based on the 
                    // mode of moving. The RELATIVE STEPS are accepted by the M12.
                    if (Mode == MoveMode.ABS)
                    {
                        stepToMove = Math.Abs(Steps) - axis.AbsPosition;
                    }
                    else
                    {
                        stepToMove = Steps;
                    }

                    // Move the the target position
                    if (axis.CheckSoftLimitation(axis.AbsPosition + stepToMove))
                    {
                        try
                        {
                            // limit the speed to the maximum value defined in the config file.
                            Speed = Speed * axis.MaxSpeed / 100;
                            
                            _m12.Move(axis.ID, stepToMove, (byte)Speed);
                            ret = true;
                        }
                        catch (Exception ex)
                        {
                            if (ex is UnitErrorException && ((UnitErrorException)ex).Error == Errors.ERR_USER_STOPPED)
                            {
                                // ignore the `user stop` error.
                                ret = true;
                            }
                            else
                            {
                                axis.LastError = $"sdk reported error code {ex.Message}";
                                ret = false;
                            }
                        }
                    }
                    else
                    {
                        axis.LastError = "target position exceeds the limitation.";

                        ret = false;
                    }

                }
                catch (Exception ex)
                {
                    axis.LastError = ex.Message;
                    ret = false;
                }
            }
            else
            {
                axis.LastError = "the axis is locked.";
                ret = false;
            }

            return ret;
        }

        public override void Stop()
        {
            _m12.Stop(UnitID.U1);
            _m12.Stop(UnitID.U2);
            _m12.Stop(UnitID.U3);
            _m12.Stop(UnitID.U4);
            _m12.Stop(UnitID.U5);
            _m12.Stop(UnitID.U6);
            _m12.Stop(UnitID.U7);
            _m12.Stop(UnitID.U8);
            _m12.Stop(UnitID.U9);
            _m12.Stop(UnitID.U10);
            _m12.Stop(UnitID.U11);
            _m12.Stop(UnitID.U12);

        }

        public void SetDOUT(int Channel, DigitalIOStatus Status)
        {
            _m12.SetDOUT((DigitalOutput)(((DigitalOutput)DigitalOutput.DOUT1) + Channel), Status);
        }

        public DigitalOutputStatus ReadDOUT()
        {
            return _m12.ReadDOUT();
        }

        public DigitalIOStatus ReadDOUT(int Channel)
        {
            return _m12.ReadDOUT((DigitalOutput)((int)DigitalOutput.DOUT1 + Channel));
        }

        /// <summary>
        /// Read digital input status.
        /// </summary>
        /// <returns></returns>
        public DigitalInputStatus ReadDIN()
        {
            return _m12.ReadDIN();
        }

        /// <summary>
        /// Call the registered actions to set the digital output status to the corresponding devices.
        /// </summary>
        /// <param name="Status"></param>
        public void UpdateDINStatus(DigitalInputStatus Status)
        {
            OnDigitalInputUpdated?.Invoke(this, new M12DINUpdatedArgs(Status));
        }

        /// <summary>
        /// Read the analog input value in mV.
        /// <para>NOTE AN1 and AN2 are used to connect to the contact sensors.</para>
        /// <para>AN3 and AN4 used to the internal PowerMeters.</para>
        /// </summary>
        /// <returns></returns>
        public double[] ReadAN(ADCChannels Ch)
        {
            return _m12.ReadADC(Ch);
        }

        /// <summary>
        /// Call the registered actions to set the analog input value to the corresponding devices.
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="Voltage"></param>
        public void UpdateAnalogValue(ADCChannels Channel, double Voltage)
        {
            OnAnalogInputValueUpdated?.Invoke(this, new M12AnalogInputUpdatedArgs(Channel, Voltage));
        }

        /// <summary>
        /// Perform Fast-1D alignment with single-adc-channel-capture.
        /// </summary>
        /// <param name="Axis"></param>
        /// <param name="Range"></param>
        /// <param name="Interval"></param>
        /// <param name="Speed"></param>
        /// <param name="AnalogCapture"></param>
        /// <param name="ScanResult"></param>
        public void StartFast1D(IAxis Axis, double Range, double Interval, int Speed, ADCChannels AnalogCapture, out List<Point2D> ScanResult)
        {
            StartFast1D(Axis, Range, Interval, Speed, AnalogCapture, out ScanResult, 0, out List<Point2D> ss);
        }

        /// <summary>
        /// Perform Fast-1D alignment with dual-adc-channel-capture.
        /// </summary>
        /// <param name="Axis"></param>
        /// <param name="Range"></param>
        /// <param name="Interval"></param>
        /// <param name="Speed"></param>
        /// <param name="AnalogCapture"></param>
        /// <param name="ScanResult"></param>
        public void StartFast1D(IAxis Axis, double Range, double Interval, int Speed, ADCChannels AnalogCapture, out List<Point2D> ScanResult, ADCChannels AnalogCapture2, out List<Point2D> ScanResult2)
        {
            ScanResult = null;
            ScanResult2 = null;

            if(Axis.GetType() != typeof(IrixiM12Axis))
                throw new Exception("The axis you specified does not support the fast alignment function.");

            var unit = ((IrixiM12Axis)Axis).ID;

            var rangeInStep = Axis.UnitHelper.ConvertDistanceToSteps(Range);
            var intervalInStep = (ushort)Axis.UnitHelper.ConvertDistanceToSteps(Interval);

            // limit the speed to the maximum value defined in the config file.
            var cSpeed = (byte)(Speed * Axis.MaxSpeed / 100);
            if (cSpeed == 0)
                cSpeed = 1;
            else if (cSpeed > 100)
                cSpeed = 100;

            _m12.StartFast1D(unit, rangeInStep, intervalInStep, cSpeed, AnalogCapture, out ScanResult, AnalogCapture2, out ScanResult2);
        }

        /// <summary>
        /// Perform blind-search alignment.
        /// </summary>
        /// <param name="HAxis"></param>
        /// <param name="VAxis"></param>
        /// <param name="Range"></param>
        /// <param name="Gap"></param>
        /// <param name="Interval"></param>
        /// <param name="Speed"></param>
        /// <param name="AnalogCapture"></param>
        /// <param name="ScanResults"></param>
        public void StartBlindSearch(IAxis HAxis, IAxis VAxis, double Range, double Gap, double Interval, int Speed, ADCChannels AnalogCapture, out List<Point3D> ScanResults)
        {
            ScanResults = null;

            if (HAxis.GetType() != typeof(IrixiM12Axis) || VAxis.GetType() != typeof(IrixiM12Axis))
                throw new Exception("The axis you specified does not support the Blind-Search function.");
            
            var hUnit = ((IrixiM12Axis)HAxis).ID;
            var vUnit = ((IrixiM12Axis)VAxis).ID;

            // limit the speed to the maximum value defined in the config file.
            var horiSpeed = Speed * HAxis.MaxSpeed / 100;
            if (horiSpeed == 0)
                horiSpeed = 1;
            else if (horiSpeed > 100)
                horiSpeed = 100;

            // limit the speed to the maximum value defined in the config file.
            var vertSpeed = Speed * VAxis.MaxSpeed / 100;
            if (vertSpeed == 0)
                vertSpeed = 1;
            else if (vertSpeed > 100)
                vertSpeed = 100;

            BlindSearchArgs horiArgs = new BlindSearchArgs(
                hUnit,
                (uint)HAxis.UnitHelper.ConvertDistanceToSteps(Range),
                (uint)HAxis.UnitHelper.ConvertDistanceToSteps(Gap),
                (ushort)HAxis.UnitHelper.ConvertDistanceToSteps(Interval),
                (byte)horiSpeed);

            BlindSearchArgs veriArgs = new BlindSearchArgs(
                vUnit,
                (uint)VAxis.UnitHelper.ConvertDistanceToSteps(Range),
                (uint)VAxis.UnitHelper.ConvertDistanceToSteps(Gap),
                (ushort)VAxis.UnitHelper.ConvertDistanceToSteps(Interval),
                (byte)vertSpeed);


            _m12.StartBlindSearch(horiArgs, veriArgs, AnalogCapture, out ScanResults);
        }

        #endregion

        #region Events

        private void _m12_OnUnitStateUpdated(object sender, UnitState e)
        {
            var axis = this.GetAxisByName(((int)e.UnitID).ToString());
            axis.AbsPosition = e.AbsPosition;

            Debug.WriteLine($"{axis} ABS position updated to {e.AbsPosition}.");

        }

        #endregion

        #region Override Methods

        public override void Dispose()
        {
            if (_m12 != null && _m12.IsOpened)
                _m12.Close();
        }

        #endregion

    }
}
