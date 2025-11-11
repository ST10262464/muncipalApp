/**
 * CustomSet<T>
 * ------------
 * This class is a custom implementation of a set data structure in C#. 
 * It stores unique elements and provides efficient operations for Add, Remove, Contains,
 * Union, Intersection, and Difference. The set uses a hash table internally for O(1)
 * average-case performance on basic operations.
 * 
 * Author: GeeksforGeeks Contributors
 * Reference: GeeksforGeeks (2023). "HashSet in C#."
 * Available at: https://www.geeksforgeeks.org/c-sharp-hashset-class/
 */

using System.Collections;

namespace Prog7312_App.Models.DataStructures
{
    public class CustomSet<T> : IEnumerable<T>
        where T : notnull
    {
        private CustomHashTable<T, bool> _items;

        public CustomSet()
        {
            _items = new CustomHashTable<T, bool>();
        }

        public int Count => _items.Count;

        public bool Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items.ContainsKey(item))
                return false;

            _items.Add(item, true);
            return true;
        }

        public bool Remove(T item)
        {
            if (item == null)
                return false;

            return _items.Remove(item);
        }

        public bool Contains(T item)
        {
            if (item == null)
                return false;

            return _items.ContainsKey(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public CustomSet<T> Union(CustomSet<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var result = new CustomSet<T>();
            
            foreach (var item in this)
            {
                result.Add(item);
            }

            foreach (var item in other)
            {
                result.Add(item);
            }

            return result;
        }

        public CustomSet<T> Intersection(CustomSet<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var result = new CustomSet<T>();

            foreach (var item in this)
            {
                if (other.Contains(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public CustomSet<T> Difference(CustomSet<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var result = new CustomSet<T>();

            foreach (var item in this)
            {
                if (!other.Contains(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public bool IsSubsetOf(CustomSet<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (var item in this)
            {
                if (!other.Contains(item))
                    return false;
            }

            return true;
        }

        public bool IsSupersetOf(CustomSet<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return other.IsSubsetOf(this);
        }

        public T[] ToArray()
        {
            var array = new T[Count];
            int index = 0;
            foreach (var item in this)
            {
                array[index++] = item;
            }
            return array;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var kvp in _items)
            {
                yield return kvp.Key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
