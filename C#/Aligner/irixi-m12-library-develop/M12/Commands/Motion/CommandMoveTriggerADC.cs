using System.IO;
using M12.Definitions;

namespace M12.Commands.Motion
{
    public class CommandMoveTriggerADC : CommandBase
    {
        public CommandMoveTriggerADC(UnitID UnitID, int Steps, byte Speed, ushort TriggerInterval)
        {
            this.UnitID = UnitID;
            this.Steps = Steps;
            this.Speed = Speed;
            this.TriggerInterval = TriggerInterval;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_MOVE_T_ADC;
            }
        }

        public int Steps { get; set; }

        public byte Speed { get; set; }

        public ushort TriggerInterval { get; set; }


        #endregion

        #region Methods

        internal override byte[] GeneratePayload()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write((byte)UnitID);
                    wr.Write(Steps);
                    wr.Write(Speed);
                    wr.Write(TriggerInterval);
                }

                data = stream.ToArray();
            }

            return data;

        }
        #endregion
    }
}
