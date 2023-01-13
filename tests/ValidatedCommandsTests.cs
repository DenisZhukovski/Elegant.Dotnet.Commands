using System;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class ValidatedCommandsTests : CommandsCommonTests
    {
        private readonly ICommands _commands = new Commands().Validated();
        
        public ValidatedCommandsTests() 
            : base(new Commands().Validated())
        {
        }

        [Fact]
        public void Throws_ArgumentNullException_WhenNoAction()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _commands
                    .Command(null, () => false)
            );
        }
        
        [Fact]
        public void Throws_ArgumentNullException_WhenNoGenericAction()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _commands
                    .Command<int>(null, (p) => false)
            );
        }
        
        [Fact]
        public void Throws_ArgumentNullException_WhenNoAsyncAction()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _commands
                    .AsyncCommand(null, (Func<bool>)(() => false))

            );
        }

        [Fact]
        public void Throws_ArgumentNullException_WhenNoAsyncGenericAction()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _commands
                    .AsyncCommand<int>(null, (p) => false)
            );
        }
    }
}
