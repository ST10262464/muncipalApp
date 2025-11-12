using System.Collections.Generic;
using System.Linq;

namespace Prog7312_App.Models.DataStructures
{
    /**
     * CustomGraph<T>
     * -----------------
     * Implements a simple weighted, undirected graph using an Adjacency List.
     * Contains methods for Breadth-First Search (Graph Traversal) and a basic
     * Minimum Spanning Tree (MST) algorithm demonstration.
     */
    public class CustomGraph<T> where T : notnull
    {
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
            AdjacencyList[node1].Add((node2, weight));
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

        // 2. Minimum Spanning Tree (MST) - Conceptual demonstration using a simplified Kruskal's idea
        public List<(T Node1, T Node2, int Weight)> GetMinimumSpanningTree(T startNode)
        {
            var result = new List<(T, T, int)>();
            var allEdges = new List<(T Node1, T Node2, int Weight)>();
            
            // Collect all unique edges
            foreach (var node in AdjacencyList.Keys)
            {
                foreach (var (neighbor, weight) in AdjacencyList[node])
                {
                    if (node.GetHashCode() < neighbor.GetHashCode()) 
                    {
                        allEdges.Add((node, neighbor, weight));
                    }
                }
            }
            // Sort edges by weight
            allEdges = allEdges.OrderBy(e => e.Weight).ToList();

            var nodesInMst = new HashSet<T>();
            
            foreach (var edge in allEdges)
            {
                // Simplified MST logic: ensures connection to the network
                if (result.Count < AdjacencyList.Count - 1 && (!nodesInMst.Contains(edge.Node1) || !nodesInMst.Contains(edge.Node2)))
                {
                    result.Add(edge);
                    nodesInMst.Add(edge.Node1);
                    nodesInMst.Add(edge.Node2);
                }

                if (result.Count == AdjacencyList.Count - 1) break;
            }
            
            return result; 
        }
    }
}