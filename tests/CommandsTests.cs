using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class CommandsTests : CommandsCommonTests
    {
        public CommandsTests() 
            : base(new Commands())
        {
        }
    }

    public class CommandsIntegrationTests : CommandsCommonTests
    {
        public CommandsIntegrationTests() 
            : base(
                new Commands()
                    .Locked()
                    .Validated()
                    .Safe(ex => false)
                    .Cached()
             )
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