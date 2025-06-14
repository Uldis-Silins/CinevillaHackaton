using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile_Base : MonoBehaviour
{
    [SerializeField] private float m_speed = 5f;
    [SerializeField] private float m_lifetime = 2.5f;
    [SerializeField] private float m_collisionRadius = 2f;

    public ParticleSystem explosion;

    [SerializeField] private LayerMask m_hitLayer;

    private Rigidbody m_rb;

    private Enemy_Controller_Base m_targetEnemy;
    private Vector3 m_startPosition;
    private Vector3 m_startVelocity;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, m_collisionRadius, m_hitLayer);
        
        if(hits.Length > 0)
        {
            hits[0].GetComponent<Enemy_Controller_Base>().Kill();
            Instantiate(explosion, transform.position, Quaternion.identity);
            AutoDestroy();
        }

        if (m_targetEnemy != null && Vector3.Distance(m_startPosition, m_targetEnemy.transform.position) > Vector3.Distance(m_startPosition, transform.position))
        {
            Vector3 dir = (m_targetEnemy.transform.position - transform.position).normalized * m_speed;
            Vector3 seek = Vector3.Lerp(m_startVelocity, dir, 0.5f * Time.deltaTime);
            m_rb.linearVelocity = seek;
            m_startVelocity = (m_startVelocity + seek).normalized * m_speed;
        }
    }

    public void Initialize(Vector3 direction, Enemy_Controller_Base target)
    {
        m_targetEnemy = target;
        m_startVelocity = direction * m_speed;
        m_rb.linearVelocity = m_startVelocity;
        m_startPosition = transform.position;
        
        Invoke(nameof(AutoDestroy), m_lifetime);
    }

    private void AutoDestroy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Room"))
        {
            Debug.Log("Room hit");
        }
    }

    private void OnDrawGizmos()
    {
        Color temp = Gizmos.color;
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, m_collisionRadius);

        Gizmos.color = temp;
    }
}