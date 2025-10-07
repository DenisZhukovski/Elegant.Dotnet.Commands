using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class MultiErrorHandler : IErrorHandler
    {
        private readonly IEnumerable<IErrorHandler> _handlers;

        public MultiErrorHandler(params IErrorHandler[] handlers)
            : this((IEnumerable<IErrorHandler>)handlers)
        {
        }
        
        public MultiErrorHandler(IEnumerable<IErrorHandler> handlers)
        {
            _handlers = handlers;
        }
        
        public bool Handle(Exception exception, string? name)
        {
            foreach (var errorHandler in _handlers)
            {
                if (errorHandler.Handle(exception, name))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> HandleAsync(Exception exception, string? name)
        {
            foreach (var errorHandler in _handlers)
            {
                if (await errorHandler.HandleAsync(exception, name))
                {
                    return true;
                }
            }

            return false;
        }

        public IErrorHandler Add(IList<Func<Exception, string?, bool>> onErrorList)
        {
            var newList = new List<IErrorHandler>()
            {
                new ErrorHandler(onErrorList)
            };
            newList.AddRange(_handlers);
            return new MultiErrorHandler(newList);
        }

        public IErrorHandler Add(IList<Func<Exception, string?, Task<bool>>> onErrorAsyncList)
        {
            var newList = new List<IErrorHandler>()
            {
                new ErrorHandler(onErrorAsyncList)
            };
            newList.AddRange(_handlers);
            return new MultiErrorHandler(newList);
        }
    }
}
