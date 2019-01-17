using System.Collections.Generic;
using System.IO;

namespace M12.Base
{
    public class MemoryBlock
    {
        public MemoryBlock(byte[] Data)
        {
            using (MemoryStream stream = new MemoryStream(Data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    this.Sequence = reader.ReadUInt16();
                    this.Length = (ushort)(reader.ReadUInt16() / 2);

                    if (this.Length > 0)
                    {
                        this.Values = new List<short>();

                        for (int i = 0; i < Length; i++)
                        {
                            Values.Add(reader.ReadInt16());
                        }
                    }
                }
            }
        }

        public ushort Sequence { get; set; }

        public ushort Length { get; set; }

        public List<short> Values { get; set; }


    }
}
