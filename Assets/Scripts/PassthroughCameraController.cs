using System.Collections;
using UnityEngine;

public class PassthroughCameraController : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer m_passthroughLayer;

    [SerializeField] private AnimationCurve m_fireCurve;

    private bool m_inFireAnimation;

    private void Update()
    {
        m_passthroughLayer.SetBrightnessContrastSaturation(0f, 0f, 1f - Mathf.PingPong(Time.time, 2f));
    }

    public void AnimateFire()
    {
        m_inFireAnimation = true;
    }
}