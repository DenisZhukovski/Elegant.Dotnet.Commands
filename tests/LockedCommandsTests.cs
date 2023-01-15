using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dotnet.Commands.UnitTests.Mocks;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class LockedCommandsTests : CommandsCommonTests
    {
        private readonly ICommands _commands;

        public LockedCommandsTests()
            : this(new Commands())
        {
        }
        
        protected LockedCommandsTests(ICommands commands) 
            : base(commands.Locked(0))
        {
            _commands = commands.Locked(0);
        }
        
        [Fact]
        public async Task ForceExecute()
        {
            int executionsCount = 0;
            var commandsTasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                commandsTasks.Add(
                    _commands
                        .AsyncCommand(async () =>
                        {
                            await Task.Delay(500);
                            Interlocked.Increment(ref executionsCount);
                        }, forceExecution: true)
                        .ExecuteAsync()
                ); 
            }
            await Task.WhenAll(commandsTasks);
            Assert.Equal(100, executionsCount);
        }
        
        [Fact]
        public async Task LongAsyncCommandExecution_IgnoresAllOtherExecutions()
        {
            var longTask = new TaskCompletionSource<bool>();
            int executionsCount = 0;
            var commandTasks = _commands.ExecuteAsync(
                () =>
                {
                    executionsCount++;
                    return longTask.Task;
                },
                count: 100
            );
            longTask.SetResult(true);
            await Task.WhenAll(commandTasks);
            Assert.Equal(1, executionsCount);
        }
    }
}