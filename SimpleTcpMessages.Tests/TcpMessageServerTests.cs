using Xunit;

namespace SimpleTcpMessages.Tests
{
    public class TcpMessageServerTests
    {
        [Fact]
        public void ShouldSetDefaultAddressWhenNoneIsSpecified()
        {
            var server = new TcpMessageServer();
            Assert.Equal("0.0.0.0", server.Address.ToString());
        }

        [Fact]
        public void ShouldSetRandomPortAfterStartWhenNoneIsSpecified()
        {
            var server = new TcpMessageServer();
            Assert.Equal(0, server.Port);
            server.Start();
            Assert.NotEqual(0, server.Port);
            server.Stop();
            server.WaitForStop();
        }
    }
}
