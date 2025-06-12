using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=gNmPmWR2vV4

/// <summary>
/// Octant, node of an octree
/// </summary>
#region OctreeNode
public class OctreeNode
{
    public List<OctreeObject> objects = new();

    private static int s_nextId;
    public readonly int id;

    public Bounds bounds;
    public Bounds[] childBounds = new Bounds[CHILD_COUNT];
    public OctreeNode[] children;

    public float minNodeSize;

    private const int CHILD_COUNT = 8;

    // Is a leaf node
    public bool IsLeaf { get { return children == null; } }

    public OctreeNode(Bounds bounds, float minNodeSize)
    {
        id = s_nextId++;

        this.bounds = bounds;
        this.minNodeSize = minNodeSize;

        Vector3 childSize = bounds.size * 0.5f;
        Vector3 centerOffset = bounds.size * 0.25f;
        Vector3 parentCenter = bounds.center;

        for (int i = 0; i < CHILD_COUNT; i++)
        {
            Vector3 childCenter = parentCenter;
            childCenter.x += centerOffset.x * ((i & 1) == 0 ? -1 : 1);
            childCenter.y += centerOffset.y * ((i & 2) == 0 ? -1 : 1);
            childCenter.z += centerOffset.z * ((i & 4) == 0 ? -1 : 1);
            childBounds[i] = new Bounds(childCenter, childSize);
        }
    }

    public void DrawNode()
    {
        Color prevColor = Gizmos.color;
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if(children != null)
        {
            foreach(var child in children)
            {
                if(child != null)
                {
                    child.DrawNode();
                }
            }
        }

        Gizmos.color = prevColor;
    }

    public void Add(GameObject obj)
    {
        Subdivide(new OctreeObject(obj));
    }

    /// <summary>
    /// Recursively subdivide bounds while object is intersecting child bounds
    /// </summary>
    private void Subdivide(OctreeObject obj)
    {
        // Min bounds size reached
        if(bounds.size.x <= minNodeSize)
        {
            objects.Add(obj);
            return;
        }

        if(children == null)
        {
            children = new OctreeNode[CHILD_COUNT];
        }

        bool isIntersecting = false;

        for (int i = 0; i < CHILD_COUNT; i++)
        {
            if (children[i] == null)
            {
                children[i] = new OctreeNode(childBounds[i], minNodeSize);
            }

            // Object intersects one of child bounds.
            // Subdivide this child further until no intersection or min size reched
            if (obj.Intersects(childBounds[i]))
            {
                children[i].Subdivide(obj);
                isIntersecting = true;
            }
        }

        if(!isIntersecting)
        {
            objects.Add(obj);
        }
    }
}
#endregion  // OctreeNode

/// <summary>
/// GameObject pointed to by octant
/// </summary>
#region OctreeObject
public class OctreeObject
{
    private Bounds m_bounds;

    public OctreeObject(GameObject gameObject)
    {
        m_bounds = gameObject.GetComponent<Collider>().bounds;
    }

    public bool Intersects(Bounds other)
    {
        return m_bounds.Intersects(other);
    }
}
#endregion  // OctreeObject

/// <summary>
/// Spatial (3D) tree structure of octants
/// </summary>
#region Octree
public class Octree
{
    /// <summary>
    /// Top most node of the tree
    /// </summary>
    public OctreeNode root;

    /// <summary>
    /// Bounds that fit the size of the tree
    /// </summary>
    public Bounds bounds;

    public Graph graph;
    private List<OctreeNode> m_emptyLeaves = new();

    /// <summary>
    /// Constructs new octree instance
    /// </summary>
    /// <param name="worldObjects">All GameObjects are inside the tree</param>
    /// <param name="minNodeSize">Minimum allowed size of a node</param>
    public Octree(GameObject[] worldObjects, float minNodeSize, Graph graph)
    {
        this.graph = graph;

        CalculateBounds(worldObjects);
        CreateTree(worldObjects, minNodeSize);
        GetEmptyLeaves(root);
        GetEdges();
    }

    void CreateTree(GameObject[] worldObjects, float minNodeSize)
    {
        root = new OctreeNode(bounds, minNodeSize);

        foreach (GameObject obj in worldObjects)
        {
            root.Add(obj);
        }
    }

    // Create bounds that fit all objects and make it square along bigest axis to be useable for octree
    private void CalculateBounds(GameObject[] worldObjects)
    {
        foreach (GameObject obj in worldObjects)
        {
            bounds.Encapsulate(obj.GetComponent<Collider>().bounds);
        }

        Vector3 size = Vector3.one * Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 0.5f;
        bounds.SetMinMax(bounds.center - size, bounds.center + size);
    }

    void GetEmptyLeaves(OctreeNode node)
    {
        if(node.IsLeaf && node.objects.Count == 0)
        {
            m_emptyLeaves.Add(node);
            graph.AddNode(node);
            return;
        }

        if (node.children == null) return;

        foreach (OctreeNode child in node.children) 
        { 
            GetEmptyLeaves(child); 
        }

        for (int i = 0; i < node.children.Length; i++)
        {
            for (int j = i + 1; j < node.children.Length; j++)
            {
                graph.AddEdge(node.children[i], node.children[j]);
            }
        }
    }

    void GetEdges()
    {
        foreach (var leaf in m_emptyLeaves)
        {
            Bounds inflatedBounds = new Bounds(leaf.bounds.center, leaf.bounds.size + (Vector3.one * (root.minNodeSize * 0.25f)));

            foreach (var otherLeaf in m_emptyLeaves)
            {
                if(inflatedBounds.Intersects(otherLeaf.bounds))
                {
                    graph.AddEdge(leaf, otherLeaf);
                }
            }
        }
    }
}
#endregion  // Octree