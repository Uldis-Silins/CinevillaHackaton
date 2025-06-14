using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Weapon_Base : MonoBehaviour
{
    [SerializeField] private HandInputController m_handInput;
    [SerializeField] private Transform m_muzzle;
    [SerializeField] private PassthroughCameraController m_passthroughCameraController;

    [SerializeField] private Enemy_SpawnController m_enemySpawnController;

    [SerializeField] private Projectile_Base m_projectilePrefab;

    private void OnEnable()
    {
        m_handInput.onHandFired.AddListener(OnHandInputFired);
    }

    private void OnDisable()
    {
        m_handInput.onHandFired.RemoveListener(OnHandInputFired);
    }

    private void Update()
    {
        
    }

    private void Fire()
    {
        Projectile_Base instance = Instantiate<Projectile_Base>(m_projectilePrefab, m_muzzle.position, m_muzzle.rotation);
        Enemy_Controller_Base closestEnemy = GetClosestEnemy();
        float t = closestEnemy  == null ? 0f : Vector3.Angle(m_muzzle.position - m_muzzle.up, closestEnemy.transform.position - m_muzzle.position);
        Debug.Log(t / 360f);
        Vector3 aimAssist = closestEnemy == null ? -m_muzzle.up : Vector3.Lerp(-m_muzzle.up, (closestEnemy.transform.position - m_muzzle.position).normalized, t / 360f);
        Vector3 projectileVelocity = aimAssist;
        instance.Initialize(projectileVelocity);
        m_passthroughCameraController.AnimateFire();
    }

    private Enemy_Controller_Base GetClosestEnemy()
    {
        float closestDist = float.MaxValue;
        Enemy_Controller_Base closestEnemy = null;

        foreach (Enemy_Controller_Base enemy in m_enemySpawnController.SpawnedEnemies)
        {
            Ray ray = new Ray(m_muzzle.transform.position, m_muzzle.forward);
            float distance = Vector3.Cross(ray.direction, enemy.transform.position - ray.origin).magnitude;

            if (distance < closestDist)
            {
                Debug.Log(Vector3.Angle(ray.origin + ray.direction, enemy.transform.position));
                closestDist = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void OnHandInputFired(float timestamp)
    {
        Fire();
    }
}