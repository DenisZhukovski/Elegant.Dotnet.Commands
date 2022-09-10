using System;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public interface ICanExecuteAsync<TArgument>
    {
        event EventHandler? CanExecuteChanged;
        
        Task<bool> CanExecuteAsync(TArgument? param);

        void RaiseCanExecuteChanged();
    }
}