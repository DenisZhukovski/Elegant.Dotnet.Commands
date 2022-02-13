using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public static class CommandsExtensions
    {
        public static CachedCommands Cached(this ICommands commands)
        {
            return new CachedCommands(commands);
        }

        public static ICommands Validated(this ICommands commands)
        {
            return new ValidatedCommands(commands);
        }

        public static void Execute(this ICommand command)
        {
            command.Execute(null);
        }

        public static Task ExecuteAsync(this IAsyncCommand command)
        {
            return command.ExecuteAsync(null);
        }

        public static void Execute(this IAsyncCommand command)
        {
            command.Execute(null);
        }

        public static Task ExecuteAsync(this ICommand command, object parameter)
        {
            if (command is IAsyncCommand asyncCommand)
            {
                return asyncCommand.ExecuteAsync(parameter);
            }
            command.Execute(parameter);
            return Task.CompletedTask;
        }
    }
}
