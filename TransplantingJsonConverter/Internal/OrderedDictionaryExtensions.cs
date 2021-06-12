using System.Collections.Generic;

namespace TransplantingJsonConverter.Internal
{
    internal static class OrderedDictionaryExtensions
    {
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvplist, IEqualityComparer<TKey> keyComparer)
        {
            return new OrderedDictionary<TKey, TValue>(kvplist, keyComparer);
        }
    }
}
