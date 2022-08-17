using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands.UnitTests.Mocks
{
    public static class CommandsExtensions
    {
        public static Task ExecuteAsync(this ICommands commands, Func<Task> handler, int count)
        {
            return commands.ExecuteAsync((ct) => handler(), count);
        }

        public static Task ExecuteAsync(this ICommands commands, Func<CancellationToken, Task> handler, int count)
        {
            var commandTasks = new List<Task>();
            for (var i = 0; i < count; i++)
            {
                commandTasks.Add(
                    commands
                        .AsyncCommand(handler)
                        .ExecuteAsync(null)
                );
            }

            return Task.WhenAll(commandTasks);
        }
    }
}
