using EvlDaemon;
using Xunit;

namespace EvlDaemonTest
{
    public class TpiTest
    {
        [Fact]
        public void ChecksumIsCorrect()
        {
            Assert.Equal("D2", Tpi.CalculateChecksum("6543"));
        }

        [Fact]
        public void ChecksumTruncatedToOneByte()
        {
            Assert.Equal("CA", Tpi.CalculateChecksum("005123456"));
        }

        [Fact]
        public void ChecksumLeadingZeroAdded()
        {
            Assert.Equal("0F", Tpi.CalculateChecksum("5108A"));
        }

        [Fact]
        public void GetCommandReturnsCorrectPart()
        {
            Assert.Equal("005", Tpi.GetCommand("005user54"));
        }

        [Fact]
        public void GetDataReturnsCorrectPart()
        {
            Assert.Equal("user", Tpi.GetData("005user54"));
        }

        [Fact]
        public void GetDataReturnsEmptyString()
        {
            Assert.Equal("", Tpi.GetData("5108A"));
        }

        [Fact]
        public void GetChecksumReturnsCorrectPart()
        {
            Assert.Equal("54", Tpi.GetChecksum("005user54"));
        }

    }
}
