using System;
using System.Runtime.CompilerServices;

namespace Dotnet.Commands
{
    public class SafeCommand : ICommand
    {
        private readonly string? _name;
        internal readonly IErrorHandler OnError;
        internal readonly ICommand Command;

        public SafeCommand(ICommand command, IErrorHandler onError, [CallerMemberName] string? name = null)
        {
            Command = command;
            OnError = onError;
            _name = name;
        }
        
        public event EventHandler? CanExecuteChanged
        {
            add =>  Command.CanExecuteChanged += value;
            remove => Command.CanExecuteChanged -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            Command.RaiseCanExecuteChanged();
        }

        public Exception? Exception { get; private set; }
        
        public bool CanExecute(object? parameter)
        {
            try
            {
                Exception = null;
                return Command.CanExecute(parameter);
            }
            catch (Exception e)
            {
                Exception = e;
                if (!OnError.Handle(e, _name))
                {
                    throw;
                }

                return false;
            }
        }

        public void Execute(object? parameter)
        {
            try
            {
                Exception = null;
                Command.Execute(parameter);
            }
            catch (Exception e)
            {
                Exception = e;
                if (!OnError.Handle(e, _name))
                {
                    throw;
                }
            }
        }
    }
}
