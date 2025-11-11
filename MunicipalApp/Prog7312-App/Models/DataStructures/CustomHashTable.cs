/**
 * CustomHashTable<TKey, TValue>
 * -----------------------------
 * This class is a custom implementation of a hash table in C# using separate chaining 
 * (buckets) to handle collisions. It supports generic key-value pairs, dynamic resizing, 
 * and common hash table operations including Add, Remove, TryGetValue, ContainsKey, 
 * and enumeration over keys and values.
 * 
 * Author: GeeksforGeeks Contributors
 * Reference: GeeksforGeeks (2023). "Hash Table in C#."
 * Available at: https://www.geeksforgeeks.org/hash-table-in-c-sharp/
 */

using System.Collections;

namespace Prog7312_App.Models.DataStructures
{
    
    public class CustomHashTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        private const int DefaultCapacity = 16;
        private const double LoadFactorThreshold = 0.75;
        
        private Bucket<TKey, TValue>[] _buckets;
        private int _count;
        private int _capacity;

        public CustomHashTable() : this(DefaultCapacity) { }

        public CustomHashTable(int capacity)
        {
            _capacity = capacity;
            _buckets = new Bucket<TKey, TValue>[_capacity];
            _count = 0;
        }

        public int Count => _count;

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue? value) && value != null)
                    return value;
                throw new KeyNotFoundException($"Key '{key}' not found");
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if ((double)_count / _capacity >= LoadFactorThreshold)
            {
                Resize();
            }

            int index = GetBucketIndex(key);
            var bucket = _buckets[index];

            if (bucket == null)
            {
                _buckets[index] = new Bucket<TKey, TValue>();
                bucket = _buckets[index];
            }

            if (bucket.AddOrUpdate(key, value))
            {
                _count++;
            }
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            if (key == null)
            {
                value = default;
                return false;
            }

            int index = GetBucketIndex(key);
            var bucket = _buckets[index];

            if (bucket != null)
            {
                return bucket.TryGetValue(key, out value);
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                return false;

            int index = GetBucketIndex(key);
            var bucket = _buckets[index];

            if (bucket != null && bucket.Remove(key))
            {
                _count--;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _buckets = new Bucket<TKey, TValue>[_capacity];
            _count = 0;
        }

        public CustomDynamicArray<TKey> Keys
        {
            get
            {
                var keys = new CustomDynamicArray<TKey>();
                foreach (var kvp in this)
                {
                    keys.Add(kvp.Key);
                }
                return keys;
            }
        }

        public CustomDynamicArray<TValue> Values
        {
            get
            {
                var values = new CustomDynamicArray<TValue>();
                foreach (var kvp in this)
                {
                    values.Add(kvp.Value);
                }
                return values;
            }
        }

        private int GetBucketIndex(TKey key)
        {
            int hash = key.GetHashCode();
            return Math.Abs(hash) % _capacity;
        }

        private void Resize()
        {
            var oldBuckets = _buckets;
            _capacity *= 2;
            _buckets = new Bucket<TKey, TValue>[_capacity];
            _count = 0;

            foreach (var bucket in oldBuckets)
            {
                if (bucket != null)
                {
                    foreach (var kvp in bucket.GetItems())
                    {
                        Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var bucket in _buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket.GetItems())
                    {
                        yield return item;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    
    // Bucket class for handling collisions in the hash table using separate chaining
    internal class Bucket<TKey, TValue>
        where TKey : notnull
    {
        private readonly CustomLinkedList<KeyValuePair<TKey, TValue>> _items;

        public Bucket()
        {
            _items = new CustomLinkedList<KeyValuePair<TKey, TValue>>();
        }

        public bool AddOrUpdate(TKey key, TValue value)
        {
            var existingItem = _items.FirstOrDefault(item => EqualityComparer<TKey>.Default.Equals(item.Key, key));
            
            if (!EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(existingItem, default))
            {
                // Update existing item
                _items.Remove(existingItem);
                _items.Add(new KeyValuePair<TKey, TValue>(key, value));
                return false; // Didn't add new item
            }
            else
            {
                // Add new item
                _items.Add(new KeyValuePair<TKey, TValue>(key, value));
                return true; // Added new item
            }
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            var item = _items.FirstOrDefault(kvp => EqualityComparer<TKey>.Default.Equals(kvp.Key, key));
            
            if (!EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(item, default))
            {
                value = item.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool Remove(TKey key)
        {
            var item = _items.FirstOrDefault(kvp => EqualityComparer<TKey>.Default.Equals(kvp.Key, key));
            
            if (!EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(item, default))
            {
                return _items.Remove(item);
            }

            return false;
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> GetItems()
        {
            return _items;
        }
    }
}
