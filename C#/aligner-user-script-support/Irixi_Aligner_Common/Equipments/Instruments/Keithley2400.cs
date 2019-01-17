using Irixi_Aligner_Common.Configuration.Equipments;
using Irixi_Aligner_Common.Equipments.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace Irixi_Aligner_Common.Equipments.Instruments
{
    //TODO The Voltage Source, Current Source, Voltage Sensor, Current Sensor should be packaged into classes

    /// <summary>
    /// Class of Keithley 2400
    /// Contains the low-level operation functions in this class, and it's ready to bind to the view
    /// The default unit in this class is A/V/Ohm
    /// </summary>
    public class Keithley2400 : InstrumentBase
    {
        #region Definitions

        const double PROT_AMPS_DEF = 0.000105; // default compliance of current is 105uA
        const double PROT_AMPS_MIN = 0.00000105; // minimum compliance of current is 1.05uA
        const double PROT_AMPS_MAX = 1.05; // maximum compliance of current is 1.05A

        const double PROT_VOLT_DEF = 21; // default compliance of voltage is 21V
        const double PROT_VOLT_MIN = 0.21; // minimum compliance of voltage is 210mV
        const double PROT_VOLT_MAX = 210; // maximum compliance of voltage is 210V

        const double MEAS_SPEED_DEF = 1; // default measurement speed is 1 to fit 60Hz power line cycling

        public enum EnumInOutTerminal
        {
            FRONT, REAR
        }

        /// <summary>
        /// 分析源
        /// </summary>
        public enum EnumMeasFunc
        {
            OFFALL,
            ONVOLT,
            ONCURR,
            ONRES
        }

        /// <summary>
        /// 源
        /// </summary>
        public enum EnumSourceMode { VOLT, CURR, MEM }

        /// <summary>
        /// 源工作类型
        /// </summary>
        public enum EnumSourceWorkMode { FIX, LIST, SWP }

        /// <summary>
        /// 
        /// </summary>
        public enum EnumSourceRange { REAL, UP, DOWN, MAX, MIN, DEFAULT, AUTO }

        /// <summary>
        /// Options of compliance setting
        /// </summary>
        public enum EnumComplianceLIMIT { DEFAULT, MAX, MIN, REAL }

        /// <summary>
        /// Options of which measurement result to be read
        /// </summary>
        public enum EnumReadCategory { VOLT = 0, CURR }

        /// <summary>
        /// Valid current measurement range
        /// </summary>
        public enum EnumMeasRangeAmps
        {
            AUTO = 0,
            R1UA,
            R10UA,
            R100UA,
            R1MA,
            R10MA,
            R100MA,
            R1A
        }

        public enum EnumMeasRangeVolts
        {
            AUTO = 0,
            R200MV,
            R2V,
            R21V
        }

        /// <summary>
        /// Elements contained in the data string for commands :FETCh/:READ/:MEAS/:TRAC:DATA
        /// </summary>
        [Flags]
        public enum EnumDataStringElements
        {
            VOLT = 0x1,
            CURR = 0x2,
            RES = 0x4,
            TIME = 0x8,
            STAT = 0x10,
            ALL = VOLT | CURR | RES | TIME | STAT
        }

        /// <summary>
        /// see page 355 of the manual for the definiations of each bit
        /// </summary>
        [Flags]
        public enum EnumOperationStatus
        {
            OFLO = 0x1,
            FILTER = 0x2,
            FRONTREAR = 0x4,
            CMPL = 0x8,
            OVP = 0x10,
            MATH = 0x20,
            NULL = 0x40,
            LIMITS = 0x80,
            LIMITRET0 = 0x100,
            LIMITRET1 = 0x200,
            AUTOOHMS = 0x400,
            VMEAS = 0x800,
            IMEAS = 0x1000,
            RMEAS = 0x2000,
            VSOUR = 0x4000,
            ISOUR = 0x8000,
            RANGECMPL = 0x10000,
            OFFSETCMPS = 0x20000,
            CONTRACTFAIL = 0x40000,
            TESTRET0 = 0x80000,
            TESTRET1 = 0x100000,
            TESTRET2 = 0x200000,
            RMTSENSE = 0x400000,
            PULSEMODE = 0x800000
        }

        public enum AmpsUnit
        {
            uA,
            mA,
            A
        }

        public enum VoltsUnit
        {
            uV,
            mV,
            V
        }
        #endregion

        #region Variables

        EnumSourceMode source_mode = EnumSourceMode.VOLT;
        EnumInOutTerminal inout_terminal = EnumInOutTerminal.FRONT;
        EnumMeasFunc measurement_func = EnumMeasFunc.ONVOLT;
        EnumDataStringElements data_string_elements = EnumDataStringElements.ALL;
        EnumMeasRangeAmps range_amps = EnumMeasRangeAmps.AUTO;
        AmpsUnit ampsMeasUnit;
        VoltsUnit voltsMeasUnit;
        EnumMeasRangeVolts range_volts = EnumMeasRangeVolts.AUTO;
        Keithliey2400ProgressReportArgs prgChgArgs = new Keithliey2400ProgressReportArgs();
        double meas_speed = MEAS_SPEED_DEF;
        bool is_output_enabled = false;
        double voltage_level = 0, current_level = 0;
        double measured_val = 0;
        double cmpl_voltage = PROT_VOLT_DEF, cmpl_current = PROT_AMPS_DEF;
        bool is_in_range_cmpl = false, is_meas_over_range = false;

        #endregion

        #region Constructor

        public Keithley2400(ConfigurationKeithley2400 Config) : base(Config)
        {
            this.Config = Config;
            serialport = new SerialPort(Config.Port, Config.BaudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 2000
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the output source mode
        /// </summary>
        public EnumSourceMode SourceMode
        {
            private set
            {
                UpdateProperty(ref source_mode, value);
            }
            get
            {
                return source_mode;
            }
        }

        public EnumMeasFunc MeasurementFunc
        {
            private set
            {
                UpdateProperty(ref measurement_func, value);
            }
            get
            {
                return measurement_func;
            }
        }

        /// <summary>
        /// Get the measurement speed by NPLC (Number of Power Line Cycles)
        /// </summary>
        public double MeasurementSpeed
        {
            private set
            {
                UpdateProperty(ref meas_speed, value);
            }
            get
            {
                return meas_speed;
            }
        }

        /// <summary>
        /// Get whether the output is enabled
        /// </summary>
        public bool IsOutputEnabled
        {
            private set
            {
                UpdateProperty(ref is_output_enabled, value);
            }
            get
            {
                return is_output_enabled;
            }
        }

        /// <summary>
        /// Get which output panel is valid
        /// </summary>
        public EnumInOutTerminal InOutTerminal
        {
            private set
            {
                UpdateProperty(ref inout_terminal, value);
            }
            get
            {
                return inout_terminal;
            }
        }

        /// <summary>
        /// Get the valid measurement value according to current setting, unit A/V
        /// </summary>
        public double MeasurementValue
        {
            private set
            {
                UpdateProperty(ref measured_val, value);
            }
            get
            {
                return measured_val;
            }
        }

        /// <summary>
        /// Get the unit of the amps measurement value displayed
        /// </summary>
        public AmpsUnit AmpsMeasUnit
        {
            get => ampsMeasUnit;

            private set
            {
                UpdateProperty(ref ampsMeasUnit, value);
            }

        }

        /// <summary>
        /// Get the unit of the volts measurement value displayed
        /// </summary>
        public VoltsUnit VoltsMeasUnit
        {
            get => voltsMeasUnit;
            private set
            {
                UpdateProperty(ref voltsMeasUnit, value);
            }
        }

        /// <summary>
        /// Get the range of current measurement, unit A
        /// </summary>
        public EnumMeasRangeAmps MeasRangeOfAmps
        {
            private set
            {
                UpdateProperty(ref range_amps, value);
            }
            get
            {
                return range_amps;
            }
        }

        /// <summary>
        /// Get the range of voltage measurement, unit V
        /// </summary>
        public EnumMeasRangeVolts MeasRangeOfVolts
        {
            private set
            {
                UpdateProperty(ref range_volts, value);
            }
            get
            {
                return range_volts;
            }
        }

        /// <summary>
        /// Get the voltage set by user in V
        /// </summary>
        public double OutputVoltageLevel
        {
            set
            {
                UpdateProperty(ref voltage_level, value);
            }
            get
            {
                return voltage_level;
            }
        }

        /// <summary>
        /// Get the current set by user in A
        /// </summary>
        public double OutputCurrentLevel
        {
            set
            {
                UpdateProperty(ref current_level, value);
            }
            get
            {
                return current_level;
            }
        }

        /// <summary>
        /// Get compliance in voltage source mode
        /// </summary>
        public double ComplianceVoltage
        {
            private set
            {
                UpdateProperty(ref cmpl_voltage, value);
            }
            get
            {
                return cmpl_voltage;
            }
        }

        /// <summary>
        /// Get compliance in current source mode
        /// </summary>
        public double ComplianceCurrent
        {
            private set
            {
                UpdateProperty(ref cmpl_current, value);
            }
            get
            {
                return cmpl_current;
            }
        }

        public new ConfigurationKeithley2400 Config
        {
            private set;
            get;
        }

        /// <summary>
        /// Get specified data elements for data string
        /// </summary>
        public EnumDataStringElements DataStringElements
        {
            private set
            {
                data_string_elements = value;
            }
            get
            {
                return data_string_elements;
            }
        }

        /// <summary>
        /// Get whether measurement was made while in over-range
        /// </summary>
        public bool IsMeasOverRange
        {
            private set
            {
                UpdateProperty(ref is_meas_over_range, value);
            }
            get
            {
                return is_meas_over_range;
            }
        }

        /// <summary>
        /// Get whether in range compliance
        /// </summary>
        public bool IsInRangeCompliance
        {
            private set
            {
                UpdateProperty(ref is_in_range_cmpl, value);
            }
            get
            {
                return is_in_range_cmpl;
            }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Set the SourceMeter to V-Source Mode
        /// </summary>
        public void SetToVoltageSource()
        {
            SetOutputState(false);
            SetMeasurementFunc(EnumMeasFunc.ONCURR);
            SetSourceMode(EnumSourceMode.VOLT);
            SetRangeOfVoltageSource(EnumSourceRange.AUTO);

            // only return current measurement value under V-Source
            SetDataElement(EnumDataStringElements.CURR | EnumDataStringElements.STAT);
        }

        /// <summary>
        /// Set the SourceMeter to I-Source Mode
        /// </summary>
        public void SetToCurrentSource()
        {
            SetOutputState(false);
            SetMeasurementFunc(Keithley2400.EnumMeasFunc.ONVOLT);
            SetSourceMode(Keithley2400.EnumSourceMode.CURR);
            SetRangeOfCurrentSource(Keithley2400.EnumSourceRange.AUTO);

            // only return voltage measurement value under I-Source
            SetDataElement(EnumDataStringElements.VOLT | EnumDataStringElements.STAT);
        }

        #endregion

        #region Appropriative Methods of Keithley 2400

        #region Common

        public void SetOutputState(bool IsEnabled)
        {
            if (IsEnabled)
            {
                Send("OUTP ON");
            }
            else
            {
                // the fetching loop MUST be stopped before turn off the output
                StopAutoFetching();

                Send("OUTP OFF");
                // SetBeep(700, 0.2);
                this.MeasurementValue = 0;
            }

            this.IsOutputEnabled = IsEnabled;
        }

        public void GetOutputState()
        {
            var ret = Read("OUTP?");
            if (bool.TryParse(ret, out bool r))
                this.IsOutputEnabled = r;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));
        }

        /// <summary>
        /// Set which In/Out terminal is valid
        /// </summary>
        /// <param name="Terminal">Front panel / Rear panel</param>
        public void SetInOutTerminal(EnumInOutTerminal Terminal)
        {
            switch (Terminal)
            {
                case EnumInOutTerminal.FRONT:
                    Send(":ROUT:TERM FRON");
                    break;

                case EnumInOutTerminal.REAR:
                    Send(":ROUT:TERM REAR");
                    break;
            }
        }

        public void GetInOutTerminal()
        {
            var ret = Read(":ROUT:TERM?");

            if (ret.Contains("FRON"))
                this.InOutTerminal = EnumInOutTerminal.FRONT;
            else if (ret.Contains("REAR"))
                this.InOutTerminal = EnumInOutTerminal.REAR;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));
        }

        #endregion

        #region Format Subsystem
        /// <summary>
        /// Set the elements valid while executing :Read/etc. commands
        /// </summary>
        /// <param name="Elements"></param>
        public void SetDataElement(EnumDataStringElements Elements)
        {
            List<string> elemlsit = new List<string>();

            if (Elements.HasFlag(EnumDataStringElements.VOLT))
                elemlsit.Add(EnumDataStringElements.VOLT.ToString());

            if (Elements.HasFlag(EnumDataStringElements.CURR))
                elemlsit.Add(EnumDataStringElements.CURR.ToString());

            if (Elements.HasFlag(EnumDataStringElements.RES))
                elemlsit.Add(EnumDataStringElements.RES.ToString());

            if (Elements.HasFlag(EnumDataStringElements.TIME))
                elemlsit.Add(EnumDataStringElements.TIME.ToString());

            if (Elements.HasFlag(EnumDataStringElements.STAT))
                elemlsit.Add(EnumDataStringElements.STAT.ToString());

            if (elemlsit.Count == 0)
                throw new ArgumentException(string.Format("the null elemtents passed, {0}", new StackTrace().GetFrame(0).ToString()));
            else
            {
                string arg = String.Join(",", elemlsit.ToArray());
                Send(string.Format("FORM:ELEM {0}", arg));

                this.DataStringElements = Elements;
            }
        }
        #endregion

        #region Sense1 Subsystem

        public void SetMeasurementSpeed(double Nplc)
        {
            Send(string.Format(":SENS:CURR:NPLC {0}", Nplc));
        }

        public void GetMeasurementSpeed()
        {
            var ret = Read(":SENS:CURR:NPLC?");
            if (double.TryParse(ret, out double r))
                this.MeasurementSpeed = r;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));

        }

        public void SetMeasurementFunc(EnumMeasFunc MeasFunc)
        {
            switch (MeasFunc)
            {
                case EnumMeasFunc.OFFALL:
                    Send(":SENS:FUNC:OFF:ALL");
                    break;

                case EnumMeasFunc.ONCURR:
                    Send(":SENS:FUNC:OFF:ALL;:SENS:FUNC:ON \"CURR\"");
                    break;

                case EnumMeasFunc.ONVOLT:
                    Send(":SENS:FUNC:OFF:ALL;:SENS:FUNC:ON \"VOLT\"");
                    break;

                case EnumMeasFunc.ONRES:
                    Send(":SENS:FUNC:OFF:ALL;:SENS:FUNC:ON \"RES\"");
                    break;
            }

            this.MeasurementFunc = MeasFunc;
        }

        public void GetMeasurementFunc()
        {
            //CURR:DC
            var ret = Read(":SENS:FUNC?");

            if (ret == "")
                this.MeasurementFunc = EnumMeasFunc.OFFALL;
            else if (ret.Contains("CURR"))
                this.MeasurementFunc = EnumMeasFunc.ONCURR;
            else if (ret.Contains("VOLT"))
                this.MeasurementFunc = EnumMeasFunc.ONVOLT;
            else if (ret.Contains("CURR"))
                this.MeasurementFunc = EnumMeasFunc.ONRES;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));

        }

        public void SetMeasRangeOfAmps(EnumMeasRangeAmps Range)
        {
            switch (Range)
            {
                case EnumMeasRangeAmps.AUTO:
                    Send(":SENS:CURR:RANG:AUTO ON");
                    break;

                default:
                    Send(string.Format(":SENS:CURR:RANG {0}", this.ConvertMeasRangeAmpsToDouble(Range)));
                    break;
            }

            GetMeasRangeOfAmps();
        }

        public void GetMeasRangeOfAmps()
        {
            var ret = Read("SENS:CURR:RANGE:AUTO?");
            if (ret.Contains("1"))
            {
                this.MeasRangeOfAmps = EnumMeasRangeAmps.AUTO;
            }
            else
            {

                ret = Read(":SENS:CURR:RANG?");

                if (double.TryParse(ret, out double r))
                    this.MeasRangeOfAmps = this.ConvertDoubleToMeasRangeAmps(r);
                else
                    throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));
            }
        }

        public void SetMeasRangeOfVolts(EnumMeasRangeVolts Range)
        {
            switch (Range)
            {
                case EnumMeasRangeVolts.AUTO:
                    Send(":SENS:VOLT:RANG:AUTO ON");
                    break;

                default:
                    Send(string.Format(":SENS:VOLT:RANG {0}", this.ConvertMeasRangeVoltToDouble(Range)));
                    break;
            }

            GetMeasRangeOfVolts();
        }

        public void GetMeasRangeOfVolts()
        {
            var ret = Read("SENS:VOLT:RANGE:AUTO?");
            if (ret.Contains("1"))
            {
                this.MeasRangeOfVolts = EnumMeasRangeVolts.AUTO;
            }
            else
            {
                ret = Read(":SENS:VOLT:RANG?");

                if (double.TryParse(ret, out double r))
                    this.MeasRangeOfVolts = this.ConvertDoubleToMeasRangeVolt(r);
                else
                    throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));
            }
        }

        #endregion

        #region Source Subsystem

        public void SetSourceMode(EnumSourceMode Mode)
        {
            switch (Mode)
            {
                case EnumSourceMode.CURR:
                    Send(":SOUR:FUNC CURR");
                    break;

                case EnumSourceMode.VOLT:
                    Send(":SOUR:FUNC VOLT");
                    break;

                case EnumSourceMode.MEM:
                    Send(":SOUR:FUNC MEM");
                    break;
            }

            this.SourceMode = Mode;
        }

        public void GetSourceMode()
        {
            var ret = Read(":SOUR:FUNC?");

            if (ret.Contains("CURR"))
                this.SourceMode = EnumSourceMode.CURR;
            else if (ret.Contains("VOLT"))
                this.SourceMode = EnumSourceMode.VOLT;
            else if (ret.Contains("MEM"))
                this.SourceMode = EnumSourceMode.MEM;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));
        }

        public void SetSourcingModeOfCurrentSource(EnumSourceWorkMode Mode)
        {
            switch (Mode)
            {
                case EnumSourceWorkMode.FIX:
                    Send(":SOUR:CURR:MODE FIX");
                    break;

                case EnumSourceWorkMode.LIST:
                    Send(":SOUR:CURR:MODE LIST");
                    break;

                case EnumSourceWorkMode.SWP:
                    Send(":SOUR:CURR:MODE SWP");
                    break;
            }
        }

        public void SetSourcingModeOfVoltageSource(EnumSourceWorkMode Mode)
        {
            switch (Mode)
            {
                case EnumSourceWorkMode.FIX:
                    Send(":SOUR:VOLT:MODE FIX");
                    break;

                case EnumSourceWorkMode.LIST:
                    Send(":SOUR:VOLT:MODE LIST");
                    break;

                case EnumSourceWorkMode.SWP:
                    Send(":SOUR:VOLT:MODE SWP");
                    break;
            }
        }

        public void SetRangeOfCurrentSource(EnumSourceRange Range, double Real = -1)
        {
            switch (Range)
            {
                case EnumSourceRange.AUTO:
                    Send(":SOUR:CURR:RANG:AUTO 1");
                    break;

                case EnumSourceRange.DEFAULT:
                    Send(":SOUR:CURR:RANG DEF");
                    break;

                case EnumSourceRange.DOWN:
                    Send(":SOUR:CURR:RANG DOWN");
                    break;

                case EnumSourceRange.UP:
                    Send(":SOUR:CURR:RANG UP");
                    break;

                case EnumSourceRange.MIN:
                    Send(":SOUR:CURR:RANG MIN");
                    break;

                case EnumSourceRange.MAX:
                    Send(":SOUR:CURR:RANG MAX");
                    break;

                case EnumSourceRange.REAL:
                    Send(":SOUR:CURR:RANG " + ((decimal)Real).ToString());
                    break;
            }
        }

        public void SetRangeOfVoltageSource(EnumSourceRange Range, double Real = -1)
        {
            switch (Range)
            {
                case EnumSourceRange.AUTO:
                    Send(":SOUR:VOLT:RANG:AUTO 1");
                    break;

                case EnumSourceRange.DEFAULT:
                    Send(":SOUR:VOLT:RANG DEF");
                    break;

                case EnumSourceRange.DOWN:
                    Send(":SOUR:VOLT:RANG DOWN");
                    break;

                case EnumSourceRange.UP:
                    Send(":SOUR:VOLT:RANG UP");
                    break;

                case EnumSourceRange.MIN:
                    Send(":SOUR:VOLT:RANG MIN");
                    break;

                case EnumSourceRange.MAX:
                    Send(":SOUR:VOLT:RANG MAX");
                    break;

                case EnumSourceRange.REAL:
                    Send(":SOUR:VOLT:RANG " + ((decimal)Real).ToString());
                    break;
            }
        }

        public void SetComplianceCurrent(EnumComplianceLIMIT Cmpl, double Real = -1)
        {
            switch (Cmpl)
            {
                case EnumComplianceLIMIT.DEFAULT:
                    Send(":SENS:CURR:PROT DEF");
                    break;

                case EnumComplianceLIMIT.MIN:
                    Send(":SENS:CURR:PROT MIN");
                    break;

                case EnumComplianceLIMIT.MAX:
                    Send(":SENS:CURR:PROT MAX");
                    break;

                case EnumComplianceLIMIT.REAL:
                    Send(":SENS:CURR:PROT " + Real.ToString("F7"));
                    break;
            }

            GetComplianceCurrent();
        }

        public void GetComplianceCurrent()
        {
            var ret = Read(":SENS:CURR:PROT?");

            if (double.TryParse(ret, out double r))
                this.ComplianceCurrent = r;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));
        }

        public void SetComplianceVoltage(EnumComplianceLIMIT Cmpl, double Real = -1)
        {
            switch (Cmpl)
            {
                case EnumComplianceLIMIT.DEFAULT:
                    Send(":SENS:VOLT:PROT DEF");
                    break;

                case EnumComplianceLIMIT.MIN:
                    Send(":SENS:VOLT:PROT MIN");
                    break;

                case EnumComplianceLIMIT.MAX:
                    Send(":SENS:VOLT:PROT MAX");
                    break;

                case EnumComplianceLIMIT.REAL:
                    Send(":SENS:VOLT:PROT " + ((decimal)Real).ToString());
                    break;
            }

            GetComplianceVoltage();
        }

        public void GetComplianceVoltage()
        {
            var ret = Read(":SENS:VOLT:PROT?");

            if (double.TryParse(ret, out double r))
                this.ComplianceVoltage = r;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));
        }

        public void SetVoltageSourceLevel(double Voltage)
        {
            Send(":SOUR:VOLT:LEV " + Voltage.ToString());
            this.OutputVoltageLevel = Voltage;
        }

        public void GetVoltageSourceLevel()
        {
            var ret = Read(":SOUR:VOLT:LEV?");

            if (double.TryParse(ret, out double r))
                this.OutputVoltageLevel = r;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));
        }

        public void SetCurrentSourceLevel(double Current)
        {
            Send(":SOUR:CURR:LEV " + Current.ToString());
            this.OutputCurrentLevel = Current;
        }

        public void GetCurrentSourceLevel()
        {
            var ret = Read(":SOUR:CURR:LEV?");

            if (double.TryParse(ret, out double r))
                this.OutputCurrentLevel = r;
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));

        }
        #endregion

        #region System Subsystem

        /// <summary>
        /// Remove the SourceMeter from the remote state and enables the operation of front panel keys
        /// </summary>
        public void SetExitRemoteState()
        {
            Send(":SYST:LOC");
        }

        /// <summary>
        /// Control beeper
        /// </summary>
        /// <param name="Frequency"></param>
        /// <param name="Duration"></param>
        public void SetBeep(double Frequency, double Duration)
        {
            if (Frequency < 65 || Frequency > 2000000)
            {
                throw new ArgumentException(string.Format("the argument frequency is invalid, {0}", new StackTrace().GetFrame(0).ToString()));
            }
            else if (Duration < 0 || Duration > 7.9)
            {
                throw new ArgumentException(string.Format("the argument duration is invalid, {0}", new StackTrace().GetFrame(0).ToString()));
            }
            else
            {

                Send(string.Format(":SYST:BEEP {0},{1}", Frequency, Duration));
            }
        }

        /// <summary>
        /// Enable or disable beeper
        /// </summary>
        /// <param name="IsEnabled"></param>
        public void SetBeeperState(bool IsEnabled)
        {
            if (IsEnabled)
                Send(":SYST:BEEP:STAT ON");
            else
                Send(":SYST:BEEP:STAT OFF");
        }

        #endregion

        #region Display Subsystem
        /// <summary>
        /// Enable or disable the front display circuitry, when disabled, the instrument operates at a higher speed
        /// </summary>
        /// <param name="IsEnabled"></param>
        public void SetDisplayCircuitry(bool IsEnabled)
        {
            if (IsEnabled)
                Send(":DISP:ENAB ON");
            else
                Send(":DISP:ENAB OFF");
        }

        /// <summary>
        /// Enable or disable the text message display function
        /// </summary>
        /// <param name="WinId"></param>
        /// <param name="IsEnabled"></param>
        public void SetDisplayTextState(int WinId, bool IsEnabled)
        {
            if (WinId != 1 && WinId != 2)
            {
                throw new ArgumentOutOfRangeException(string.Format("window id is error, {0}", new StackTrace().GetFrame(0).ToString()));
            }
            else
            {
                if (IsEnabled)
                    Send(string.Format(":DISP:WIND{0}:TEXT:STAT ON", WinId));
                else
                    Send(string.Format(":DISP:WIND{0}:TEXT:STAT OFF", WinId));
            }
        }

        /// <summary>
        /// Set the message displayed on the screen
        /// </summary>
        /// <param name="WinId"></param>
        /// <param name="Message"></param>
        public void SetDisplayTextMessage(int WinId, string Message)
        {
            if (WinId != 1 && WinId != 2)
            {
                throw new ArgumentOutOfRangeException(string.Format("window id is error, {0}", new StackTrace().GetFrame(0).ToString()));
            }
            else
            {
                if (WinId == 1 && Message.Length > 20)
                {
                    throw new ArgumentOutOfRangeException(string.Format("the length of message on top display can not be greater then 20, {0}", new StackTrace().GetFrame(0).ToString()));
                }
                else if (WinId == 2 && Message.Length > 32)
                {
                    throw new ArgumentOutOfRangeException(string.Format("the length of message on bottom display can not be greater then 32, {0}", new StackTrace().GetFrame(0).ToString()));
                }
                else
                {
                    Send(string.Format(":DISP:WIND{0}:TEXT:DATA \"{1}\"", WinId, Message));
                }
            }
        }
        #endregion

        #endregion

        #region Override Methods

        protected override void UserInitProc()
        {
#if !FAKE_ME
            string desc = this.GetDescription();
            if (desc.ToUpper().IndexOf("MODEL 2400") > -1 || desc.ToUpper().IndexOf("MODEL 2401") > -1)
            {
                // reset to default setting and clear the error query
                Reset();

                // switch the source mode to v-source
                SetToVoltageSource();

                // Set measurement range
                SetMeasRangeOfAmps(EnumMeasRangeAmps.AUTO);
                SetMeasRangeOfVolts(EnumMeasRangeVolts.AUTO);

                // Set default compliance
                SetComplianceCurrent(EnumComplianceLIMIT.DEFAULT);
                SetComplianceVoltage(EnumComplianceLIMIT.DEFAULT);

                // Set Output level to zero
                SetVoltageSourceLevel(0);
                SetCurrentSourceLevel(0);

                // disable original display
                SetDisplayCircuitry(true);

                // enable user message display
                SetDisplayTextState(1, true);
                SetDisplayTextState(2, true);

                // show user messages
                SetDisplayTextMessage(1, this.Config.Caption);
                SetDisplayTextMessage(2, "powered by IRIXI ALIGNER");

                // enable beeper
                SetBeeperState(true);
            }
            else
            {
                throw new Exception("the identification is error");
            }
#else
            return;
#endif
        }

        public override double Fetch()
        {
#if !FAKE_ME
            var ret = Read(":READ?");
            string[] meas_ret = ret.Split(',');
            if (double.TryParse(meas_ret[0], out double meas_val))
            {

                // if the operation status has been requested
                if (this.DataStringElements.HasFlag(EnumDataStringElements.STAT))
                {
                    // parse the status from data string
                    if (double.TryParse(meas_ret[meas_ret.Length - 1], out double stat_tmp))
                    {
                        var status = (EnumOperationStatus)stat_tmp;


                        // check flag of over-range
                        if (status.HasFlag(EnumOperationStatus.RANGECMPL))
                        {

                            prgChgArgs.IsMeasOverRange = true;
                        }
                        else
                        {
                            prgChgArgs.IsMeasOverRange = false;
                        }

                        // check flag of range compliance
                        if (status.HasFlag(EnumOperationStatus.CMPL))
                        {
                            prgChgArgs.IsInRangeCompliance = true;
                        }
                        else
                        {
                            prgChgArgs.IsInRangeCompliance = false;
                        }
                    }
                }

                prgChgArgs.MeasurementValue = meas_val;



                return meas_val;
            }
            else
                throw new InvalidCastException(string.Format("unknown value {0} returned, {1}", ret, new StackTrace().GetFrame(0).ToString()));

#else
            Thread.Sleep(5);
            return new Random((int)DateTime.Now.Ticks).NextDouble() * 100;
#endif
        }

        protected override void DoAutoFecth(CancellationToken token, IProgress<EventArgs> progress)
        {
            // disable display to speed up instrument operation
            // SetDisplayCircuitry(false);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    Fetch();

                    progress.Report(prgChgArgs);
                }
                catch (Exception ex)
                {
                    throw new AggregateException(new Exception[] { ex });
                }



                Thread.Sleep(20);
            }

            // resume display
            //SetDisplayCircuitry(true);
        }

        protected override void AutoFetchReport(EventArgs Args)
        {
            var args = Args as Keithliey2400ProgressReportArgs;

            // change the values on the window in UI thread context
            this.IsMeasOverRange = args.IsMeasOverRange;
            this.IsInRangeCompliance = args.IsInRangeCompliance;
            this.MeasurementValue = args.MeasurementValue;
        }

        protected override void UserDisposeProc()
        {
            // turn off output
            SetOutputState(false);

            // enable display
            SetDisplayCircuitry(true);

            // remove remote state
            SetExitRemoteState();
        }

        protected override void Send(string Command)
        {
            try
            {
                lock (serialport)
                {
                    serialport.WriteLine(Command);

                    Thread.Sleep(10);

                    // check if error occured
                    serialport.WriteLine(":SYST:ERR:COUN?");
                    var ret = serialport.ReadLine().Replace("\r", "").Replace("\n", "");

                    if (int.TryParse(ret, out int err_count))
                    {
                        if (err_count != 0) // error occured
                        {
                            // read all errors occured
                            serialport.WriteLine(":SYST:ERR:ALL?");
                            var err = serialport.ReadLine();
                            throw new InvalidOperationException(err);
                        }
                    }
                    else
                    {
                        throw new InvalidCastException(string.Format("unable to convert error count {0} to number", err_count));
                    }
                }
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                throw ex;
            }
        }

        protected override string Read(string Command)
        {

            try
            {
                lock (serialport)
                {
                    serialport.WriteLine(Command);
                    return serialport.ReadLine().Replace("\r", "").Replace("\n", "");
                }
            }
            catch (TimeoutException)
            {
                // read all errors occured
                serialport.WriteLine(":SYST:ERR:ALL?");
                this.LastError = serialport.ReadLine().Replace("\r", "").Replace("\n", "");
                throw new InvalidOperationException(this.LastError);
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        double ConvertMeasRangeAmpsToDouble(EnumMeasRangeAmps Range)
        {
            double real = 1.05 * Math.Pow(10, ((int)Range) - 7);
            return real;
        }

        EnumMeasRangeAmps ConvertDoubleToMeasRangeAmps(double Range)
        {
            var digital = Range / 1.05;
            digital = Math.Log10(digital);
            return (EnumMeasRangeAmps)(digital + 7);
        }

        double ConvertMeasRangeVoltToDouble(EnumMeasRangeVolts Range)
        {
            double real = 2.1 * Math.Pow(10, ((int)Range - 2));
            return real;
        }

        EnumMeasRangeVolts ConvertDoubleToMeasRangeVolt(double Range)
        {
            var digital = Range / 2.1;
            digital = Math.Log10(digital);
            return (EnumMeasRangeVolts)(digital + 2);
        }

        #endregion
    }
}
