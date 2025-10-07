using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public interface IErrorHandler
    {
        bool Handle(Exception exception, string? name);

        Task<bool> HandleAsync(Exception exception, string? name);

        IErrorHandler Add(IList<Func<Exception, string?, bool>> onErrorList);
        
        IErrorHandler Add(IList<Func<Exception, string?, Task<bool>>> onErrorAsyncList);
    }
}
