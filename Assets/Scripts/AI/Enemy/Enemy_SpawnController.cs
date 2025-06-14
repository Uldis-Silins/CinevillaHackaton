using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
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

    [SerializeField] private PlayerController m_playerController;
    [SerializeField] private float m_pointsPerBat = 0.7f;

    [SerializeField] private PassthroughCameraController m_passthroughCameraController;

    private int m_maxSpawnCount = 10;

    private List<Enemy_Controller_Base> m_spawnedEnemies = new();

    private SpawnerSelector m_spawnerSelector;
    private int m_curSpawnerIndex;

    private readonly float m_startSpawnDelay = 0.2f;

    private bool m_inGameOver;

    private Coroutine m_spawnRoutine;

    public IReadOnlyList<Enemy_Controller_Base> SpawnedEnemies => m_spawnedEnemies;

    private void Start()
    {
        m_spawnerSelector = LoopSpawnSelector;
        //StartCoroutine(RespawnEnemy(m_startSpawnDelay));
        m_spawnRoutine = StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {
        foreach (var enemy in m_spawnedEnemies)
        {
            if(enemy.LifeTime > 10f && enemy.CurrentRoute.PathDistanceType == PathDistanceType.Far)
            {
                enemy.SetRoute(GetEnemyRoute(PathDistanceType.Mid));
            }
            else if (enemy.LifeTime > 20f && enemy.CurrentRoute.PathDistanceType == PathDistanceType.Mid)
            {
                enemy.SetRoute(GetEnemyRoute(PathDistanceType.Close));
            }
            else if(enemy.LifeTime > 30f && !m_inGameOver)
            {
                m_passthroughCameraController.AnimateGameOver();
                m_inGameOver = true;
            }
        }

        if (m_inGameOver && m_spawnedEnemies.Count > 0)
        {
            StopCoroutine(m_spawnRoutine);

            for (int i = m_spawnedEnemies.Count - 1; i >= 0; i--)
            {
                Destroy(m_spawnedEnemies[i].gameObject);
            }

            m_spawnedEnemies.Clear();
        }

        if (!m_inGameOver)
        {
            int enemiesInClose = GetEnemyCountPerPath(PathDistanceType.Close);

            if (enemiesInClose > 0)
            {
                m_passthroughCameraController.SetCameraColor(PassthroughCameraController.CameraColorType.Close);
            }
            else
            {
                int enemiesInMid = GetEnemyCountPerPath(PathDistanceType.Mid);

                if (enemiesInMid > 0)
                {
                    m_passthroughCameraController.SetCameraColor(PassthroughCameraController.CameraColorType.Mid);
                }
                else
                {
                    m_passthroughCameraController.SetCameraColor(PassthroughCameraController.CameraColorType.Far);
                }
            }
        }
    }

    private IEnumerator RespawnEnemy(float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);
        if(m_inGameOver) yield break;
        Transform spawnPoint = m_spawnerSelector();
        Enemy_Controller_Base instance = Instantiate(m_enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        m_spawnedEnemies.Add(instance);

        var path = GetEnemyRoute(PathDistanceType.Far);
        instance.Initialize(m_paths[0]);

        instance.onKilled.AddListener(OnEnemyKilled);
    }

    private Transform RandomSpawnSelector()
    {
        m_curSpawnerIndex = Random.Range(0, m_spawnPoints.Length);
        return m_spawnPoints[m_curSpawnerIndex];
    }

    private Transform LoopSpawnSelector()
    {
        m_curSpawnerIndex = (m_curSpawnerIndex + 1) % m_spawnPoints.Length;
        return m_spawnPoints[m_curSpawnerIndex];
    }

    private int GetEnemyCountPerPath(PathDistanceType type)
    {
        int enemies = 0;

        foreach (var enemy in m_spawnedEnemies)
        {
            if(enemy.CurrentRoute.PathDistanceType == type)
            {
                enemies++;
            }
        }

        return enemies;
    }

    private EnemyRoute GetEnemyRoute(PathDistanceType type)
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

    private IEnumerator SpawnEnemies()
    {
        StartCoroutine(RespawnEnemy(1f));

        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (m_spawnedEnemies.Count < m_maxSpawnCount)
            {
                StartCoroutine(RespawnEnemy(1f));
            }
        }
    }

    private void OnEnemyKilled(Enemy_Controller_Base killedEnemy)
    {
        m_spawnedEnemies.Remove(killedEnemy);
        killedEnemy.onKilled.RemoveListener(OnEnemyKilled);

        m_playerController.score += m_pointsPerBat;

        if(m_playerController.score >= 13f)
        {
            m_inGameOver = true;
        }

        StartCoroutine(RespawnEnemy(1f));
    }
}