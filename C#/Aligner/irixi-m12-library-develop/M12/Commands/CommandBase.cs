using System;
using System.IO;
using M12.Definitions;
using M12.Interfaces;
using STM32F407;

namespace M12.Commands
{
    public class CommandBase : ICommand
    {
        #region Variable

        const int LEN_MAX_PAYLOAD = 512;
        const int LEN_COMMAND = 1;
        const int LEN_API_ID = 1;
        const int LEN_FRAME_ID = 2;
        const int LEN_CRC = 4;

        /// <summary>
        /// The header of the package.
        /// </summary>
        const byte HEADER = 0x7E;

        const byte API_IDENTIFIER_COMMAND = 0x4D;
        const byte API_IDENTIFIER_DATA = 0x69;

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public virtual UnitID UnitID { get; set; }

        public virtual UInt16 FrameID { get; set; }

        /// <summary>
        /// Get the value of the command.
        /// </summary>
        public virtual Commands Command
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The method should be implemented by the child class to generate the payload.
        /// </summary>
        /// <returns></returns>
        internal virtual byte[] GeneratePayload()
        {
            return new byte[] { };
        }


        /// <summary>
        /// Convert command to byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            // generate byte array of the payload.
            var payload = GeneratePayload();

            if (payload.Length > LEN_MAX_PAYLOAD)
                throw new InvalidDataException("the length of the payload is out-of-range.");

            byte[] output = null;
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(mem))
                {

                    // write package header (uint8).
                    wr.Write(HEADER);

                    // write package length (uint16).
                    wr.Write((UInt16)(payload.Length + LEN_API_ID + LEN_FRAME_ID + LEN_COMMAND));

                    // write API Identifier
                    wr.Write(API_IDENTIFIER_COMMAND);

                    // write frame ID
                    wr.Write(FrameID);

                    // write command (uint8).
                    wr.Write((byte)this.Command);

                    // write payload.
                    if (payload.Length > 0)
                    {
                        wr.Write(payload);
                    }

                    // calculate CRC32 of the package.
                    var pack = mem.ToArray();
                    CRC32 crc = new CRC32();
                    var crcval = crc.Calculate(pack, pack.Length);
                    wr.Write(crcval);
                }

                output = mem.ToArray();
            }

            return output;

        }

        #endregion
    }
}
