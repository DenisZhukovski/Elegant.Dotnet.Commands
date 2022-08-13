using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotnet.Commands.UnitTests.Mocks;
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
        public void CommandCanExecuteFalse()
        {
            Assert.False(
                new Commands()
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void CommandCanExecuteTrue()
        {
            Assert.False(
                new Commands()
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void AsyncCommandCanExecuteFalse()
        {
            Assert.False(
                new Commands()
                    .AsyncCommand(() => Task.CompletedTask, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void AsyncCommandCanExecuteTrue()
        {
            Assert.False(
                new Commands()
                    .AsyncCommand(() => Task.CompletedTask, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public async Task LongAsyncCommandExecution_IgnoresAllOtherExecutions()
        {
            var longTask = new TaskCompletionSource<bool>();
            int executionsCount = 0;
            var commandTasks = new Commands().ExecuteAsync(
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

        [Fact]
        public void CommandExecution()
        {
            int executionsCount = 0;
            new Commands()
                .Command(() => executionsCount++)
                .Execute(null);
            Assert.Equal(1, executionsCount);
        }

        [Fact]
        public void CommandExecutionWithParameter()
        {
            int expectedNumber = 0;
            new Commands()
                .Command<int>((number) => expectedNumber = number)
                .Execute(13);
            Assert.Equal(13, expectedNumber);
        }

        [Fact]
        public async Task AsyncCommandExecutionWithParameter()
        {
            int expectedNumber = 0;
            await new Commands()
                .AsyncCommand<int>(async (number) =>
                {
                    await Task.Delay(300);
                    expectedNumber = number;
                })
                .ExecuteAsync(13);
            Assert.Equal(13, expectedNumber);
        }

        [Fact]
        public async Task AsyncCommandExecution()
        {
            int executionsCount = 0;
            await new Commands()
                .AsyncCommand(async () =>
                {
                    await Task.Delay(500);
                    executionsCount++;
                })
                .ExecuteAsync(null);
            Assert.Equal(1, executionsCount);
        }

        [Fact]
        public void CommandThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Commands()
                    .Command(() =>
                    {
                        throw new ArgumentException(string.Empty);
                    })
                    .Execute()
            );
        }

        [Fact]
        public void CommandWithParameterThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Commands()
                    .Command<int>((number) =>
                    {
                        throw new ArgumentException(string.Empty);
                    })
                    .Execute(13)
            );
        }

        [Fact]
        public Task AsyncCommand_ThrowsException()
        {
            return Assert.ThrowsAsync<ArgumentException>(() =>
                new Commands()
                    .AsyncCommand(async () =>
                    {
                        await Task.Delay(300);
                        throw new ArgumentException(string.Empty);
                    })
                    .ExecuteAsync()
            );
        }

        [Fact]
        public Task AsyncCommandWithParameter_ThrowsException()
        {
            return Assert.ThrowsAsync<ArgumentException>(() =>
                new Commands()
                    .AsyncCommand<int>(async (number) =>
                    {
                        await Task.Delay(300);
                        throw new ArgumentException(string.Empty);
                    })
                    .ExecuteAsync(13)
            );
        }

        [Fact]
        public async Task CanExecuteFalse_AsyncCommandNotExecuted()
        {
            var commandExecuted = false;
            await new Commands()
                .AsyncCommand(async () =>
                {
                    await Task.Delay(500);
                    commandExecuted = true;
                }, () => false)
                .ExecuteAsync(null);

            Assert.False(commandExecuted);
        }

        [Fact]
        public void CanExecuteFalse_CommandNotExecuted()
        {
            var commandExecuted = false;
            new Commands()
                .Command(() => {
                    commandExecuted = true;
                }, () => false)
                .ExecuteAsync(null);

            Assert.False(commandExecuted);
        }

        [Fact]
        public void CanExecuteIsNull_CommandExecuted()
        {
            var commandExecuted = false;
            new Commands()
                .Command(() => {
                    commandExecuted = true;
                }, null)
                .ExecuteAsync(null);

            Assert.True(commandExecuted);
        }

        [Fact]
        public async Task CanExecuteTrue_CommandExecuted()
        {
            var commandExecuted = false;
            await new Commands()
                .Command(() => {
                    commandExecuted = true;
                }, () => true)
                .ExecuteAsync(null);

            Assert.True(commandExecuted);
        }

        [Fact]
        public async Task ForceExecute()
        {
            int executionsCount = 0;
            var commands = new Commands();
            var commandsTasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                commandsTasks.Add(
                    commands
                        .AsyncCommand(async () =>
                        {
                            await Task.Delay(500);
                            executionsCount++;
                        }, forceExecution: true)
                        .ExecuteAsync()
                ); 
            }
            await Task.WhenAll(commandsTasks);
            Assert.Equal(100, executionsCount);
        }

        [Fact]
        public void TwoDifferentCommands()
        {
            var commands = new Commands();
            Assert.NotEqual(
                commands.Command(() => { }),
                commands.Command(() => { })
            );
        }

        [Fact]
        public void TwoDifferentAsyncCommands()
        {
            var commands = new Commands();
            Assert.NotEqual(
                commands.AsyncCommand(() => Task.CompletedTask),
                commands.AsyncCommand(() => Task.CompletedTask)
            );
        }
    }
}
