using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TransplantingJsonConverter.Internal
{
    internal sealed class OrderedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _map;
        private readonly List<KeyValuePair<TKey, TValue>> _kvplist;

        public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> kvplist, IEqualityComparer<TKey> keyComparer)
        {
            if (kvplist is null)
            {
                throw new ArgumentNullException(nameof(kvplist));
            }
            if (keyComparer is null)
            {
                throw new ArgumentNullException(nameof(keyComparer));
            }

            _kvplist = kvplist.ToList();
            _map = _kvplist.ToDictionary(x => x.Key, x => x.Value, keyComparer);
        }

        public TValue this[TKey key] => ((IReadOnlyDictionary<TKey, TValue>)_map)[key];

        public IEnumerable<TKey> Keys => _kvplist.Select(x => x.Key).ToList().AsReadOnly();

        public IEnumerable<TValue> Values => _kvplist.Select(x => x.Value).ToList().AsReadOnly();

        public int Count => ((IReadOnlyCollection<KeyValuePair<TKey, TValue>>)_map).Count;

        public bool ContainsKey(TKey key)
        {
            return ((IReadOnlyDictionary<TKey, TValue>)_map).ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _kvplist.GetEnumerator();

        public bool TryGetValue(TKey key, out TValue value)
        {
            return ((IReadOnlyDictionary<TKey, TValue>)_map).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() => _kvplist.GetEnumerator();
    }
}
