using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M12.Definitions;

namespace M12.Commands.Motion
{
    public class CommandMove : CommandBase
    {
        public CommandMove(UnitID UnitID, int Steps, byte Speed)
        {
            this.UnitID = UnitID;
            this.Steps = Steps;
            this.Speed = Speed;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_MOVE;
            }
        }
      
        public int Steps { get; set; }

        public byte Speed { get; set; }


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
                }

                data = stream.ToArray();
            }

            return data;

        }
        #endregion
    }
}
