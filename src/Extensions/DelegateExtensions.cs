using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    internal static class DelegateExtensions
    {
        public static Action Safe(this Action? action, Func<Exception, bool> onError)
        {
            return () =>
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    if (!onError(e))
                    {
                        throw;
                    }
                }
            };
        }
        
        public static Action<T> Safe<T>(this Action<T>? action, Func<Exception, bool> onError)
        {
            return p =>
            {
                try
                {
                    action?.Invoke(p);
                }
                catch (Exception e)
                {
                    if (!onError(e))
                    {
                        throw;
                    }
                }
            };
        }
        
        public static Func<T> Safe<T>(this Func<T>? action, Func<Exception, bool> onError)
        {
            return () =>
            {
                try
                {
                    return action != null 
                        ? action.Invoke()
                        : default;
                }
                catch (Exception e)
                {
                    if (!onError(e))
                    {
                        throw;
                    }

                    return default;
                }
            };
        }
        
        public static Func<T, TResult> Safe<T, TResult>(this Func<T, TResult>? action, Func<Exception, bool> onError)
        {
            return p =>
            {
                try
                {
                    return action != null ? action(p) : default;
                }
                catch (Exception e)
                {
                    if (!onError(e))
                    {
                        throw;
                    }
        
                    return default;
                }
            };
        }
        
        public static Func<CancellationToken, Task> Safe(this Func<CancellationToken, Task>? action, Func<Exception, bool> onError)
        {
            return async ct =>
            {
                try
                {
                    if (action != null)
                    {
                        await action(ct);
                    }
                }
                catch (Exception e)
                {
                    if (!onError(e))
                    {
                        throw;
                    }
                }
            };
        }
            
        public static Func<T, Task> Safe<T>(this Func<T, Task>? action, Func<Exception, bool> onError)
        {
            return async p =>
            {
                try
                {
                    if (action != null)
                    {
                        await action(p);
                    }
                }
                catch (Exception e)
                {
                    if (!onError(e))
                    {
                        throw;
                    }
                }
            };
        }
        
        public static Func<T, Task<T2>> Safe<T, T2>(this Func<T, Task<T2>>? action, Func<Exception, bool> onError)
        {
            return async p =>
            {
                try
                {
                    if (action != null)
                    {
                        return await action(p);
                    }

                    return default;
                }
                catch (Exception e)
                {
                    if (!onError(e))
                    {
                        throw;
                    }

                    return default;
                }
            };
        }
        
        public static Func<T, T2, Task> Safe<T, T2>(this Func<T, T2, Task>? action, Func<Exception, bool> onError)
        {
            return async (p, p2) =>
            {
                try
                {
                    if (action != null)
                    {
                        await action(p, p2);
                    }
                }
                catch (Exception e)
                {
                    if (!onError(e))
                    {
                        throw;
                    }
                }
            };
        }
    }
}