using System.IO;
using M12.Definitions;

namespace M12.Commands.Alignment
{
    public class CommandBlindSearch : CommandBase
    {
        public CommandBlindSearch(BlindSearchArgs HorizontalArgs,  BlindSearchArgs VerticalArgs)
        {
            this.HorizontalArgs = HorizontalArgs;
            this.VerticalArgs = VerticalArgs;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_BLINDSEARCH;
            }
        }

        /// <summary>
        /// Get or set the scan arguments of the horizontal axis.
        /// </summary>
        public BlindSearchArgs HorizontalArgs { get; set; }

        /// <summary>
        /// Get or set the scan arguments of the vertical axis.
        /// </summary>
        public BlindSearchArgs VerticalArgs { get; set; }


        #endregion

        #region Methods

        internal override byte[] GeneratePayload()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write((byte)HorizontalArgs.Unit);
                    wr.Write(HorizontalArgs.Range);
                    wr.Write(HorizontalArgs.Gap);
                    wr.Write(HorizontalArgs.Speed);
                    wr.Write(HorizontalArgs.Interval);

                    wr.Write((byte)VerticalArgs.Unit);
                    wr.Write(VerticalArgs.Range);
                    wr.Write(VerticalArgs.Gap);
                    wr.Write(VerticalArgs.Speed);
                    wr.Write(VerticalArgs.Interval);
                }

                data = stream.ToArray();
            }

            return data;

        }
        #endregion
    }
}
