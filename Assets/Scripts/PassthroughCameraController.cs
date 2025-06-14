using System.Collections;
using UnityEngine;

public class PassthroughCameraController : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer m_passthroughLayer;

    [SerializeField] private AnimationCurve m_fireCurve;
    [SerializeField] private float m_fireAnimationTime = 0.5f;

    [SerializeField] private AnimationCurve m_gameOverCurve;
    [SerializeField] private float m_gameOverAnimationTime = 0.5f;

    private bool m_inFireAnimation;
    private float m_fireAnimationTimer;
    private float m_gameOverAnimationTimer;

    private bool m_inGameOver;

    private void Update()
    {
        if (m_inFireAnimation && !m_inGameOver)
        {
            m_fireAnimationTimer -= Time.deltaTime;
            m_passthroughLayer.SetBrightnessContrastSaturation(0f, 0f, m_fireCurve.Evaluate(m_fireAnimationTimer / m_fireAnimationTime));

            if(m_fireAnimationTimer < 0f)
            {
                m_inFireAnimation = false;
            }
        }

        if(m_inGameOver)
        {
            m_gameOverAnimationTimer += Time.deltaTime;
            m_passthroughLayer.SetColorMapControls(0f, 0f, m_gameOverCurve.Evaluate(m_gameOverAnimationTimer / m_gameOverAnimationTime));

            if (m_gameOverAnimationTimer > m_gameOverAnimationTime)
            {
                m_inGameOver = false;
            }
        }
    }

    public void AnimateFire()
    {
        m_inFireAnimation = true;
        m_fireAnimationTimer = m_fireAnimationTime;
    }

    public void AnimateGameOver()
    {
        m_gameOverAnimationTimer = 0f;
        m_inGameOver = true;
    }
}