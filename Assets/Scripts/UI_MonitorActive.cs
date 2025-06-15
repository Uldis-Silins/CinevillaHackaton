using TMPro;
using UnityEngine;

public class UI_MonitorActive : MonoBehaviour
{
    public AudioSource audioSource;
    public Animator anim;
    public TextMeshPro m_scoreLabel;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnKill()
    {
        anim.SetTrigger("PlayAnimation");
    }

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

    public void SetScore(int score)
    {
        m_scoreLabel.text = score.ToString();
    }
}
