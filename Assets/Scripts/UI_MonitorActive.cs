using UnityEngine;

public class UI_MonitorActive : MonoBehaviour
{
    public AudioSource audioSource;


    public void PlaySoundEvent()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Audio neiet vecit!!! UI_MonitorActive .");
        }
    }

}
