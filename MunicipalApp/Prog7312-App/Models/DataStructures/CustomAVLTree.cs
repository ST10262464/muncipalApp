using System;
using System.Collections.Generic;

namespace Prog7312_App.Models.DataStructures
{
    /**
     * CustomAVLTree<TKey, TValue>
     * --------------------------
     * Implements a Self-Balancing Binary Search Tree (AVL Tree) for maintaining 
     * O(log n) efficiency in the worst case, fulfilling the advanced tree requirement.
     */
    public class CustomAVLTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class Node
        {
            public TKey Key;
            public TValue Value;
            public int Height;
            public Node? Left;
            public Node? Right;

            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Height = 1; // New node is initially at height 1
            }
        }

        private Node? _root;
        public int Count { get; private set; }

        // --- Utility Methods ---

        private int GetHeight(Node? node)
        {
            return node == null ? 0 : node.Height;
        }

        private int GetBalanceFactor(Node? node)
        {
            return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
        }

        private void UpdateHeight(Node node)
        {
            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
        }

        // --- Rotations ---

        // Right Rotation (LL Case)
        private Node RotateRight(Node y)
        {
            Node x = y.Left!;
            Node T2 = x.Right;

            // Perform rotation
            x.Right = y;
            y.Left = T2;

            // Update heights
            UpdateHeight(y);
            UpdateHeight(x);

            return x; // New root
        }

        // Left Rotation (RR Case)
        private Node RotateLeft(Node x)
        {
            Node y = x.Right!;
            Node T2 = y.Left;

            // Perform rotation
            y.Left = x;
            x.Right = T2;

            // Update heights
            UpdateHeight(x);
            UpdateHeight(y);

            return y; // New root
        }


        // --- Insert Method (Recursive) ---

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
                // Key already exists, update value (or handle as duplicate)
                node.Value = value; 
                Count--; // Decrement count since no new node was added
                return node;
            }

            // 1. Update height of current node
            UpdateHeight(node);

            // 2. Get balance factor
            int balance = GetBalanceFactor(node);

            // 3. Perform rotations if unbalanced (Balance is outside [-1, 1])

            // Left Left Case (LL)
            if (balance > 1 && key.CompareTo(node.Left!.Key) < 0)
            {
                return RotateRight(node);
            }

            // Right Right Case (RR)
            if (balance < -1 && key.CompareTo(node.Right!.Key) > 0)
            {
                return RotateLeft(node);
            }

            // Left Right Case (LR)
            if (balance > 1 && key.CompareTo(node.Left!.Key) > 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right Left Case (RL)
            if (balance < -1 && key.CompareTo(node.Right!.Key) < 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        // --- Find Method (Recursive) ---

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

        // Additional utility: In-Order Traversal for testing/display
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