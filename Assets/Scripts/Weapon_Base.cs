using System.Collections.Generic;
using UnityEngine;

public class Weapon_Base : MonoBehaviour
{
    [SerializeField] private HandInputController m_handInput;
    [SerializeField] private Transform m_muzzle;
    [SerializeField] private PassthroughCameraController m_passthroughCameraController;

    [SerializeField] private Transform m_cameraTransform;

    [SerializeField] private Enemy_SpawnController m_enemySpawnController;

    [SerializeField] private Projectile_Base m_projectilePrefab;

    private Enemy_Controller_Base m_currentClosest;

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
        if (m_currentClosest) m_currentClosest.aimTarget.SetActive(false);
        m_currentClosest = GetClosestEnemy();
        if (m_currentClosest) m_currentClosest.aimTarget.SetActive(true);
    }

    private void Fire()
    {
        Projectile_Base instance = Instantiate<Projectile_Base>(m_projectilePrefab, m_muzzle.position, Quaternion.identity);

        float t = 0f;// m_currentClosest == null ? 0f : Vector3.Angle(m_muzzle.position - m_muzzle.up, m_currentClosest.transform.position - m_muzzle.position);
        Vector3 aimAssist = m_currentClosest == null ? -m_muzzle.up : Vector3.Lerp(-m_muzzle.up, -m_muzzle.up + (m_currentClosest.transform.position - m_muzzle.position).normalized, t / 360f);
        Vector3 projectileVelocity = aimAssist;
        instance.Initialize(projectileVelocity, m_currentClosest);
        m_passthroughCameraController.AnimateFire();
    }

    private Enemy_Controller_Base GetClosestEnemy()
    {
        float closestDist = float.MaxValue;
        Enemy_Controller_Base closestEnemy = null;

        foreach (Enemy_Controller_Base enemy in m_enemySpawnController.SpawnedEnemies)
        {
            if (Mathf.Abs(Vector3.SignedAngle(m_cameraTransform.forward, enemy.transform.position - m_cameraTransform.position, m_cameraTransform.up)) > 15f) continue;
            Ray ray = new Ray(m_cameraTransform.position, m_cameraTransform.forward);
            float distance = Vector3.Cross(ray.direction, enemy.transform.position - ray.origin).magnitude;

            if (distance < closestDist)
            {
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