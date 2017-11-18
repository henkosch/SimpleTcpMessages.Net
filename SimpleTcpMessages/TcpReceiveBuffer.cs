using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcpMessages
{
    public class TcpReceiveBuffer
    {
        int expectedTotalByteCount;
        int receivedByteCount;

        List<ByteChunk> receivedByteChunks;

        byte[] receivedBytesResult;

        public int RemainingByteCount { get { return expectedTotalByteCount - receivedByteCount; } }
        public bool Completed { get { return RemainingByteCount <= 0; } }

        public byte[] ReceivedBytesResult
        {
            get
            {
                if (!Completed) return null;
                if (receivedBytesResult != null) return receivedBytesResult;
                receivedBytesResult = new byte[expectedTotalByteCount];
                var index = 0;
                foreach (var chunk in receivedByteChunks)
                {
                    Array.Copy(chunk.Data, chunk.Index, receivedBytesResult, index, chunk.Length);
                    index += chunk.Length;
                }
                return receivedBytesResult;
            }
        }

        public TcpReceiveBuffer(int expectedTotalByteCount)
        {
            this.expectedTotalByteCount = expectedTotalByteCount;
            receivedByteChunks = new List<ByteChunk>();
        }

        private void AddBytes(ByteChunk chunk)
        {
            receivedByteChunks.Add(chunk);
            receivedByteCount += chunk.Length;
        }

        public int ReceiveBytes(byte[] bytes, int index = 0)
        {
            if (Completed) return 0;

            var receiveBytes = Math.Min(bytes.Length - index, RemainingByteCount);

            AddBytes(new ByteChunk(bytes, index, receiveBytes));

            return receiveBytes;
        }
    }
}
