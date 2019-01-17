using System;
using System.Collections;
using System.IO;

namespace M12.Base
{
    public class UnitSettings
    {
        public enum ModeEnum
        {
            OnePulse,
            TwoPulse
        }

        public enum PulsePinEnum
        {
            CW,
            CCW
        }

        public enum ActiveLevelEnum
        {
            High,
            Low
        }

        public UnitSettings(byte[] Data)
        {
            using (MemoryStream stream = new MemoryStream(Data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    this.UnitID = reader.ReadByte();

                    BitArray bits = new BitArray(reader.ReadBytes(1));
                    this.Mode = bits[0] == false ? ModeEnum.OnePulse : ModeEnum.TwoPulse;
                    this.PulsePin = bits[1] == false ? PulsePinEnum.CW : PulsePinEnum.CCW;
                    this.IsFlipDIR = bits[2];
                    this.IsFlipLimitSensor = bits[3];
                    this.IsDetectTimming = bits[4];
                    this.LimitSensorActiveLevel = bits[5] == false ? ActiveLevelEnum.High : ActiveLevelEnum.Low;

                    this.GeneralAcceleration = reader.ReadUInt16();
                }
            }
        }

        public UnitSettings(ModeEnum Mode, PulsePinEnum PulsePin, 
            bool IsFlipDIR, bool IsFlipLimitSensor, bool IsDetectTimming, 
            ActiveLevelEnum LSActiveLevel)
        {
            this.Mode = Mode;
            this.PulsePin = PulsePin;
            this.IsFlipDIR = IsFlipDIR;
            this.IsFlipLimitSensor = IsFlipLimitSensor;
            this.IsDetectTimming = IsDetectTimming;
            this.LimitSensorActiveLevel = LSActiveLevel;
        }

        public int UnitID { get; set; }
        public ModeEnum Mode { get; set; }
        public PulsePinEnum PulsePin { get; set; }
        public bool IsFlipDIR { get; set; }
        public bool IsFlipLimitSensor { get; set; }
        public bool IsDetectTimming { get; set; }
        public ActiveLevelEnum LimitSensorActiveLevel { get; set; }
        
        public UInt16 GeneralAcceleration { get; set; }

        #region Methods

        public byte[] ToArray()
        {
            byte ret = 0;

            ret |= (byte)((this.Mode == ModeEnum.OnePulse ? 0 : 1) << 0);
            ret |= (byte)((this.PulsePin == PulsePinEnum.CW ? 0 : 1) << 1);
            ret |= (byte)((this.IsFlipDIR == false ? 0 : 1) << 2);
            ret |= (byte)((this.IsFlipLimitSensor == false ? 0 : 1) << 3);
            ret |= (byte)((this.IsDetectTimming == false ? 0 : 1) << 4);
            ret |= (byte)((this.LimitSensorActiveLevel == ActiveLevelEnum.High ? 0 : 1) << 5);

            return new byte[] { ret };
        }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(ModeEnum), Mode)}, " +
                $"Pulse Pin {Enum.GetName(typeof(PulsePinEnum), PulsePin)}, " +
                $"{(IsFlipDIR ? "DIR Flipped" : "DIR Default") }, " +
                $"{(IsFlipLimitSensor ? "LS Flipped" : "LS Default")}, " +
                $"{(IsDetectTimming ? "Detect Timming" : "Ignore Timming")}, " +
                $"LS {Enum.GetName(typeof(ActiveLevelEnum), LimitSensorActiveLevel)}, " +
                $"Move Acc: {GeneralAcceleration} steps";
        }

        #endregion
    }
}
