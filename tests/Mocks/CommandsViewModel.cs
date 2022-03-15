using System.Windows.Input;

namespace Dotnet.Commands.UnitTests.Mocks
{
    public class CommandsViewModel
    {
        private readonly CachedCommands _cachedCommands;

        public CommandsViewModel(ICommands commands)
        {
            _cachedCommands = commands.Cached();
        }

        public int Quantity { get; private set; }

        public ICommand IncreaseQuantityCommand => _cachedCommands.Command(() =>
        {
            Quantity++;
        });
    }
}
