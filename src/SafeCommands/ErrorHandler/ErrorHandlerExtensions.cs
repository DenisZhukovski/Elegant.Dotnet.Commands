using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public static class ErrorHandlerExtensions
    {
        public static IErrorHandler Add(this IErrorHandler handler, Func<Exception, string?, Task<bool>> onError)
        {
            return handler.Add(
                new List<Func<Exception, string?, Task<bool>>>
                {
                    onError
                }
            );
        }
        
        public static IErrorHandler Add(this IErrorHandler handler, Func<Exception, string?, bool> onError)
        {
            return handler.Add(
                new List<Func<Exception, string?, bool>>
                {
                    onError
                }
            );
        }
    }
}
