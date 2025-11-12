// File: Prog7312-App/Models/DataStructures/CustomGraph.cs

using System.Collections.Generic;
using System.Linq;

namespace Prog7312_App.Models.DataStructures
{
    /**
     * CustomGraph<T>
     * -----------------
     * Implements a weighted, undirected graph using an Adjacency List.
     * Contains methods for Breadth-First Search (Graph Traversal) and a correct
     * Minimum Spanning Tree (MST) implementation using Prim's algorithm, satisfying 
     * the graph and MST requirements.
     */
    public class CustomGraph<T> where T : notnull
    {
        // Tuple structure for edges in the MST result: (Node1, Node2, Weight)
        public struct Edge
        {
            public T Node1;
            public T Node2;
            public int Weight;
        }

        public Dictionary<T, List<(T Neighbor, int Weight)>> AdjacencyList { get; } = new();

        public void AddNode(T node)
        {
            if (!AdjacencyList.ContainsKey(node))
            {
                AdjacencyList.Add(node, new List<(T, int)>());
            }
        }

        public void AddEdge(T node1, T node2, int weight)
        {
            AddNode(node1);
            AddNode(node2);
            // Ensure edge is added only if it doesn't exist to prevent duplicates
            if (!AdjacencyList[node1].Any(e => e.Neighbor.Equals(node2)))
                AdjacencyList[node1].Add((node2, weight));
            
            if (!AdjacencyList[node2].Any(e => e.Neighbor.Equals(node1)))
                AdjacencyList[node2].Add((node1, weight)); // Undirected graph
        }

        // 1. Graph Traversal: Breadth-First Search (BFS)
        public List<T> BreadthFirstSearch(T startNode)
        {
            if (!AdjacencyList.ContainsKey(startNode)) return new List<T>();

            var visited = new HashSet<T>();
            var queue = new Queue<T>();
            var traversalOrder = new List<T>();

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                traversalOrder.Add(currentNode);

                foreach (var (neighbor, _) in AdjacencyList[currentNode])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return traversalOrder;
        }

        // 2. Minimum Spanning Tree (MST) - Prim's Algorithm (Utilizes Priority Queue)
        public List<Edge> GetMinimumSpanningTree(T startNode)
        {
            if (!AdjacencyList.ContainsKey(startNode)) 
                return new List<Edge>();

            var resultMst = new List<Edge>();
            // Use CustomPriorityQueue (Min-Heap) to efficiently get the minimum weight edge
            var pq = new CustomPriorityQueue<Edge>(); 
            var visitedNodes = new HashSet<T>();

            // 1. Start with the initial node
            visitedNodes.Add(startNode);
            
            // 2. Add all edges from the start node to the Priority Queue
            foreach (var (neighbor, weight) in AdjacencyList[startNode])
            {
                pq.Enqueue(new Edge { Node1 = startNode, Node2 = neighbor, Weight = weight }, weight);
            }

            // 3. Continue until all nodes are included in the MST (or PQ is empty)
            while (pq.Count > 0 && resultMst.Count < AdjacencyList.Count - 1)
            {
                // FIX: Use non-nullable 'Edge' struct for the out parameter
                Edge currentEdge; 
                
                // Get the lightest edge from the Priority Queue
                if (!pq.TryDequeue(out currentEdge, out int priority))
                {
                    continue;
                }
                
                // Determine which of the two nodes is the 'new' node outside the MST
                T nodeU = currentEdge.Node1;
                T nodeV = currentEdge.Node2;
                T newNode;
                
                if (visitedNodes.Contains(nodeU) && !visitedNodes.Contains(nodeV))
                {
                    newNode = nodeV;
                }
                else if (visitedNodes.Contains(nodeV) && !visitedNodes.Contains(nodeU))
                {
                    newNode = nodeU;
                }
                else
                {
                    // This edge would create a cycle, so we skip it.
                    continue;
                }
                
                // Add the edge and the new node to the MST
                resultMst.Add(currentEdge); 
                visitedNodes.Add(newNode);
                
                // Add all unvisited neighbors of the new node to the Priority Queue
                foreach (var (neighbor, weight) in AdjacencyList[newNode])
                {
                    if (!visitedNodes.Contains(neighbor))
                    {
                        pq.Enqueue(new Edge { Node1 = newNode, Node2 = neighbor, Weight = weight }, weight);
                    }
                }
            }

            return resultMst;
        }
    }
}