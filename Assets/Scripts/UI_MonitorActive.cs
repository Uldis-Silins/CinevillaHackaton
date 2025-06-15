using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UI_MonitorActive : MonoBehaviour
{
    public UnityEvent onHit;

    public SpriteRenderer startGameRenderer;

    public AudioSource audioSource;
    public Animator anim;
    public TextMeshPro m_scoreLabel;
    public Collider hitBox;

    public Vector3 inGameLocation = new Vector3(0, 0, 40);

    private Vector3 m_startLocation;
    private Vector3 m_targetLocation;
    private bool m_gameStarted;
    private float m_animTimer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        m_startLocation = transform.position;
    }

    private void Update()
    {
        if (m_gameStarted)
        {
            transform.position = Vector3.Lerp(m_startLocation, m_targetLocation, 1f - (m_animTimer / 2f));
        }
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
        startGameRenderer.enabled = false;
        m_targetLocation = inGameLocation;
        hitBox.enabled = false;
        onHit.Invoke();

        m_animTimer = 2f;
        m_gameStarted = true;

        audioSource.Play();
    }
}
