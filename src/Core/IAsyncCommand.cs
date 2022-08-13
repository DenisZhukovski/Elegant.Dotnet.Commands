using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
	public interface IAsyncCommand : ICommand
	{
		Task ExecuteAsync(object? parameter);

	    void RaiseCanExecuteChanged();
	}

	public interface IAsyncCommand<in TParam> : IAsyncCommand
	{
		Task ExecuteAsync(TParam parameter);
    }
}
