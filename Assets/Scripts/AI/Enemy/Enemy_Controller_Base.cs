using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Controller_Base : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Enemy_Controller_Base> onKilled;

    [SerializeField] private Mover m_mover;

    [SerializeField] private Transform m_ragdollPrefab;
    [SerializeField] private Transform m_rootForRagdoll;

    public AudioSource batAudio;
    public AudioClip[] batClips;

    public GameObject aimTarget;

    private float m_spawnTime;

    public float LifeTime => Time.time - m_spawnTime;
    public Enemy_SpawnController.EnemyRoute CurrentRoute {  get; private set; }

    public void Initialize(Enemy_SpawnController.EnemyRoute route)
    {
        aimTarget.SetActive(false);
        SetRoute(route);
        m_spawnTime = Time.time;

        batAudio.clip = batClips[Random.Range(0, batClips.Length)];
        batAudio.Play();
    }

    public void SetRoute(Enemy_SpawnController.EnemyRoute route)
    {
        CurrentRoute = route;
        m_mover.builder = route.Path;
        m_mover.Initialize();
    }

    public void Kill()
    {
        var instance = Instantiate(m_ragdollPrefab, transform.position, transform.rotation);
        instance.GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        CopyTransform(m_rootForRagdoll, instance);

        onKilled?.Invoke(this);
        Destroy(gameObject);
    }

    private void CopyTransform(Transform myRoot, Transform fromRoot)
    {
        myRoot.SetPositionAndRotation(fromRoot.transform.position, fromRoot.transform.rotation);

        if (myRoot.childCount == fromRoot.childCount)
        {
            for (int i = 0; i < myRoot.childCount; i++)
            {
                CopyTransform(myRoot.GetChild(i), fromRoot.GetChild(i));
            }
        }
    }

}