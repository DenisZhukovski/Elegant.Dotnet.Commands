using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dotnet.Commands.UnitTests.Mocks;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class SafeCommandsTests
    {
        private List<Exception> _exceptions = new List<Exception>();

        private readonly ICommands _commands;

        public SafeCommandsTests()
        {
            _commands = new Commands().Validated().Safe(ex => _exceptions.Add(ex));
        }
            
        [Fact]
        public void CreatesCommand()
        {
            Assert.NotNull(_commands.Command(() => { }));
        }

        [Fact]
        public void CreatesAsyncCommand()
        {
            Assert.NotNull(_commands.AsyncCommand(() => Task.CompletedTask));
        }

        [Fact]
        public void CommandCanExecuteFalse()
        {
            Assert.False(
                _commands
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void CommandCanExecuteTrue()
        {
            Assert.False(
                _commands
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void AsyncCommandCanExecuteFalse()
        {
            Assert.False(
                _commands
                    .AsyncCommand(() => Task.CompletedTask, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void AsyncCommandCanExecuteTrue()
        {
            Assert.False(
                _commands
                    .AsyncCommand(() => Task.CompletedTask, () => false)
                    .CanExecute(null)
            );
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

        [Fact]
        public void CommandExecution()
        {
            int executionsCount = 0;
            _commands
                .Command(() => executionsCount++)
                .Execute(null);
            Assert.Equal(1, executionsCount);
        }

        [Fact]
        public void CommandExecutionWithParameter()
        {
            int expectedNumber = 0;
            _commands
                .Command<int>((number) => expectedNumber = number)
                .Execute(13);
            Assert.Equal(13, expectedNumber);
        }

        [Fact]
        public async Task AsyncCommandExecutionWithParameter()
        {
            int expectedNumber = 0;
            await _commands
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
            await _commands
                .AsyncCommand(async () =>
                {
                    await Task.Delay(500);
                    executionsCount++;
                })
                .ExecuteAsync(null);
            Assert.Equal(1, executionsCount);
        }
        
        [Fact]
        public async Task AsyncCommandSyncExecution()
        {
            int executionsCount = 0;
            _commands
                .AsyncCommand(async () =>
                {
                    await Task.Delay(500);
                    executionsCount++;
                })
                .Execute(null);

            await Task.Delay(2000);
            Assert.Equal(1, executionsCount);
        }
        
        [Fact]
        public async Task CanExecuteAsync()
        {
            int executionsCount = 0;
            await _commands
                .AsyncCommand<int>(async (number) =>
                {
                    await Task.Delay(500);
                    executionsCount++;
                }, async (number) =>
                {
                    await Task.Delay(500);
                    return true;
                })
                .ExecuteAsync(12);
            Assert.Equal(1, executionsCount);
        }
        
        [Fact]
        public async Task CanExecuteAsyncFalse()
        {
            int executionsCount = 0;
            await _commands
                .AsyncCommand<int>(async (number) =>
                {
                    await Task.Delay(500);
                    executionsCount++;
                }, async (number) =>
                {
                    await Task.Delay(500);
                    return false;
                })
                .ExecuteAsync(12);
            Assert.Equal(0, executionsCount);
        }

        [Fact]
        public void CommandThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Commands()
                    .Safe(ex => false)
                    .Validated()
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
                    .Safe(ex => false)
                    .Validated()
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
                    .Safe(ex => false)
                    .Validated()
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
                    .Safe(ex => false)
                    .Validated()
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
            await _commands
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
            _commands
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
            _commands
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
            await _commands
                .Command(() => {
                    commandExecuted = true;
                }, () => true)
                .ExecuteAsync(null);

            Assert.True(commandExecuted);
        }
        
        [Fact]
        public void CanExecuteChangedEvent()
        {
            var commandExecuted = true;
            var command = _commands.Command(
                () => { },
                () => false
            );
            command.CanExecuteChanged += (sender, args) =>
            {
                var canExecuteArgs = (CanExecuteArgs)args;
                commandExecuted = canExecuteArgs.CanExecute;
            };

            command.CanExecute(null);

            Assert.False(commandExecuted);
        }

        [Fact]
        public async Task ForceExecute()
        {
            int executionsCount = 0;
            var commands = _commands;
            var commandsTasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                commandsTasks.Add(
                    commands
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
        public void TwoDifferentCommands()
        {
            var commands = _commands;
            Assert.NotEqual(
                commands.Command(() => { }),
                commands.Command(() => { })
            );
        }

        [Fact]
        public void TwoDifferentAsyncCommands()
        {
            var commands = _commands;
            Assert.NotEqual(
                commands.AsyncCommand(() => Task.CompletedTask),
                commands.AsyncCommand(() => Task.CompletedTask)
            );
        }

        [Fact]
        public Task  AsyncCommand_CancelThrowsException()
        {
            var asyncCommand = new Commands()
                .Safe(ex => false)
                .Validated()
                .AsyncCommand(async (ct) =>
                {
                    await Task.Delay(3000).ConfigureAwait(false);
                    ct.ThrowIfCancellationRequested();
                    throw new ArgumentException(string.Empty);
                });
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                asyncCommand.Cancel();
            });
            return Assert.ThrowsAsync<OperationCanceledException>(() =>
                asyncCommand.ExecuteAsync()
            );
        }

        [Fact]
        public Task AsyncCommandWithParam_CancelThrowsException()
        {
            var asyncCommand = new Commands()
                .Safe(ex => false)
                .Validated()
                .AsyncCommand<int>(async (param, ct) =>
                {
                    await Task.Delay(3000).ConfigureAwait(false);
                    ct.ThrowIfCancellationRequested();
                    throw new ArgumentException(string.Empty);
                }, (Func<int, bool>?)null);
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                asyncCommand.Cancel();
            });
            return Assert.ThrowsAsync<OperationCanceledException>(() =>
                asyncCommand.ExecuteAsync(0)
            );
        }

        [Fact]
        public Task AsyncCommand_NotCancelCommand_IfCancellationWasRequestedBeforeExecuting()
        {
            var asyncCommand = new Commands()
                .Safe(ex => false)
                .Validated()
                .AsyncCommand(async (ct) =>
                {
                    await Task.Delay(500, ct).ConfigureAwait(false);
                    ct.ThrowIfCancellationRequested();
                    throw new ArgumentException(string.Empty);
                });
            asyncCommand.Cancel();
            return Assert.ThrowsAsync<ArgumentException>(() =>
                asyncCommand.ExecuteAsync()
            );
        }
    }
}