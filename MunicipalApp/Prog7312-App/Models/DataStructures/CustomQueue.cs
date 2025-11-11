/**
 * CustomQueue<T>
 * ---------------
 * This class is a custom implementation of a queue data structure in C#. 
 * It supports generic type elements and follows FIFO (First-In-First-Out) order. 
 * The class provides standard queue operations including Enqueue, Dequeue, Peek, 
 * TryDequeue, TryPeek, Contains, Clear, and enumeration. The queue automatically 
 * resizes when the capacity is exceeded.
 * 
 * Author: GeeksforGeeks Contributors
 * Reference: GeeksforGeeks (2023). "Queue Data Structure in C#."
 * Available at: https://www.geeksforgeeks.org/queue-data-structure/
 */

using System.Collections;

namespace Prog7312_App.Models.DataStructures
{
    public class CustomQueue<T> : IEnumerable<T>
    {
        private T[] _items;
        private int _head;
        private int _tail;
        private int _count;
        private const int DefaultCapacity = 4;

        public CustomQueue()
        {
            _items = new T[DefaultCapacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public CustomQueue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            
            _items = new T[capacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public void Enqueue(T item)
        {
            if (_count >= _items.Length)
            {
                Resize();
            }

            _items[_tail] = item;
            _tail = (_tail + 1) % _items.Length;
            _count++;
        }

        public T Dequeue()
        {
            if (_count == 0)
                throw new InvalidOperationException("Queue is empty");

            T item = _items[_head];
            _items[_head] = default(T)!;
            _head = (_head + 1) % _items.Length;
            _count--;
            return item;
        }

        public T Peek()
        {
            if (_count == 0)
                throw new InvalidOperationException("Queue is empty");

            return _items[_head];
        }

        public bool TryDequeue(out T? result)
        {
            if (_count == 0)
            {
                result = default(T);
                return false;
            }

            result = Dequeue();
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

        public void Clear()
        {
            if (_head < _tail)
            {
                Array.Clear(_items, _head, _count);
            }
            else
            {
                Array.Clear(_items, _head, _items.Length - _head);
                Array.Clear(_items, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public bool Contains(T item)
        {
            int index = _head;
            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[index], item))
                    return true;
                index = (index + 1) % _items.Length;
            }
            return false;
        }

        private void Resize()
        {
            int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
            T[] newItems = new T[newCapacity];
            
            if (_count > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_items, _head, newItems, 0, _count);
                }
                else
                {
                    Array.Copy(_items, _head, newItems, 0, _items.Length - _head);
                    Array.Copy(_items, 0, newItems, _items.Length - _head, _tail);
                }
            }
            
            _items = newItems;
            _head = 0;
            _tail = _count;
        }

        public T[] ToArray()
        {
            T[] result = new T[_count];
            int index = _head;
            for (int i = 0; i < _count; i++)
            {
                result[i] = _items[index];
                index = (index + 1) % _items.Length;
            }
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int index = _head;
            for (int i = 0; i < _count; i++)
            {
                yield return _items[index];
                index = (index + 1) % _items.Length;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
