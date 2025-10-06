using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IList<Func<Exception, string?, bool>> _onErrorList;
        private readonly IList<Func<Exception, string?, Task<bool>>> _onErrorAsyncList;

        public ErrorHandler(IList<Func<Exception, string?, bool>> onErrorList)
            : this(onErrorList, new List<Func<Exception, string?, Task<bool>>>())
        {
        }
        
        public ErrorHandler(Func<Exception, string?, bool> onErrorList)
            : this(new List<Func<Exception, string?, bool>> { onErrorList })
        {
        }
        
        public ErrorHandler(Func<Exception, string?, Task<bool>> onErrorAsyncList)
            : this(new List<Func<Exception, string?, Task<bool>>> { onErrorAsyncList })
        {
        }
        
        public ErrorHandler(IList<Func<Exception, string?, Task<bool>>> onErrorAsyncList)
            : this(new  List<Func<Exception, string?, bool>>(), onErrorAsyncList)
        {
        }
        
        public ErrorHandler(
            IList<Func<Exception, string?, bool>> onErrorList,
            IList<Func<Exception, string?, Task<bool>>> onErrorAsyncList)
        {
            _onErrorList = onErrorList;
            _onErrorAsyncList = onErrorAsyncList;
        }
        
        public bool Handle(Exception exception, string? name)
        {
            var handled = InternalHandle(exception, name);
            if (!handled && _onErrorAsyncList.Any())
            {
                InternalHandleAsync(exception, name).FireAndForget();
                return true;
            }
            
            return handled;
        }
        
        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public async Task<bool> HandleAsync(Exception exception, string? name)
        {
            var handled = await InternalHandleAsync(exception, name);
            if (!handled)
            {
                return InternalHandle(exception, name);;
            }
            
            return handled;
        }

        public IErrorHandler Add(IList<Func<Exception, string?, bool>> onErrorList)
        {
            var newOnError = new List<Func<Exception, string?, bool>>(onErrorList);
            newOnError.AddRange(_onErrorList);
            return new ErrorHandler(newOnError, _onErrorAsyncList);
        }

        public IErrorHandler Add(IList<Func<Exception, string?, Task<bool>>> onErrorAsyncList)
        {
            var newOnError = new List<Func<Exception, string?, Task<bool>>>(onErrorAsyncList);
            newOnError.AddRange(_onErrorAsyncList);
            return new ErrorHandler(_onErrorList, newOnError);
        }

        private bool InternalHandle(Exception exception, string? name)
        {
            foreach (var onError in _onErrorList)
            {
                if (onError(exception, name))
                {
                    return true;
                }
            }

            return false;
        }
        
        private async Task<bool> InternalHandleAsync(Exception exception, string? name)
        {
            foreach (var onError in _onErrorAsyncList)
            {
                if (await onError(exception, name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
