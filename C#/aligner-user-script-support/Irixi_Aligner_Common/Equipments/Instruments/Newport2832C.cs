using Irixi_Aligner_Common.Configuration.Equipments;
using Irixi_Aligner_Common.Equipments.Base;
using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;

namespace Irixi_Aligner_Common.Equipments.Instruments
{
    public class Newport2832C : InstrumentBase
    {
        #region Definition

        // for 2832C, the maximum channel is always 2
        const int MAX_CH = 2;

        public enum EnumChannel
        {
            A,
            B
        }

        /// <summary>
        /// Valid current measurement range
        /// </summary>
        public enum EnumRange
        {
            [Description("AUTO")]
            AUTO,

            [Description("RANGE 0")]
            R0 = 0,

            [Description("RANGE 1")]
            R1,

            [Description("RANGE 2")]
            R2,

            [Description("RANGE 3")]
            R3,

            [Description("RANGE 4")]
            R4,

            [Description("RANGE 5")]
            R5,

            [Description("RANGE 6")]
            R6
        }

        public enum EnumUnits
        {
            [Description("A")]
            A,  // amps

            [Description("W")]
            W,  // watts

            [Description("W/cm")]
            W_cm, //watts/cm^3

            [Description("dBm")]
            dBm,

            [Description("dB")]
            dB,

            [Description("REL")]
            REL
        }

        [Flags]
        public enum EnumStatusFlag
        {
            OVERRANGE,
            SATURATED,
            DATAERR,
            RANGING
        }

        #endregion

        #region Variables

        #endregion

        #region Constructor

        public Newport2832C(ConfigurationNewport2832C Config) : base(Config)
        {
            this.Config = Config;
            IsMultiChannel = true;

            // create meta properties for each channel
            MetaProperty = new Newport2832CMetaProperty[MAX_CH];
            for (int i = 0; i < MAX_CH; i++)
            {
                MetaProperty[i] = new Newport2832CMetaProperty();
            }

            serialport = new SerialPort(Config.Port, Config.BaudRate)
            {
                ReadTimeout = 500
            };

        }

        #endregion

        #region Properties

        public new ConfigurationNewport2832C Config { get; }

        public Newport2832CMetaProperty[] MetaProperty
        {
            get;
        }

        #endregion

        #region Override Methods

        protected override void UserInitProc()
        {
#if !FAKE_ME
            string desc = this.GetDescription();
            if (desc.ToUpper().IndexOf("2832-C") > -1)
            {
                // reset to default setting and clear the error query
                Reset();

                // Reset settings of channel A
                SetDisplayChannel(EnumChannel.A);
                SetMeasurementRange(EnumChannel.A, EnumRange.AUTO);
                SetLambda(EnumChannel.A, 1550);
                SetUnit(EnumChannel.A, EnumUnits.W);

                // Reset settings of channel B
                SetDisplayChannel(EnumChannel.B);
                SetMeasurementRange(EnumChannel.B, EnumRange.AUTO);
                SetLambda(EnumChannel.B, 1550);
                SetUnit(EnumChannel.B, EnumUnits.W);

                // Reset settings of channel A
                this.ActiveChannel = (int)EnumChannel.A;

                this.IsInitialized = true;

                // Start to auto fetch process
                StartAutoFetching();

            }
            else
            {
                throw new Exception("the description string is error");
            }
#else
            StartAutoFetching();
#endif
        }

        public override double Fetch()
        {

#if !FAKE_ME
            var retgrp = Read("RWS?");

            var ret = retgrp.Split(',');
            if (ret.Length != 4)
                throw new FormatException("the length of the response of the RWS request is error");

            // parse status flag
            for (int i = 0; i < MAX_CH; i++)
            {
                if (int.TryParse(ret[i], out int stat))
                    MetaProperties[i].Status = (EnumStatusFlag)stat;
                else
                    throw new FormatException("the STATUS part of the response of the RWS request is error");
            }

            // parse power value
            for (int i = 0; i < MAX_CH; i++)
            {
                if (double.TryParse(ret[i + 2], out double pwr))
                    MetaProperties[i].MeasurementValue = pwr;
                else
                    throw new FormatException("the POWER part of the response of the RWS request is error");
            }
            
            return MetaProperties[ActiveChannel].MeasurementValue;
#else
            var val = new Random((int)DateTime.Now.Ticks).NextDouble() * 10;
            for (int i = 0; i < MAX_CH; i++)
            {
                MetaProperty[i].MeasurementValue = val;
            }
            return val;
#endif
        }

        protected override void DoAutoFecth(CancellationToken token, IProgress<EventArgs> progress)
        {
            while (!token.IsCancellationRequested)
            {
                Fetch();
                Thread.Sleep(200);
            }
        }

        #endregion

        #region Appropriative Methods of Newport 2832C

        public void SetDisplayChannel(EnumChannel CH)
        {
            ActivedChannel = (int)CH;
            Send(string.Format("DISPCH {0}", CH));
        }

        public void SetMeasurementRange(EnumChannel CH, EnumRange Range)
        {
            if (Range == EnumRange.AUTO)
            {
                Send(string.Format("AUTO_{0} 1", CH));

            }
            else
            {
                Send(string.Format("RANGE_{0} {1}", CH, (int)Range));
            }

            MetaProperty[(int)CH].Range = Range;
        }

        public void GetMeasurementRange(EnumChannel CH)
        {
            var ret = Read(string.Format("AUTO_{0}?", CH));
            if (ret == "1")
            {
                MetaProperty[(int)CH].Range = EnumRange.AUTO;
            }
            else if (ret == "0")
            {
                ret = Read(string.Format("RANGE_{0}?", CH));
                if (int.TryParse(ret, out int range))
                {
                    MetaProperty[(int)CH].Range = (EnumRange)range;
                }
                else
                {
                    throw new FormatException(string.Format("the format of the response of RANGE request is error"));
                }
            }
            else
            {
                throw new FormatException(string.Format("the format of the response of AUTO request is error"));
            }
        }

        public void SetLambda(EnumChannel CH, int Lambda)
        {
            Send(string.Format("LAMBDA_{0} {1}", CH, Lambda));
            MetaProperty[(int)CH].Lambda = Lambda;
        }

        public void GetLambda(EnumChannel CH)
        {
            var ret = Read(string.Format("LAMBDA_{0}?", CH));
            if (int.TryParse(ret, out int lambda))
            {
                MetaProperty[(int)CH].Lambda = lambda;
            }
            else
            {
                throw new FormatException(string.Format("the format of the response of LAMBDA request is error"));
            }
        }

        public void SetUnit(EnumChannel CH, EnumUnits Unit)
        {
            if (Unit == EnumUnits.W_cm)
            {
                Send(string.Format("UNITS_{0} \"W/cm\"", CH));
            }
            else
            {
                Send(string.Format("UNITS_{0} \"{1}\"", CH, Unit));
            }

            MetaProperty[(int)CH].Unit = Unit;
        }

        public void GetUnit(EnumChannel CH)
        {
            var ret = Read(string.Format("UNITS_{0}?", CH));
            ret = ret.Replace("\\", "").Replace("\"", "");
            if (ret == "W/cm")
            {
                MetaProperty[(int)CH].Unit = EnumUnits.W_cm;
            }
            else
            {
                MetaProperty[(int)CH].Unit = (EnumUnits)Enum.Parse(typeof(EnumUnits), ret);
            }
        }

        #endregion

    }
}



