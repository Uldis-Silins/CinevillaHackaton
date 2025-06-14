using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Controller_Base : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Enemy_Controller_Base> onKilled;

    [SerializeField] private Mover m_mover;

    public GameObject aimTarget;

    private float m_spawnTime;

    public float LifeTime => Time.time - m_spawnTime;
    public Enemy_SpawnController.EnemyRoute CurrentRoute {  get; private set; }

    public void Initialize(Enemy_SpawnController.EnemyRoute route)
    {
        aimTarget.SetActive(false);
        SetRoute(route);
        m_spawnTime = Time.time;
    }

    public void SetRoute(Enemy_SpawnController.EnemyRoute route)
    {
        CurrentRoute = route;
        m_mover.builder = route.Path;
        m_mover.Initialize();
    }

    public void Kill()
    {
        onKilled?.Invoke(this);
        Destroy(gameObject);
    }
}