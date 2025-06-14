using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeBuilder : MonoBehaviour
{
    public GameObject[] objects;
    public float minNodeSize;

    public bool drawDebug;

    private Octree m_octree;

    public readonly Graph waypoints = new();

    private void Awake()
    {
        m_octree = new Octree(objects, minNodeSize, waypoints);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !drawDebug) return;

        Color prevColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_octree.bounds.center, m_octree.bounds.size);

        Gizmos.color = prevColor;

        m_octree.root.DrawNode();
        waypoints.DrawGraph();
    }
}
