/**
 * CustomPriorityQueue<T>
 * ----------------------
 * This class is a custom implementation of a priority queue (min-heap) data structure in C#. 
 * It supports generic type elements with a priority value. Elements are dequeued based on 
 * priority (lower value = higher priority). The class provides operations including Enqueue, 
 * Dequeue, Peek, TryDequeue, TryPeek, Clear, and enumeration. The queue uses a binary heap 
 * for efficient priority-based operations.
 * 
 * Author: GeeksforGeeks Contributors
 * Reference: GeeksforGeeks (2023). "Priority Queue using Binary Heap."
 * Available at: https://www.geeksforgeeks.org/priority-queue-using-binary-heap/
 */

using System.Collections;

namespace Prog7312_App.Models.DataStructures
{
    public class CustomPriorityQueue<T> : IEnumerable<(T Item, int Priority)>
    {
        private CustomDynamicArray<(T Item, int Priority)> _heap;

        public CustomPriorityQueue()
        {
            _heap = new CustomDynamicArray<(T Item, int Priority)>();
        }

        public int Count => _heap.Count;

        public bool IsEmpty => _heap.Count == 0;

        public void Enqueue(T item, int priority)
        {
            _heap.Add((item, priority));
            HeapifyUp(_heap.Count - 1);
        }

        public T Dequeue()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Priority queue is empty");

            var result = _heap[0].Item;
            
            // Move last element to root
            _heap[0] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);
            
            if (_heap.Count > 0)
            {
                HeapifyDown(0);
            }

            return result;
        }

        public T Peek()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Priority queue is empty");

            return _heap[0].Item;
        }

        public int PeekPriority()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Priority queue is empty");

            return _heap[0].Priority;
        }

        public bool TryDequeue(out T? result, out int priority)
        {
            if (_heap.Count == 0)
            {
                result = default(T);
                priority = 0;
                return false;
            }

            priority = _heap[0].Priority;
            result = Dequeue();
            return true;
        }

        public bool TryPeek(out T? result, out int priority)
        {
            if (_heap.Count == 0)
            {
                result = default(T);
                priority = 0;
                return false;
            }

            result = _heap[0].Item;
            priority = _heap[0].Priority;
            return true;
        }

        public void Clear()
        {
            _heap.Clear();
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                
                if (_heap[index].Priority >= _heap[parentIndex].Priority)
                    break;

                // Swap with parent
                var temp = _heap[index];
                _heap[index] = _heap[parentIndex];
                _heap[parentIndex] = temp;

                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                if (leftChild < _heap.Count && _heap[leftChild].Priority < _heap[smallest].Priority)
                    smallest = leftChild;

                if (rightChild < _heap.Count && _heap[rightChild].Priority < _heap[smallest].Priority)
                    smallest = rightChild;

                if (smallest == index)
                    break;

                // Swap with smallest child
                var temp = _heap[index];
                _heap[index] = _heap[smallest];
                _heap[smallest] = temp;

                index = smallest;
            }
        }

        public IEnumerator<(T Item, int Priority)> GetEnumerator()
        {
            return _heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
