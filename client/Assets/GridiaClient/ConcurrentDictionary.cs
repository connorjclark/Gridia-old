namespace Gridia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ConcurrentDictionary<K, V>
    {
        #region Fields

        private readonly Dictionary<K, V> _dic = new Dictionary<K, V>();
        private readonly Object _lock = new Object();

        #endregion Fields

        #region Indexers

        public V this[K key]
        {
            get {
                return _dic[key];
            }
            set {
                lock (_lock) _dic[key] = value;
            }
        }

        #endregion Indexers

        #region Methods

        public bool HasKey(K key)
        {
            lock (_lock) return _dic.ContainsKey(key);
        }

        public void Remove(K key)
        {
            lock (_lock) _dic.Remove(key);
        }

        public void TryGetValue(K key, out V value)
        {
            lock (_lock) _dic.TryGetValue(key, out value);
        }

        public List<V> ValuesToList()
        {
            lock (_lock) {
                return _dic.Values.ToList();
            }
        }

        #endregion Methods
    }
}