/**
 * CustomStack<T>
 * ---------------
 * This class is a custom implementation of a stack data structure in C#. 
 * It supports generic type elements and follows LIFO (Last-In-First-Out) order. 
 * The class provides standard stack operations including Push, Pop, Peek, 
 * TryPop, TryPeek, Contains, Clear, and enumeration. The stack automatically 
 * resizes when the capacity is exceeded.
 * 
 * Author: GeeksforGeeks Contributors
 * Reference: GeeksforGeeks (2023). "Stack Data Structure in C#."
 * Available at: https://www.geeksforgeeks.org/stack-data-structure-in-c-sharp/
 */

using System.Collections;

namespace Prog7312_App.Models.DataStructures
{
    public class CustomStack<T> : IEnumerable<T>
    {
        private T[] _items;
        private int _count;
        private const int DefaultCapacity = 4;

        public CustomStack()
        {
            _items = new T[DefaultCapacity];
            _count = 0;
        }

        public CustomStack(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            
            _items = new T[capacity];
            _count = 0;
        }

        public int Count => _count;

        public void Push(T item)
        {
            if (_count >= _items.Length)
            {
                Resize();
            }
            _items[_count] = item;
            _count++;
        }

        public T Pop()
        {
            if (_count == 0)
                throw new InvalidOperationException("Stack is empty");

            _count--;
            T item = _items[_count];
            _items[_count] = default(T)!; // Clear reference
            return item;
        }

        public T Peek()
        {
            if (_count == 0)
                throw new InvalidOperationException("Stack is empty");

            return _items[_count - 1];
        }

        public bool TryPop(out T? result)
        {
            if (_count == 0)
            {
                result = default(T);
                return false;
            }

            result = Pop();
            return true;
        }

        public bool TryPeek(out T? result)
        {
            if (_count == 0)
            {
                result = default(T);
                return false;
            }

            result = Peek();
            return true;
        }

        public bool IsEmpty => _count == 0;

        public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _items[i] = default(T)!;
            }
            _count = 0;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return true;
            }
            return false;
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

        public T[] ToArray()
        {
            T[] result = new T[_count];
            for (int i = 0; i < _count; i++)
            {
                result[_count - 1 - i] = _items[i]; // Reverse order for stack semantics
            }
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            // Iterate from top to bottom (LIFO order)
            for (int i = _count - 1; i >= 0; i--)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
