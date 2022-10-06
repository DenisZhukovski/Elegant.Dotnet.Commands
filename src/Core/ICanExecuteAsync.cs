using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public interface ICanExecuteAsync<TArgument>
    {
        Task<bool> CanExecute(TArgument? param);
    }
}