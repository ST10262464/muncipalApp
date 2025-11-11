/**
 * CustomSortedDictionary<TKey, TValue>
 * -------------------------------------
 * This class is a custom implementation of a sorted dictionary using a binary search tree (BST).
 * It maintains key-value pairs in sorted order based on the keys. The class provides operations
 * including Add, Remove, TryGetValue, ContainsKey, Clear, and in-order traversal. Keys must
 * implement IComparable<TKey> for sorting.
 * 
 * Author: GeeksforGeeks Contributors
 * Reference: GeeksforGeeks (2023). "Binary Search Tree in C#."
 * Available at: https://www.geeksforgeeks.org/binary-search-tree-data-structure/
 */

using System.Collections;

namespace Prog7312_App.Models.DataStructures
{
    public class CustomSortedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        private class Node
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public Node? Left { get; set; }
            public Node? Right { get; set; }

            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Left = null;
                Right = null;
            }
        }

        private Node? _root;
        private int _count;

        public CustomSortedDictionary()
        {
            _root = null;
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

            bool added = false;
            _root = AddRecursive(_root, key, value, ref added);
            if (added)
                _count++;
        }

        private Node AddRecursive(Node? node, TKey key, TValue value, ref bool added)
        {
            if (node == null)
            {
                added = true;
                return new Node(key, value);
            }

            int comparison = key.CompareTo(node.Key);

            if (comparison < 0)
            {
                node.Left = AddRecursive(node.Left, key, value, ref added);
            }
            else if (comparison > 0)
            {
                node.Right = AddRecursive(node.Right, key, value, ref added);
            }
            else
            {
                // Key already exists, update value
                node.Value = value;
                added = false;
            }

            return node;
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            if (key == null)
            {
                value = default;
                return false;
            }

            Node? node = FindNode(_root, key);
            if (node != null)
            {
                value = node.Value;
                return true;
            }

            value = default;
            return false;
        }

        private Node? FindNode(Node? node, TKey key)
        {
            if (node == null)
                return null;

            int comparison = key.CompareTo(node.Key);

            if (comparison < 0)
                return FindNode(node.Left, key);
            else if (comparison > 0)
                return FindNode(node.Right, key);
            else
                return node;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                return false;

            bool removed = false;
            _root = RemoveRecursive(_root, key, ref removed);
            if (removed)
                _count--;
            return removed;
        }

        private Node? RemoveRecursive(Node? node, TKey key, ref bool removed)
        {
            if (node == null)
                return null;

            int comparison = key.CompareTo(node.Key);

            if (comparison < 0)
            {
                node.Left = RemoveRecursive(node.Left, key, ref removed);
            }
            else if (comparison > 0)
            {
                node.Right = RemoveRecursive(node.Right, key, ref removed);
            }
            else
            {
                removed = true;

                // Node with only one child or no child
                if (node.Left == null)
                    return node.Right;
                else if (node.Right == null)
                    return node.Left;

                // Node with two children: Get the inorder successor (smallest in the right subtree)
                Node minNode = FindMin(node.Right);
                node.Key = minNode.Key;
                node.Value = minNode.Value;
                node.Right = RemoveRecursive(node.Right, minNode.Key, ref removed);
                removed = false; // Already counted
            }

            return node;
        }

        private Node FindMin(Node node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        public CustomDynamicArray<TKey> Keys
        {
            get
            {
                var keys = new CustomDynamicArray<TKey>();
                InOrderTraversal(_root, (key, value) => keys.Add(key));
                return keys;
            }
        }

        public CustomDynamicArray<TValue> Values
        {
            get
            {
                var values = new CustomDynamicArray<TValue>();
                InOrderTraversal(_root, (key, value) => values.Add(value));
                return values;
            }
        }

        private void InOrderTraversal(Node? node, Action<TKey, TValue> action)
        {
            if (node == null)
                return;

            InOrderTraversal(node.Left, action);
            action(node.Key, node.Value);
            InOrderTraversal(node.Right, action);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var items = new CustomDynamicArray<KeyValuePair<TKey, TValue>>();
            InOrderTraversal(_root, (key, value) => items.Add(new KeyValuePair<TKey, TValue>(key, value)));
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
