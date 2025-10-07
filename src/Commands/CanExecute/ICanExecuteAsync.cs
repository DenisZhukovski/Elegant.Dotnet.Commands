using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public interface ICanExecuteAsync<in TArgument>
    {
        Task<bool> CanExecute(TArgument? param);
    }
}