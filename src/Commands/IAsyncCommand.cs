using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
	public interface IAsyncCommand : ICommand
	{
		Task<bool> ExecuteAsync(object? parameter);

		void RaiseCanExecuteChanged();

		void Cancel();
	}

	public interface IAsyncCommand<in TParam> : IAsyncCommand
	{
		Task<bool> ExecuteAsync(TParam? parameter);
    }
}
