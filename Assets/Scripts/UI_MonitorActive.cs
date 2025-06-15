using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UI_MonitorActive : MonoBehaviour
{
    public UnityEvent onHit;

    public AudioSource audioSource;
    public Animator anim;
    public TextMeshPro m_scoreLabel;
    public Collider hitBox;

    public Vector3 inGameLocation = new Vector3(0, 0, 40);

    private Vector3 m_targetLocation;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, m_targetLocation, 10f * Time.deltaTime);
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

    public void OnHit()
    {
        m_targetLocation = inGameLocation;
        hitBox.enabled = false;
        onHit.Invoke();
    }
}
