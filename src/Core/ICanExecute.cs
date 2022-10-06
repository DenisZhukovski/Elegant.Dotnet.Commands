using System;

namespace Dotnet.Commands
{
    public interface ICanExecute<TArgument>
    {
        event EventHandler? CanExecuteChanged;
        
        bool CanExecute(TArgument? param);

        void RaiseCanExecuteChanged();
    }
}