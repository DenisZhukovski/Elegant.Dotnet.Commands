using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class SafeCommandsTests : CommandsCommonTests
    {
        private readonly List<Exception> _exceptions = new List<Exception>();
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