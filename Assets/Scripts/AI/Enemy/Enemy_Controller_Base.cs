using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Controller_Base : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Enemy_Controller_Base> onKilled;

    [SerializeField] private Mover m_mover;

    public void Initialize(Enemy_SpawnController.EnemyRoute route)
    {
        m_mover.builder = route.Path;
    }

    public void Kill()
    {
        onKilled?.Invoke(this);
        Destroy(gameObject);
    }
}