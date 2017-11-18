using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcpMessages
{
    class ByteChunk
    {
        public ByteChunk(byte[] data, int index, int length)
        {
            this.Data = data;
            this.Index = index;
            this.Length = length;
        }

        public ByteChunk(byte[] data, int index = 0) : this(data, index, data.Length - index)
        {
        }

        public byte[] Data { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
    }
}
