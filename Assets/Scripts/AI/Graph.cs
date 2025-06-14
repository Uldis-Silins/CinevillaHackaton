using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    private static int m_nextId;
    public readonly int id;

    public float f, g, h;
    public Node from;

    public List<Edge> edges = new();
    public OctreeNode octreeNode;

    public Node(OctreeNode octreeNode)
    {
        id = m_nextId++;
        this.octreeNode = octreeNode;
    }

    public override bool Equals(object obj) => obj is Node other && other.id == id;
    public override int GetHashCode() => id.GetHashCode();
}

public class Edge
{
    public readonly Node a, b;

    public Edge(Node a, Node b)
    {
        this.a = a;
        this.b = b;
    }

    public override bool Equals(object obj) => obj is Edge other && ((a == other.a &&  b == other.b) || (a == other.b && b == other.a));

    public override int GetHashCode() => a.GetHashCode() ^ b.GetHashCode();
}

public class Graph
{
    public readonly Dictionary<OctreeNode, Node> nodes = new();
    public readonly HashSet<Edge> edges = new();

    private List<Node> m_pathList = new();

    private const int MAX_ITERATIONS = 512;

    public bool AStar(OctreeNode startNode, OctreeNode endNode)
    {
        m_pathList.Clear();
        Node start = FindNode(startNode);
        Node end = FindNode(endNode);

        if (start == null)
        {
            Debug.LogError("Start node not found");
            return false;
        }

        if(end == null)
        {
            Debug.LogError("End node not found");
            return false;
        }

        SortedSet<Node> openSet = new(new NodeComparer());
        HashSet<Node> closedSet = new();
        int iterationCount = 0;

        start.g = 0;
        start.h = Heuristic(start, end);
        start.f = start.g + start.h;
        start.from = null;

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            if(++iterationCount > MAX_ITERATIONS)
            {
                Debug.LogWarning("Exceeded max iteration count");
                return false;
            }

            Node current = openSet.First();
            openSet.Remove(current);

            if(current.Equals(end))
            {
                ReconstructPath(current);
                return true;
            }

            closedSet.Add(current);

            foreach (Edge edge in current.edges)
            {
                Node neighbor = Equals(edge.a, current) ? edge.b : edge.a;

                if (closedSet.Contains(neighbor)) continue;

                float tentative_gScore = current.g + Heuristic(current, neighbor);

                if(tentative_gScore < neighbor.g || !openSet.Contains(neighbor))
                {
                    neighbor.g = tentative_gScore;
                    neighbor.h = Heuristic(current, neighbor);
                    neighbor.f = neighbor.g + neighbor.h;
                    neighbor.from = current;
                    openSet.Add(neighbor);
                }
            }
        }

        Debug.Log("No path found");
        return false;
    }

    void ReconstructPath(Node current)
    {
        while (current != null)
        {
            m_pathList.Add(current);
            current = current.from;
        }

        m_pathList.Reverse();
    }

    float Heuristic(Node a, Node b) => (a.octreeNode.bounds.center - b.octreeNode.bounds.center).sqrMagnitude;

    public int GetPathLength() => m_pathList.Count;

    public OctreeNode GetNode(int index)
    {
        if(m_pathList == null) return null;

        if(index < 0 || index >= m_pathList.Count) return null;

        return m_pathList[index].octreeNode;
    }

    public void AddNode(OctreeNode octreeNode)
    {
        if(!nodes.ContainsKey(octreeNode))
        {
            nodes.Add(octreeNode, new Node(octreeNode));
        }
    }

    public void AddEdge(OctreeNode a, OctreeNode b)
    {
        Node nodeA = FindNode(a);
        Node nodeB = FindNode(b);

        if (nodeA == null || nodeB == null) return;

        Edge edge = new Edge(nodeA, nodeB);

        if(edges.Add(edge))
        {
            nodeA.edges.Add(edge);
            nodeB.edges.Add(edge);
        }
    }

    Node FindNode(OctreeNode octreeNode)
    {
        nodes.TryGetValue(octreeNode, out Node node);
        return node;
    }

    public void DrawGraph()
    {
        Color prevColor = Gizmos.color;
        Gizmos.color = Color.red;

        foreach (Edge edge in edges)
        {
            Gizmos.DrawLine(edge.a.octreeNode.bounds.center, edge.b.octreeNode.bounds.center);
        }

        foreach (Node node in nodes.Values)
        {
            Gizmos.DrawSphere(node.octreeNode.bounds.center, 0.2f);
        }

        Gizmos.color = prevColor;
    }

    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            if(x == null || y == null) return 0;

            int compare = x.f.CompareTo(y.f);
            if (compare == 0) compare = x.id.CompareTo(y.id);
            return compare;
        }
    }
}
