using System;
using System.IO;

namespace IrixiStepperControllerHelper
{
    public class CommandStruct
    {
        // the length of command struct
        // 1 byte should be plused for report id at the begin of the buffer
        const int MAX_CMD_LEN = 33;

        const byte REPORT_ID_CMD = 0x1;

        UInt32 _cmd_counter = 0;

        #region Properties

        public EnumCommand Command { set; get; }
        public byte CommandOrder { set; get; }
        public int AxisIndex { set; get; }
        public byte Dummy0 { set; get; }
        public byte Dummy1 { set; get; }
        public int AccSteps { set; get; }
        public int DriveVelocity { set; get; }
        public int TotalSteps { set; get; }
        public MoveMode Mode { set; get; }

        public int GenOutPort { set; get; }
        public OutputState GenOutState { set; get; }

        public bool IsReversed { set; get; }

        #endregion

        #region Methods

        /// <summary>
        /// Convert the command struct to the byte array
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] data = new byte[MAX_CMD_LEN];

            MemoryStream stream = new MemoryStream(data);
            BinaryWriter writer = new BinaryWriter(stream);

            // report ID
            writer.Write(REPORT_ID_CMD);

            writer.Write(_cmd_counter++);
            writer.Write((int)this.Command);
            writer.Write((byte)this.CommandOrder);
            writer.Write((byte)this.AxisIndex);
            writer.Write((byte)0xCC); // dummy byte
            writer.Write((byte)0xCC); // dummy byte

            switch (this.Command)
            {
                case EnumCommand.GENOUT:
                    writer.Write(this.GenOutPort);
                    writer.Write((int)this.GenOutState);
                    break;

                case EnumCommand.REVERSE:
                    writer.Write(this.IsReversed ? (int)1 : (int)0);
                    break;

                default:
                    writer.Write(this.AccSteps);
                    writer.Write(this.DriveVelocity);
                    writer.Write(this.TotalSteps);
                    break;

            }

            writer.Close();
            stream.Close();

            return data;

        }

        #endregion
    }
}
