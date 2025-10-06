using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class AsyncCommand<TArgument> : IAsyncCommand<TArgument>
    {
        private readonly Func<TArgument?, CancellationToken, Task> _action;
        private readonly ICanExecute<TArgument> _canExecute;
        private CancellationTokenSource? _cancellationTokenSource;

        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action,
            Func<TArgument?, Task<bool>>? canExecute = null)
            : this(action, new CommandCanExecuteAsync<TArgument>(canExecute))
        {
        }
        
        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action,
            Func<TArgument?, bool>? canExecute = null)
        : this(action, new CommandCanExecute<TArgument>(canExecute))
        {
        }
        
        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action, 
            ICanExecute<TArgument> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => _canExecute.CanExecuteChanged += value;
            remove => _canExecute.CanExecuteChanged -= value;
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null)
            {
                return _canExecute.CanExecute((TArgument?)parameter);
            }

            if (parameter is TArgument argument)
            {
                return _canExecute.CanExecute(argument);
            }
            return _canExecute.CanExecute((TArgument)Convert.ChangeType(parameter, typeof(TArgument)));
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                Execute((TArgument)parameter);
            }
            else
            {
                if (parameter is TArgument argument)
                {
                    Execute(argument);
                }
                else
                {
                    Execute((TArgument)Convert.ChangeType(parameter, typeof(TArgument)));
                }
            }
        }

        public void Execute(TArgument? parameter)
        {
            ExecuteAsync(parameter).FireAndForget();
        }

        public Task<bool> ExecuteAsync(object? parameter)
        {
            return parameter switch
            {
                null => ExecuteAsync((TArgument?)parameter),
                TArgument argument => ExecuteAsync(argument),
                _ => ExecuteAsync((TArgument)Convert.ChangeType(parameter, typeof(TArgument)))
            };
        }

        public async Task<bool> ExecuteAsync(TArgument? parameter)
        {
            var canExecute = await CanExecuteAsync(parameter).ConfigureAwait(false);
            if (canExecute)
            {
                // Cancel the previous operation, if one is pending
                _cancellationTokenSource?.Cancel();

                var cancellationTokenSource = _cancellationTokenSource = new CancellationTokenSource();
                await _action(parameter, cancellationTokenSource.Token).ConfigureAwait(false);
            }

            return canExecute;
        }

        public virtual void RaiseCanExecuteChanged()
        {
            _canExecute.RaiseCanExecuteChanged();
        }
        
        private async Task<bool> CanExecuteAsync(TArgument? parameter)
        {
            bool canExecute;
            if (_canExecute is ICanExecuteAsync<TArgument> canExecuteAsync)
            {
                canExecute = await canExecuteAsync.CanExecute(parameter);
            }
            else
            {
                canExecute = _canExecute.CanExecute(parameter);
            }

            return canExecute;
        }
    }
}
