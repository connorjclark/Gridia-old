using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gridia
{
    public class ConcurrentDictionary<K, V>
    {
        private Dictionary<K, V> _dic = new Dictionary<K, V>();
        private Object _lock = new Object();

        public V this[K key] { 
            get {
                return _dic[key];
            }
            set {
                lock (_lock) _dic[key] = value;
            }
        }

        public void Remove(K key) {
            lock (_lock) _dic.Remove(key);
        }

        public bool HasKey(K key)
        {
            lock (_lock) return _dic.ContainsKey(key);
        }

        public void TryGetValue(K key, out V value)
        {
            lock (_lock) _dic.TryGetValue(key, out value);
        }

        public List<V> ValuesToList() {
            lock (_lock) {
                return _dic.Values.ToList();
            }
        }
    }
}
