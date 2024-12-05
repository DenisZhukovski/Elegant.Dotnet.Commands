using System.Threading.Tasks;

namespace Dotnet.Commands
{
	public interface ICommand : System.Windows.Input.ICommand
	{
		void RaiseCanExecuteChanged();
	}
	
	public interface IAsyncCommand : ICommand
	{
		Task<bool> ExecuteAsync(object? parameter);

		void Cancel();
	}

	public interface IAsyncCommand<in TParam> : IAsyncCommand
	{
		Task<bool> ExecuteAsync(TParam? parameter);
    }
}
