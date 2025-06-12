using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile_Base : MonoBehaviour
{
    [SerializeField] private float m_speed = 5f;
    [SerializeField] private float m_lifetime = 2.5f;

    private Rigidbody m_rb;

    private Vector3 m_prevPosition;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction)
    {
        m_prevPosition = transform.position;
        m_rb.AddForce(direction * m_speed, ForceMode.Impulse);
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
}