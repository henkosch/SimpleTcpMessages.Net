using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SimpleTcpMessages.Tests
{
    public class TcpReceiveBufferTests
    {
        [Fact]
        public void ShouldReceiveBytes()
        {
            var buffer = new TcpReceiveBuffer(5);

            Assert.Equal(5, buffer.RemainingByteCount);
            Assert.False(buffer.Completed);

            var received1 = buffer.ReceiveBytes(new byte[] { 1, 2, 3 });
            Assert.Equal(3, received1);

            Assert.Equal(2, buffer.RemainingByteCount);
            Assert.False(buffer.Completed);

            var received2 = buffer.ReceiveBytes(new byte[] { 4, 5, 6 });
            Assert.Equal(2, received2);

            Assert.Equal(0, buffer.RemainingByteCount);
            Assert.True(buffer.Completed);

            Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, buffer.ReceivedBytesResult);
        }
    }
}
