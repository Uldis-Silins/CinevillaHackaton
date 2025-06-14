using System.Collections;
using UnityEngine;

public class PassthroughCameraController : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer m_passthroughLayer;

    [SerializeField] private AnimationCurve m_fireCurve;
    [SerializeField] private float m_fireAnimationTime = 0.5f;

    private bool m_inFireAnimation;
    private float m_fireAnimationTimer;

    private void Update()
    {
        if (m_inFireAnimation)
        {
            m_fireAnimationTimer -= Time.deltaTime;
            m_passthroughLayer.SetBrightnessContrastSaturation(0f, 0f, m_fireCurve.Evaluate(m_fireAnimationTimer / m_fireAnimationTime));

            if(m_fireAnimationTimer < 0f)
            {
                m_inFireAnimation = false;
            }
        }
    }

    public void AnimateFire()
    {
        m_inFireAnimation = true;
        m_fireAnimationTimer = m_fireAnimationTime;
    }
}