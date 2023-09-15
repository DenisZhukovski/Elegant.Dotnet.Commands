using System;
using System.Collections.Generic;

namespace Dotnet.Commands
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TValue> getFunc)
        {
            if (!dictionary.ContainsKey(key))
            {
                lock (dictionary)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, getFunc());
                    }
                }
            }

            return dictionary[key];
        }
    }
}
