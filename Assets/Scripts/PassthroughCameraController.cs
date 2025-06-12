using System.Collections;
using UnityEngine;

public class PassthroughCameraController : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer m_passthroughLayer;

    private void Update()
    {
        m_passthroughLayer.SetBrightnessContrastSaturation(0f, 0f, 1f - Mathf.PingPong(Time.time, 2f));
    }
}