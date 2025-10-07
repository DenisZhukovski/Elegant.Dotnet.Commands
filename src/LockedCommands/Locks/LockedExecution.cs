using System;

namespace Dotnet.Commands
{
    public class LockedExecution<TParam>
    {
        private readonly Action<TParam> _execute;
        private readonly bool _forceExecution;
        private readonly ICommandExecutionLock _commandExecutionLock;

        public LockedExecution(
            Action execute,
            bool forceExecution,
            ICommandExecutionLock commandExecutionLock)
            : this(_ => execute(), forceExecution, commandExecutionLock)
        {
        }
        
        public LockedExecution(
            Action<TParam> execute,
            bool forceExecution,
            ICommandExecutionLock commandExecutionLock)
        {
            _execute = execute;
            _forceExecution = forceExecution;
            _commandExecutionLock = commandExecutionLock;
        }

        public void Execute()
        {
            Execute(default);
        }
        
        public void Execute(TParam param)
        {
            if (_forceExecution)
            {
                _execute(param);
                return;
            }

            if (_commandExecutionLock.IsLocked)
            {
                return;
            }

            try
            {
                if (!_commandExecutionLock.TryLockExecution())
                {
                    return;
                }

                _execute(param);
            }
            finally
            {
                _commandExecutionLock.FreeExecutionLock();
            }
        }
    }
}