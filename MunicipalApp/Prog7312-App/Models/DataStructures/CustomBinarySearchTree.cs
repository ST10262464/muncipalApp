using System;
using System.Collections.Generic;

namespace Prog7312_App.Models.DataStructures
{
    /**
     * CustomBinarySearchTree<TKey, TValue>
     * ------------------------------------
     * Implements a Binary Search Tree for efficient O(log n) storage and retrieval 
     * of ServiceRequest objects, keyed by ReferenceNumber.
     */
    public class CustomBinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class Node
        {
            public TKey Key;
            public TValue Value;
            public Node? Left;
            public Node? Right;

            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        private Node? _root;
        public int Count { get; private set; }

        public void Insert(TKey key, TValue value)
        {
            _root = Insert(_root, key, value);
            Count++;
        }

        private Node Insert(Node? node, TKey key, TValue value)
        {
            if (node == null)
            {
                return new Node(key, value);
            }

            int compare = key.CompareTo(node.Key);

            if (compare < 0)
            {
                node.Left = Insert(node.Left, key, value);
            }
            else if (compare > 0)
            {
                node.Right = Insert(node.Right, key, value);
            }
            else
            {
                // Key already exists, update value/handle as appropriate for your logic
                node.Value = value; 
                Count--; 
            }
            return node;
        }

        public TValue? Find(TKey key)
        {
            return Find(_root, key);
        }

        private TValue? Find(Node? node, TKey key)
        {
            if (node == null)
            {
                return default(TValue);
            }

            int compare = key.CompareTo(node.Key);

            if (compare < 0)
            {
                return Find(node.Left, key);
            }
            else if (compare > 0)
            {
                return Find(node.Right, key);
            }
            else
            {
                return node.Value;
            }
        }

        public List<TValue> InOrderTraversal()
        {
            var list = new List<TValue>();
            InOrderTraversal(_root, list);
            return list;
        }

        private void InOrderTraversal(Node? node, List<TValue> list)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left, list);
                list.Add(node.Value);
                InOrderTraversal(node.Right, list);
            }
        }
    }
}