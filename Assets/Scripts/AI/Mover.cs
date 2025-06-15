using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed = 5f;
    public float accuracy = 1f;
    public float turnSpeed = 5f;

    private int m_currentWaypoint;
    private OctreeNode m_currentNode;
    private Vector3 m_destination;

    public OctreeBuilder builder;
    private Graph m_graph;

    public bool drawDebug;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (m_graph == null) return;

        if(m_graph.GetPathLength() == 0 || m_currentWaypoint >= m_graph.GetPathLength())
        {
            GetRandomDestination();
            return;
        }

        if (Vector3.Distance(m_graph.GetNode(m_currentWaypoint).bounds.center, transform.position) < accuracy)
        {
            m_currentWaypoint++;
            //Debug.Log($"Waypoint {m_currentWaypoint} reached");
        }

        if(m_currentWaypoint < m_graph.GetPathLength())
        {
            m_currentNode = m_graph.GetNode(m_currentWaypoint);
            m_destination = m_currentNode.bounds.center;

            Vector3 direction = (m_destination - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
            transform.Translate(transform.forward * speed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawDebug || m_graph == null || m_graph.GetPathLength() == 0) return;

        Color prevColor = Gizmos.color;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_graph.GetNode(0).bounds.center, 0.7f);
        Gizmos.DrawWireSphere(m_graph.GetNode(m_graph.GetPathLength() - 1).bounds.center, 0.7f);

        Gizmos.color = Color.yellow;

        for (int i = 0; i < m_graph.GetPathLength(); i++)
        {
            Gizmos.DrawWireSphere(m_graph.GetNode(i).bounds.center, 0.5f);

            if(i < m_graph.GetPathLength() - 1)
            {
                Gizmos.DrawLine(m_graph.GetNode(i).bounds.center, m_graph.GetNode(i + 1).bounds.center);
            }
        }

        Gizmos.color = prevColor;
    }

    public void Initialize()
    {
        m_graph = builder.waypoints;
        m_currentNode = GetClosestNode(transform.position);
        GetRandomDestination();
        m_currentWaypoint = 0;
    }

    void GetRandomDestination()
    {
        OctreeNode destinationNode;

        do
        {
            destinationNode = m_graph.nodes.ElementAt(Random.Range(0, m_graph.nodes.Count)).Key;
        } while (!m_graph.AStar(m_currentNode, destinationNode));

        m_currentWaypoint = 0;
    }

    OctreeNode GetClosestNode(Vector3 position)
    {
        OctreeNode closestNode = null;
        float closestDist = Mathf.Infinity;

        foreach (var nodePair in m_graph.nodes)
        {
            OctreeNode node = nodePair.Key;
            float dist = (node.bounds.center - position).sqrMagnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closestNode = node;
            }
        }

        return closestNode;
    }
}
