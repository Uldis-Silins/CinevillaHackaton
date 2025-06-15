using System.Collections;
using UnityEngine;

public class RoomAnchor : MonoBehaviour
{
    [SerializeField] private Transform[] m_spawnPoints;
    [SerializeField] private Enemy_SpawnController.EnemyRoute[] m_paths;

    private void Awake()
    {
        GameObject.FindFirstObjectByType<Enemy_SpawnController>().SetData(m_spawnPoints, m_paths);
    }
}