/**
 * CustomLinkedList<T>
 * -------------------
 * This class is a custom implementation of a doubly linked list in C#. 
 * It supports adding, removing, indexing, searching, clearing, and iterating over elements. 
 * Additional methods include FirstOrDefault and Where for LINQ-like operations.
 * 
 * Author: GeeksforGeeks Contributors
 * Reference: GeeksforGeeks (2023). "Doubly Linked List in C#."
 * Available at: https://www.geeksforgeeks.org/doubly-linked-list-in-c-sharp/
 */

using System.Collections;

namespace Prog7312_App.Models.DataStructures
{
    public class CustomLinkedList<T> : IEnumerable<T>
    {
        private Node<T>? _head;
        private Node<T>? _tail;
        private int _count;

        public CustomLinkedList()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        public int Count => _count;

        public void Add(T item)
        {
            var newNode = new Node<T>(item);
            
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail!.Next = newNode;
                newNode.Previous = _tail;
                _tail = newNode;
            }
            
            _count++;
        }

        public bool Remove(T item)
        {
            var current = _head;
            
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Data, item))
                {
                    RemoveNode(current);
                    return true;
                }
                current = current.Next;
            }
            
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();

            var node = GetNodeAt(index);
            RemoveNode(node);
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException();
                return GetNodeAt(index).Data;
            }
            set
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException();
                GetNodeAt(index).Data = value;
            }
        }

        public bool Contains(T item)
        {
            var current = _head;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Data, item))
                    return true;
                current = current.Next;
            }
            return false;
        }

        public void Clear()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        public T? FirstOrDefault(Func<T, bool> predicate)
        {
            var current = _head;
            while (current != null)
            {
                if (predicate(current.Data))
                    return current.Data;
                current = current.Next;
            }
            return default(T);
        }

        public CustomLinkedList<T> Where(Func<T, bool> predicate)
        {
            var result = new CustomLinkedList<T>();
            var current = _head;
            
            while (current != null)
            {
                if (predicate(current.Data))
                    result.Add(current.Data);
                current = current.Next;
            }
            
            return result;
        }

        private Node<T> GetNodeAt(int index)
        {
            Node<T>? current;
            
            if (index < _count / 2)
            {
                // Start from head
                current = _head;
                for (int i = 0; i < index; i++)
                {
                    current = current!.Next;
                }
            }
            else
            {
                // Start from tail
                current = _tail;
                for (int i = _count - 1; i > index; i--)
                {
                    current = current!.Previous;
                }
            }
            
            return current!;
        }

        private void RemoveNode(Node<T> node)
        {
            if (node.Previous != null)
                node.Previous.Next = node.Next;
            else
                _head = node.Next;

            if (node.Next != null)
                node.Next.Previous = node.Previous;
            else
                _tail = node.Previous;

            _count--;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = _head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] ToArray()
        {
            T[] result = new T[_count];
            var current = _head;
            int index = 0;
            
            while (current != null)
            {
                result[index++] = current.Data;
                current = current.Next;
            }
            
            return result;
        }
    }

   
    // Node class for the doubly linked list
    
    internal class Node<T>
    {
        public T Data { get; set; }
        public Node<T>? Next { get; set; }
        public Node<T>? Previous { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
            Previous = null;
        }
    }
}
