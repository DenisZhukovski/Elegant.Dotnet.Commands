using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class SafeCommandsTests : CommandsCommonTests
    {
        private readonly List<Exception> _exceptions = new();
        private readonly ICommands _commands;

        public SafeCommandsTests()
            : this(new Commands().Validated())
        {
        }
        
        protected SafeCommandsTests(ICommands commands)
            : base(commands.Safe(ex => false))
        {
            _commands = commands.Safe(ex => _exceptions.Add(ex));
        }

        [Fact]
        public void NoExceptionPropagatedFromAction()
        {
            Exception expected = null;
            try
            {
                _commands
                    .Command(() => throw new InvalidOperationException("Test"))
                    .Execute();
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public void NewSafeCommandsPriority()
        {
            Exception expected = null;
            _commands
                .Safe(ex => expected = ex)
                .Command(() => throw new InvalidOperationException("Test"))
                .Execute();
            
            Assert.IsType<InvalidOperationException>(expected);
            Assert.Empty(_exceptions);
        }
        
        [Fact]
        public void NewSafeCommandPriority()
        {
            Exception expected = null;
            _commands
                .Command(() => throw new InvalidOperationException("Test"))
                .Safe(ex => expected = ex)
                .Execute();
            
            Assert.IsType<InvalidOperationException>(expected);
            Assert.Empty(_exceptions);
        }
        
        [Fact]
        public async Task NewSafeCommandsButAsyncPriority()
        {
            Exception expected = null;
            await _commands
                .Safe(ex =>
                {
                    expected = ex;
                    return true;
                })
                .AsyncCommand(() => throw new InvalidOperationException("Test"))
                .ExecuteAsync(null);
            
            Assert.IsType<InvalidOperationException>(expected);
            Assert.Empty(_exceptions);
        }
        
        [Fact]
        public async Task NewSafeAsyncCommandPriority()
        {
            Exception expected = null;
            await _commands
                .AsyncCommand(() => throw new InvalidOperationException("Test"))
                .Safe(ex => expected = ex)
                .ExecuteAsync(null);
            
            Assert.IsType<InvalidOperationException>(expected);
            Assert.Empty(_exceptions);
        }
        
        [Fact]
        public void BaseSafeCommandsHandled()
        {
            Exception expected = null;
            _commands
                .Safe(ex =>
                {
                    expected = ex;
                    return false;
                })
                .Command(() => throw new InvalidOperationException("Test"))
                .Execute();
            
            Assert.IsType<InvalidOperationException>(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public void BaseSafeCommandHandled()
        {
            Exception expected = null;
            _commands
                .Command(() => throw new InvalidOperationException("Test"))
                .Safe((ex, name) =>
                {
                    expected = ex;
                    return false;
                })
                .Execute();
            
            Assert.IsType<InvalidOperationException>(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public async Task BaseSafeAsyncCommandsHandled()
        {
            Exception expected = null;
            await _commands
                .Safe(ex =>
                {
                    expected = ex;
                    return false;
                })
                .AsyncCommand(() => throw new InvalidOperationException("Test"))
                .ExecuteAsync();
            
            Assert.IsType<InvalidOperationException>(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public async Task BaseSafeAsyncCommandHandled()
        {
            Exception expected = null;
            await _commands
                .AsyncCommand(() => throw new InvalidOperationException("Test"))
                .Safe((ex, name) =>
                {
                    expected = ex;
                    return false;
                })
                .ExecuteAsync(null);
            
            Assert.IsType<InvalidOperationException>(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public void UnsafeThrownException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _commands
                    .Command(() => throw new InvalidOperationException("Test"))
                    .Unsafe()
                    .Execute()
            );
        }
        
        [Fact]
        public void UnsafeGenericThrownException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _commands
                    .Command<int>((i) => throw new InvalidOperationException("Test"))
                    .Unsafe()
                    .Execute(0)
            );
        }
        
        [Fact]
        public void HasErrorWhenExecute()
        {
           var command = _commands
                .Command(() => throw new InvalidOperationException("Test"));
           command.Execute();
           Assert.True(command.HasError());
        }
        
        [Fact]
        public void NoExceptionPropagatedFromCanExecuteDuringAction()
        {
            Exception expected = null;
            try
            {
                _commands
                    .Command(() => {}, () => throw new InvalidOperationException("Test"))
                    .Execute();
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public void NoExceptionPropagatedFromCanExecute()
        {
            Exception expected = null;
            try
            {
                _commands
                    .Command(
                        () => {}, 
                        () => throw new InvalidOperationException("Test"))
                    .CanExecute(null);
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public void UnsafeCanExecuteThrownException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _commands
                    .Command(
                        () => {}, 
                        () => throw new InvalidOperationException("Test"))
                    .Unsafe()
                    .CanExecute(0)
            );
        }
        
        [Fact]
        public void UnsafeGenericCanExecuteThrownException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _commands
                    .Command<int>(
                        (i) => {}, 
                        (i) => throw new InvalidOperationException("Test"))
                    .Unsafe()
                    .CanExecute(0)
            );
        }
        
        [Fact]
        public void HasErrorWhenCanExecute()
        {
            var command = _commands
                .Command(() => { }, () => throw new InvalidOperationException("Test"));
            command.CanExecute(null);
            Assert.True(command.HasError());
        }
        
        [Fact]
        public void NoExceptionPropagatedFromGenericAction()
        {
            Exception expected = null;
            try
            {
                _commands
                    .Command<int?>((p) => throw new InvalidOperationException("Test"))
                    .Execute();
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public void HasErrorWhenGenericExecute()
        {
            var command = _commands
                .Command<int>(_ => throw new InvalidOperationException("Test"));
            command.Execute(0);
            Assert.True(command.HasError());
        }
        
        [Fact]
        public void UnsafeDoesNotHaveError()
        {
            var command = _commands
                .Command<int>(_ => throw new InvalidOperationException("Test"));
            command.Execute(0);
            Assert.False(command.Unsafe().HasError());
        }
        
        [Fact]
        public void NoExceptionPropagatedFromCanExecuteDuringGenericExecution()
        {
            Exception expected = null;
            try
            {
                _commands
                    .Command<int?>(
                        (i) => {},
                        (i) => throw new InvalidOperationException("Test"))
                    .Execute();
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public void NoExceptionPropagatedFromCanExecuteGeneric()
        {
            Exception expected = null;
            try
            {
                _commands
                    .Command<int>(
                        (i) => {},
                        (i) => throw new InvalidOperationException("Test"))
                    .CanExecute(0);
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public void HasErrorWhenGenericCanExecute()
        {
            var command = _commands
                .Command<int>(
                    _ => { },
                    _ => throw new InvalidOperationException("Test")
                );
            command.CanExecute(0);
            Assert.True(command.HasError());
        }
        
        [Fact]
        public async Task NoExceptionPropagatedFromAsyncAction()
        {
            Exception expected = null;
            try
            {
                await _commands
                    .AsyncCommand(() => throw new InvalidOperationException("Test"))
                    .ExecuteAsync();
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public Task UnsafeAsyncThrowException()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                _commands
                    .AsyncCommand(() => throw new InvalidOperationException("Test"))
                    .Unsafe()
                    .ExecuteAsync()
            );
        }
        
        [Fact]
        public Task UnsafeGenericAsyncThrowException()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                _commands
                    .AsyncCommand<int>((i) => throw new InvalidOperationException("Test"))
                    .Unsafe()
                    .ExecuteAsync(0)
            );
        }
        
        [Fact]
        public Task TwoTimesUnsafeStillThrowException()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                _commands
                    .AsyncCommand<int>((i) => throw new InvalidOperationException("Test"))
                    .Unsafe().Unsafe()
                    .ExecuteAsync(0)
            );
        }
        
        [Fact]
        public async Task HasErrorWhenAsyncExecute()
        {
            var command = _commands
                .AsyncCommand(_ => throw new InvalidOperationException("Test"));
            await command.ExecuteAsync();
            Assert.True(command.HasError());
        }
        
        [Fact]
        public async Task HasErrorWhenGenericAsyncExecute()
        {
            var command = _commands
                .AsyncCommand<int>(_ => throw new InvalidOperationException("Test"));
            await command.ExecuteAsync(5);
            Assert.True(command.HasError());
        }
        
        [Fact]
        public async Task UnsafeAsyncDoesNotHaveError()
        {
            var command = _commands
                .AsyncCommand(_ => throw new InvalidOperationException("Test"));
            await command.ExecuteAsync();
            Assert.False(command.Unsafe().HasError());
        }
        
        [Fact]
        public async Task NoExceptionPropagatedFromAsyncActionCanExecuteDuringExecution()
        {
            Exception expected = null;
            try
            {
                await _commands
                    .AsyncCommand(
                        () => Task.CompletedTask,
                        () => throw new InvalidOperationException("Test"))
                    .ExecuteAsync();
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public async Task HasErrorWhenCanAsyncExecute()
        {
            var command = _commands
                .AsyncCommand(
                    () => Task.CompletedTask,
                    () => throw new InvalidOperationException("Test"));
            await command.ExecuteAsync();
            Assert.True(command.HasError());
        }
        
        [Fact]
        public async Task HasErrorWhenAsyncCanExecute()
        {
            var command = _commands
                .AsyncCommand<int?>(
                    (i) => Task.CompletedTask,
                    async (i) => throw new InvalidOperationException("Test"));
            await command.ExecuteAsync();
            Assert.True(command.HasError());
        }
        
        [Fact]
        public async Task NoExceptionPropagatedFromAsyncActionCanExecute()
        {
            Exception expected = null;
            try
            {
                _commands
                    .AsyncCommand(
                        () => Task.CompletedTask,
                        () => throw new InvalidOperationException("Test"))
                    .CanExecute(null);
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public async Task NoExceptionPropagatedFromAsyncCanExecuteDuringExecution()
        {
            Exception expected = null;
            try
            {
                await _commands
                    .AsyncCommand<int?>(
                        (i) => Task.CompletedTask,
                        async (i) => throw new InvalidOperationException("Test"))
                    .ExecuteAsync();
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
        
        [Fact]
        public async Task NoExceptionPropagatedFromAsyncGenericAction()
        {
            Exception expected = null;
            try
            {
                await _commands
                    .AsyncCommand<int?>((i) => throw new InvalidOperationException("Test"))
                    .ExecuteAsync();
            }
            catch (Exception e)
            {
                expected = e;
            }
            
            Assert.Null(expected);
            Assert.Single(_exceptions);
        }
    }
}