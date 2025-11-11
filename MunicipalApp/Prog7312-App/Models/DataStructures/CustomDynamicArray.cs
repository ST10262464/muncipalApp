
/**
 * CustomDynamicArray<T>
 * ----------------------
 * This class is a custom implementation of a dynamic array data structure in C#.
 * It mimics the behavior of List<T> by allowing dynamic resizing, element access,
 * searching, and iteration. It supports methods such as Add, Remove, RemoveAt,
 * IndexOf, Contains, Clear, and LINQ-like operations such as FirstOrDefault and Where.
 *
 * Reference: GeeksforGeeks (2023). "Dynamic Array in C#."
 * Available at: https://www.geeksforgeeks.org/dynamic-array-in-c-sharp/
 * Author: GeeksforGeeks Contributors
 */

using System.Collections;

namespace Prog7312_App.Models.DataStructures
{
    public class CustomDynamicArray<T> : IEnumerable<T>
    {
        private T[] _items;
        private int _count;
        private const int DefaultCapacity = 4;

        public CustomDynamicArray()
        {
            _items = new T[DefaultCapacity];
            _count = 0;
        }

        public CustomDynamicArray(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            
            _items = new T[capacity];
            _count = 0;
        }

        public int Count => _count;
        public int Capacity => _items.Length;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException();
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException();
                _items[index] = value;
            }
        }

        public void Add(T item)
        {
            if (_count >= _items.Length)
            {
                Resize();
            }
            _items[_count] = item;
            _count++;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();

            for (int i = index; i < _count - 1; i++)
            {
                _items[i] = _items[i + 1];
            }
            _count--;
            _items[_count] = default(T)!;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return i;
            }
            return -1;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _items[i] = default(T)!;
            }
            _count = 0;
        }

        public T? FirstOrDefault(Func<T, bool> predicate)
        {
            for (int i = 0; i < _count; i++)
            {
                if (predicate(_items[i]))
                    return _items[i];
            }
            return default(T);
        }

        public CustomDynamicArray<T> Where(Func<T, bool> predicate)
        {
            var result = new CustomDynamicArray<T>();
            for (int i = 0; i < _count; i++)
            {
                if (predicate(_items[i]))
                    result.Add(_items[i]);
            }
            return result;
        }

        private void Resize()
        {
            int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
            T[] newItems = new T[newCapacity];
            
            for (int i = 0; i < _count; i++)
            {
                newItems[i] = _items[i];
            }
            
            _items = newItems;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] ToArray()
        {
            T[] result = new T[_count];
            for (int i = 0; i < _count; i++)
            {
                result[i] = _items[i];
            }
            return result;
        }
    }
}
