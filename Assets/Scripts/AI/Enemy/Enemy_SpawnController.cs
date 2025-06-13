using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Enemy_SpawnController : MonoBehaviour
{
    public enum PathDistanceType { None = -1, Far, Mid, Close }

    private delegate Transform SpawnerSelector();

    [Serializable]
    public class EnemyRoute
    {
        [SerializeField] private PathDistanceType m_pathDistanceType;
        [SerializeField] private OctreeBuilder m_pathBuilder;

        public PathDistanceType PathDistanceType => m_pathDistanceType;
        public OctreeBuilder Path => m_pathBuilder;
    }

    [SerializeField] private Enemy_Controller_Base m_enemyPrefab;
    [SerializeField] private Transform[] m_spawnPoints;
    [SerializeField] private EnemyRoute[] m_paths;

    private List<Enemy_Controller_Base> m_spawnedEnemies = new();

    private SpawnerSelector m_spawnerSelector;

    private readonly float m_startSpawnDelay = 0.2f;

    private void Start()
    {
        m_spawnerSelector = RandomSpawnSelector;
        StartCoroutine(RespawnEnemy(m_startSpawnDelay));
    }

    private IEnumerator RespawnEnemy(float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);
        Transform spawnPoint = m_spawnerSelector();
        Enemy_Controller_Base instance = Instantiate(m_enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        m_spawnedEnemies.Add(instance);

        var path = GetEnemyPath(PathDistanceType.Far);
        instance.Initialize(m_paths[0]);

        instance.onKilled.AddListener(OnEnemyKilled);
    }

    private Transform RandomSpawnSelector()
    {
        return m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
    }

    private EnemyRoute GetEnemyPath(PathDistanceType type)
    {
        foreach (var path in m_paths)
        {
            if(path.PathDistanceType == type)
            {
                return path;
            }
        }

        throw new NullReferenceException("Path of type " + type.ToString() + " not found");
    }

    private void OnEnemyKilled(Enemy_Controller_Base killedEnemy)
    {
        m_spawnedEnemies.Remove(killedEnemy);
        killedEnemy.onKilled.RemoveListener(OnEnemyKilled);

        StartCoroutine(RespawnEnemy(1f));
    }
}