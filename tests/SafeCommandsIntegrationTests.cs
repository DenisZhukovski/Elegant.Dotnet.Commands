using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class SafeCommandsIntegrationTests : SafeCommandsTests
    {
        public SafeCommandsIntegrationTests() 
            : base(new Commands().Locked().Validated().Cached())
        {
        }
        
        [Fact]
        public override void TwoDifferentCommands()
        {
            Assert.True(true);
        }

        [Fact]
        public override void TwoDifferentAsyncCommands()
        {
            Assert.True(true);
        }
    }
}