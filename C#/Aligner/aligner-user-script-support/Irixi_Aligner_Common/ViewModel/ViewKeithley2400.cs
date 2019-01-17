using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Irixi_Aligner_Common.Equipments.Instruments;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Irixi_Aligner_Common.ViewModel
{
    public class ViewKeithley2400 : ViewModelBase
    {

        public ViewKeithley2400(Keithley2400 DeviceInstance)
        {
            this.K2400 = DeviceInstance;
        }

        #region Properties

        /// <summary>
        /// Get the instance of K2400
        /// </summary>
        public Keithley2400 K2400
        {
            private set;
            get;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Get the command to switch on/off the output
        /// </summary>
        public RelayCommand SetOutputEnabled
        {
            get
            {
                return new RelayCommand(() =>
                {
                    try
                    {
                        if (K2400.IsOutputEnabled == false)
                        {
                            K2400.SetOutputState(true);

                            // start fetch loop
                            K2400.StartAutoFetching();
                        }
                        else
                        {
                            // start fetch loop
                            K2400.StopAutoFetching();

                            K2400.SetOutputState(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>(string.Format("error occurred while setting output state, {0}", ex.Message), "ERROR"));
                    }
                });
            }
        }

        /// <summary>
        /// Switch In/Out terminal
        /// </summary>
        public RelayCommand<Keithley2400.EnumInOutTerminal> SetInOutTerminal
        {
            get
            {
                return new RelayCommand<Keithley2400.EnumInOutTerminal>(term =>
                {
                    try
                    {
                        K2400.SetInOutTerminal(term);
                    }
                    catch(Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>(string.Format("unable to switch In/Out terminal, {0}", ex.Message), "ERROR"));
                    }
                });
            }
        }

        /// <summary>
        /// Get the command to switch the mode to voltage source
        /// </summary>
        public RelayCommand SetToVoltageSource
        {
            get
            {
                return new RelayCommand(() =>
                {
                    try
                    {
                        K2400.SetToVoltageSource();
                    }
                    catch(Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>(string.Format("unable to switch to voltage source, {0}", ex.Message), "ERROR"));
                    }
                });
            }
        }

        /// <summary>
        /// Get the command to switch the mode to current source
        /// </summary>
        public RelayCommand SetToCurrentSource
        {
            get
            {
                return new RelayCommand(() =>
                {
                    try
                    {
                        K2400.SetToCurrentSource();
                    }
                    catch(Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>(string.Format("unable to switch to current source, {0}", ex.Message), "ERROR"));
                    }
                });
            }
        }

        /// <summary>
        /// Set compliance per current source mode
        /// </summary>
        public RelayCommand<double> SetCompliance
        {
            get
            {
                return new RelayCommand<double>(target =>
                {
                    try
                    {
                        if (K2400.SourceMode == Keithley2400.EnumSourceMode.CURR)
                        {
                            K2400.SetComplianceVoltage(Keithley2400.EnumComplianceLIMIT.REAL, target);
                        }
                        else if (K2400.SourceMode == Keithley2400.EnumSourceMode.VOLT)
                        {
                            K2400.SetComplianceCurrent(Keithley2400.EnumComplianceLIMIT.REAL, target);
                        }
                    }
                    catch(Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>(string.Format("unable to set compliance, {0}", ex.Message), "ERROR"));
                    }
                });
            }
        }
        
        public RelayCommand<Keithley2400.EnumMeasRangeAmps> SetCurrentMeasurementRange
        {
            get
            {
                return new RelayCommand<Keithley2400.EnumMeasRangeAmps>(target =>
                {
                    try
                    {

                        K2400.SetMeasRangeOfAmps(target);
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>(string.Format("unable to set measurement range, {0}", ex.Message), "ERROR"));
                    }
                });
            }
        }

        public RelayCommand<Keithley2400.EnumMeasRangeVolts> SetVoltageMeasurementRange
        {
            get
            {
                return new RelayCommand<Keithley2400.EnumMeasRangeVolts>(target =>
                {
                    try
                    {

                        K2400.SetMeasRangeOfVolts(target);
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>(string.Format("unable to set measurement range, {0}", ex.Message), "ERROR"));
                    }
                });
            }
        }

        /// <summary>
        /// Set SourceMeter output level per source mode
        /// </summary>
        public RelayCommand<double> SetOutputLevel
        {
            get
            {
                return new RelayCommand<double>(target =>
                {
                    try
                    {
                        if (K2400.SourceMode == Keithley2400.EnumSourceMode.CURR)
                        {
                            K2400.SetCurrentSourceLevel(target);
                        }
                        else if (K2400.SourceMode == Keithley2400.EnumSourceMode.VOLT)
                        {
                            K2400.SetVoltageSourceLevel(target);
                        }
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>(string.Format("unable to set compliance, {0}", ex.Message), "ERROR"));
                    }
                });
            }
        }

        #endregion
    }

    /// <summary>
    /// Auto tuning the unit of the measurement value in this class
    /// </summary>
    public class FormatMeasurementValue : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string ret = "";
            Keithley2400.AmpsUnit unit_curr;
            Keithley2400.VoltsUnit unit_volt;
            
            if(double.TryParse(value[0].ToString(), out double measval))
            {
                var func = (Keithley2400.EnumMeasFunc)value[1];
                bool output = (bool)value[2];

                if (output)
                {
                    if (func == Keithley2400.EnumMeasFunc.ONCURR)
                    {
                        // assuming the incoming value is in A
                        if (measval < 0.00105) // convert to uA     
                        {
                            measval *= 1000000;
                            unit_curr = Keithley2400.AmpsUnit.uA;
                        }
                        else if (measval < 1.05) // convert to mA
                        {
                            measval *= 1000;
                            unit_curr = Keithley2400.AmpsUnit.mA;
                        }
                        else // stay in A
                        {
                            unit_curr = Keithley2400.AmpsUnit.A;
                        }

                        ret = string.Format("{0:F6} {1}", measval, unit_curr);
                    }
                    else if (func == Keithley2400.EnumMeasFunc.ONVOLT)
                    {
                        // assuming the incoming value is in V
                        if (measval < 0.00105) // convert to uV
                        {
                            measval *= 1000;
                            unit_volt = Keithley2400.VoltsUnit.uV;
                        }
                        else if (measval < 1.05) // convert to mV
                        {
                            measval *= 1000;
                            unit_volt = Keithley2400.VoltsUnit.mV;
                        }
                        else // stay in A
                        {
                            unit_volt = Keithley2400.VoltsUnit.V;
                        }

                        ret = string.Format("{0:F6} {1}", measval, unit_volt);
                    }
                    else
                    {
                        ret = "FUNC ERR";
                    }
                }
                else
                {
                    ret = "OFF";
                }

                return ret;
            }
            else
            {
                return "MEAS ERR";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
