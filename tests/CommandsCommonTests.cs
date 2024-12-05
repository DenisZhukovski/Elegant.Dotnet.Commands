using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Dotnet.Commands.UnitTests.Mocks;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public abstract class CommandsCommonTests
    {
        private readonly ICommands _commands;

        protected CommandsCommonTests(ICommands commands)
        {
            _commands = commands;
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
        public void CanExecuteFalse()
        {
            Assert.False(
                _commands
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void CanExecuteTrue()
        {
            Assert.True(
                _commands
                    .Command(() => { }, () => true)
                    .CanExecute(null)
            );
        }
        
        [Fact]
        public void CommandCanExecuteTrue_WhenNull()
        {
            Assert.True(
                _commands
                    .Command(() => { })
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
            Assert.True(
                _commands
                    .AsyncCommand(() => Task.CompletedTask, () => true)
                    .CanExecute(null)
            );
        }
        
        [Fact]
        public void AsyncCommandCanExecuteTrue_WhenNull()
        {
            Assert.True(
                _commands
                    .AsyncCommand(() => Task.CompletedTask)
                    .CanExecute(null)
            );
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
        
        [Theory]
        [InlineData(13)]
        [InlineData("13")]
        public void CommandExecutionWithParsedParameter(object parameter)
        {
            int expectedNumber = 0;
            _commands
                .Command<int>(number => expectedNumber = number)
                .Execute(parameter);
            Assert.Equal(13, expectedNumber);
        }
        
        [Theory]
        [InlineData(13)]
        [InlineData((object)"13")]
        public void CommandExecutionWithParsedParameterUsingInterface(object parameter)
        {
            int expectedNumber = 0;
            ICommand command = _commands.Command<int>(number => expectedNumber = number);
            command.Execute(parameter);
            Assert.Equal(13, expectedNumber);
        }

        [Fact]
        public async Task AsyncCommandExecutionWithParameter()
        {
            int expectedNumber = 0;
            Assert.True(
                await _commands
                    .AsyncCommand<int>(async number =>
                    {
                        await Task.Delay(300);
                        expectedNumber = number;
                    })
                    .ExecuteAsync(13)
            );
            Assert.Equal(13, expectedNumber);
        }
        
        [Theory]
        [InlineData(13)]
        [InlineData((object)"13")]
        public async Task AsyncCommandExecutionWithParsedParameter(object parameter)
        {
            int expectedNumber = 0;
            Assert.True(
                await _commands
                    .AsyncCommand<int>(async number =>
                    {
                        await Task.Delay(300);
                        expectedNumber = number;
                    })
                    .ExecuteAsync(parameter)
            );
            Assert.Equal(13, expectedNumber);
        }
        
        [Fact]
        public void AsyncCommandAsRegularCommandExecutionWithParsedParameter()
        {
            int expectedNumber = 0;
            _commands
                .AsyncCommand<int>(number =>
                {
                    expectedNumber = number;
                    return Task.CompletedTask;
                })
                .Execute("13");
            Assert.Equal(13, expectedNumber);
        }

        [Fact]
        public async Task AsyncCommandExecution()
        {
            int executionsCount = 0;
            Assert.True(
                await _commands
                    .AsyncCommand(async () =>
                    {
                        await Task.Delay(500);
                        executionsCount++;
                    })
                    .ExecuteAsync(null)
            );
            
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
            Assert.True(
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
                    .ExecuteAsync(12)
            );
            Assert.Equal(1, executionsCount);
        }
        
        [Theory]
        [InlineData(13)]
        [InlineData("13")]
        public void CanExecuteAsyncWithParsedParameter(object parameter)
        {
            int expectedNumber = 0;
            Assert.True(
                _commands
                    .Command<int>(number => { }, number =>
                    {
                        expectedNumber = number;
                        return true;
                    })
                    .CanExecute(parameter)
            );
            
            Assert.Equal(
                13,
                expectedNumber
            );
        }
        
        [Fact]
        public async Task CanExecuteAsyncFalse()
        {
            int executionsCount = 0;
            Assert.False(
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
                    .ExecuteAsync(12)
            );
            Assert.Equal(0, executionsCount);
        }

        [Fact]
        public void CommandThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                _commands
                    .Command(() => throw new ArgumentException(string.Empty))
                    .Execute()
            );
        }

        [Fact]
        public void CommandWithParameterThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                _commands
                    .Command<int>((number) => throw new ArgumentException(string.Empty))
                    .Execute(13)
            );
        }

        [Fact]
        public Task AsyncCommand_ThrowsException()
        {
            return Assert.ThrowsAsync<ArgumentException>(() =>
                _commands
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
                _commands
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
            Assert.False(
                await _commands
                    .AsyncCommand(async () =>
                    {
                        await Task.Delay(500);
                        commandExecuted = true;
                    }, () => false)
                    .ExecuteAsync(null)
            );
            Assert.False(commandExecuted);
        }

        [Fact]
        public async Task CanExecuteFalse_CommandNotExecuted()
        {
            var commandExecuted = false;
            await _commands
                .Command(() => {
                    commandExecuted = true;
                }, () => false)
                .ExecuteAsync(null);

            Assert.False(commandExecuted);
        }

        [Fact]
        public async Task CanExecuteIsNull_CommandExecuted()
        {
            var commandExecuted = false;
            await _commands
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
        public virtual void TwoDifferentCommands()
        {
            var commands = _commands;
            Assert.NotEqual(
                commands.Command(() => { }),
                commands.Command(() => { })
            );
        }

        [Fact]
        public virtual void TwoDifferentAsyncCommands()
        {
            var commands = _commands;
            Assert.NotEqual(
                commands.AsyncCommand(() => Task.CompletedTask),
                commands.AsyncCommand(() => Task.CompletedTask)
            );
        }

        [Fact]
        public Task AsyncCommand_CancelThrowsException()
        {
            var asyncCommand = _commands
                .AsyncCommand(async ct =>
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
            var asyncCommand = _commands
                .AsyncCommand<int>(
                    async (param, ct) =>
                    {
                        await Task.Delay(3000).ConfigureAwait(false);
                        ct.ThrowIfCancellationRequested();
                        throw new ArgumentException(string.Empty);
                    },
                    (Func<int, bool>)null
                );
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
            var asyncCommand = _commands
                .AsyncCommand(async (ct) =>
                {
                    await Task.Delay(500).ConfigureAwait(false);
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
