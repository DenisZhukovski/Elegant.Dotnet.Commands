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
    }
}
