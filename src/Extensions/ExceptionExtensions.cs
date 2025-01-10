using System;
using System.Collections.Generic;

namespace Dotnet.Commands
{
    public static class ExceptionExtensions
    {
        internal static bool TryToHandle(this Exception exception, IList<Func<Exception, string?, bool>> onErrorList, string? name)
        {
            foreach (var onError in onErrorList)
            {
                if (onError(exception, name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}