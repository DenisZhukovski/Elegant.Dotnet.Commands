using System.Windows.Input;

namespace Dotnet.Commands.UnitTests.Mocks;

public class ChildViewModel
{
    private readonly ICommands _commands;

    public ChildViewModel(ICommands commands)
    {
        _commands = commands.Cached();
    }
    
    public int Quantity { get; private set; }

    public ICommand IncreaseQuantityCommand => _commands.Command(() =>
    {
        Quantity++;
    });
}