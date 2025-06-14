using System.Collections.Generic;
using UnityEngine;

public class Weapon_Base : MonoBehaviour
{
    [SerializeField] private HandInputController m_handInput;
    [SerializeField] private Transform m_muzzle;
    [SerializeField] private PassthroughCameraController m_passthroughCameraController;

    [SerializeField] private Projectile_Base m_projectilePrefab;

    private void OnEnable()
    {
        m_handInput.onHandFired.AddListener(OnHandInputFired);
    }

    private void OnDisable()
    {
        m_handInput.onHandFired.RemoveListener(OnHandInputFired);
    }

    private void Fire()
    {
        Projectile_Base instance = Instantiate<Projectile_Base>(m_projectilePrefab, m_muzzle.position, m_muzzle.rotation);
        instance.Initialize(-m_muzzle.up);
        m_passthroughCameraController.AnimateFire();
    }

    private void OnHandInputFired(float timestamp)
    {
        Fire();
    }
}