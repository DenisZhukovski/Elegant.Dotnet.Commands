using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class CommandsUnitTests
    {
        [Fact]
        public void CreatesCommand()
        {
            Assert.NotNull(new Commands().Command(() => { }));
        }

        [Fact]
        public void CreatesAsyncCommand()
        {
            Assert.NotNull(new Commands().AsyncCommand(() => Task.CompletedTask));
        }

        [Fact]
        public void CanExecuteFalse()
        {
            Assert.False(
                new Commands()
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void CanExecuteTrue()
        {
            Assert.False(
                new Commands()
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public async Task SingleCommandExecution_IgnoresAllOtherExecutions()
        {
            var commands = new Commands();
            TaskCompletionSource<bool> longTask = new TaskCompletionSource<bool>();
            int executionsCount = 0;
            Func<Task> longTaskHandler = () => {
                executionsCount++;
                return longTask.Task;
            };

            var commandTasks = new List<Task>();
            for (var i=0;i<100;i++)
            {
                commandTasks.Add(
                    commands
                        .AsyncCommand(longTaskHandler)
                        .ExecuteAsync(null)
                );
            }

            longTask.SetResult(true);
            await Task.WhenAll(commandTasks);

            Assert.Equal(1, executionsCount);
        }

        [Fact]
        public async Task AwaitAsyncCommandExecution()
        {
            int executionsCount = 0;
            await new Commands()
                .AsyncCommand(async () => {
                    await Task.Delay(500);
                    executionsCount++;
                })
                .ExecuteAsync(null);

            Assert.Equal(1, executionsCount);
        }
    }
}
