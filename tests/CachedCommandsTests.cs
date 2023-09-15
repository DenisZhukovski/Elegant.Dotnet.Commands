using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotnet.Commands.UnitTests.Mocks;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class CachedCommandsTests : CommandsCommonTests
    {
        private readonly ICommands _commands;

        public CachedCommandsTests()
            : this(new Commands())
        {
        }
        
        protected CachedCommandsTests(ICommands commands)
            : base(commands.Cached())
        {
            _commands = commands.Cached();
        }

        [Fact]
        public void ReturnsTheSameCommand()
        {
            Assert.Same(
                _commands.Command(() => { }),
                _commands.Command(() => { })
            );
        }

        [Fact]
        public void ReturnsTheSameAsyncCommand()
        {
            Assert.Same(
                _commands.AsyncCommand(() => Task.CompletedTask),
                _commands.AsyncCommand(() => Task.CompletedTask)
            );
        }

        [Fact]
        public void CachedCommandInViewModel()
        {
            var viewModel = new CommandsViewModel(new Commands());
            viewModel.IncreaseQuantityCommand.Execute();
            viewModel.IncreaseQuantityCommand.Execute();
            Assert.Equal(
                2,
                viewModel.Quantity
            );
        }

        /*
         * The main idea to check that cached commands are thread safe dictionary can handle getAdd in multi-threading environment.
         */
        [Fact]
        public Task CachedCommandInViewModelThreadSafe()
        {
            var viewModel = new CommandsViewModel(new Commands().Validated());
            return Asserts.DoesNotThrow(async () =>
            {
                var tasks = new List<Task>();
                for (var i = 0; i < 10000000; i++)
                {
                    tasks.Add(Task.Run(() => viewModel.IncreaseQuantityCommand));
                }
                await Task.WhenAll(tasks);
            });
        }

        [Fact]
        public void ThrowArgumentNullExceptionIfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _commands
                    .Command(() => { }, name: null)
                    .Execute()
            );
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
