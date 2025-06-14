using System.Collections;
using UnityEngine;

public class PassthroughCameraController : MonoBehaviour
{
    public enum CameraColorType { None = -1, Far, Mid, Close, GameOver }

    [SerializeField] private OVRPassthroughLayer m_passthroughLayer;

    [SerializeField] private AnimationCurve m_fireCurve;
    [SerializeField] private float m_fireAnimationTime = 0.5f;

    [SerializeField] private AnimationCurve m_gameOverCurve;
    [SerializeField] private float m_gameOverAnimationTime = 0.5f;

    private bool m_inFireAnimation;
    private float m_fireAnimationTimer;
    private float m_gameOverAnimationTimer;

    private float m_camAnimTimer;
    private float m_camSaturation;
    private float m_prevSaturation;
    private CameraColorType m_cameraColorType;

    private bool m_inGameOver;

    private void Update()
    {
        if (m_inFireAnimation && !m_inGameOver)
        {
            //m_fireAnimationTimer -= Time.deltaTime;
            //m_passthroughLayer.SetBrightnessContrastSaturation(0f, 0f, m_fireCurve.Evaluate(m_fireAnimationTimer / m_fireAnimationTime));

            //if (m_fireAnimationTimer < 0f)
            //{
            //    m_inFireAnimation = false;
            //}
        }

        if (m_inGameOver)
        {
            m_gameOverAnimationTimer += Time.deltaTime;
            m_passthroughLayer.SetColorMapControls(0f, 0f, m_gameOverCurve.Evaluate(m_gameOverAnimationTimer / m_gameOverAnimationTime));

            if (m_gameOverAnimationTimer > m_gameOverAnimationTime)
            {
                m_inGameOver = false;
            }
        }
        else
        {
            const float camAnimTime = 2f;

            if(m_camAnimTimer < camAnimTime)
            {
                m_camAnimTimer += Time.deltaTime;
                m_passthroughLayer.SetBrightnessContrastSaturation(0f, 0f, Mathf.Lerp(m_prevSaturation, m_camSaturation, m_camAnimTimer / camAnimTime));
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

    public void SetCameraColor(CameraColorType type)
    {
        if(m_cameraColorType == type) return;

        m_prevSaturation = m_camSaturation;

        switch (type)
        {
            case CameraColorType.None:
                break;
            case CameraColorType.Far:
                m_camSaturation = 0f;
                break;
            case CameraColorType.Mid:
                m_camSaturation = -0.5f;
                break;
            case CameraColorType.Close:
                m_camSaturation = -1f;
                break;
            case CameraColorType.GameOver:
                break;
            default:
                break;
        }

        m_camAnimTimer = 0f;
        m_cameraColorType = type;
    }
}