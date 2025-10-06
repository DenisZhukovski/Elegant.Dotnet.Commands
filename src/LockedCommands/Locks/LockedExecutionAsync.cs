using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class LockedExecutionAsync<TParam>
    {
        private readonly Func<TParam?, CancellationToken, Task> _execute;
        private readonly bool _forceExecution;
        private readonly ICommandExecutionLock _commandExecutionLock;
        private long _lockIndex;

        public LockedExecutionAsync(
            Func<CancellationToken, Task> execute,
            bool forceExecution,
            ICommandExecutionLock commandExecutionLock,
            ref long lockIndex)
            : this(
                (_, ct) => execute(ct),
                forceExecution,
                commandExecutionLock,
                ref lockIndex
              )
        
        {
        }
        
        public LockedExecutionAsync(
            Func<TParam?, CancellationToken, Task> execute,
            bool forceExecution,
            ICommandExecutionLock commandExecutionLock,
            ref long lockIndex)
        {
            _execute = execute;
            _forceExecution = forceExecution;
            _commandExecutionLock = commandExecutionLock;
            _lockIndex = lockIndex;
        }
        
        public Task ExecuteAsync(CancellationToken ct)
        {
            return ExecuteAsync(default, ct);
        }
        
        public async Task ExecuteAsync(TParam param, CancellationToken ct)
        {
            if (_forceExecution)
            {
                await _execute(param, ct);
                return;
            }

            if (_commandExecutionLock.IsLocked)
            {
                return;
            }

            long currentLockIndex = 0;
            try
            {
                if (!_commandExecutionLock.TryLockExecution())
                {
                    return;
                }

                currentLockIndex = Interlocked.Increment(ref _lockIndex);
                await _execute(param, ct);
            }
            finally
            {
                if (Interlocked.Read(ref _lockIndex) == currentLockIndex)
                {
                    _ = _commandExecutionLock.FreeExecutionLock();
                }
            }
        }
    }
}