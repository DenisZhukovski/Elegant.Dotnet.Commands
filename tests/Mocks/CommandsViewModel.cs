using System.Windows.Input;

namespace Dotnet.Commands.UnitTests.Mocks
{
    public class CommandsViewModel
    {
        private readonly ICommands _commands;

        public CommandsViewModel(ICommands commands)
        {
            _commands = commands.Cached();
            Child = new ChildViewModel(_commands);
        }

        public int Quantity { get; private set; }

        public ChildViewModel Child { get; }
        
        public ICommand IncreaseQuantityCommand => _commands.Command(() =>
        {
            Quantity++;
        });
    }
}
