using System.Collections;
using UnityEngine;

public class Vampires : MonoBehaviour
{
    public GameObject[] vampires;

    public float animTime = 1f;

    public AudioSource vampAudio;

    private float m_timer;
    private int m_curIndex;

    private bool m_inAnimation;

    public bool Finished {  get; private set; }

    private void Update()
    {
        if (m_inAnimation)
        {
            if (m_timer > animTime)
            {
                m_curIndex++;

                if (m_curIndex >= vampires.Length)
                {
                    m_inAnimation = false;
                    Finished = true;
                }
                else
                {
                    vampires[m_curIndex].SetActive(true);
                    m_timer = 0;
                }

                vampAudio.transform.position =  vampires[m_curIndex].transform.position;
            }

            m_timer += Time.deltaTime;
        }
    }

    public void PlayAnimation()
    {
        m_curIndex = 0;
        m_timer = 0f;
        vampires[m_curIndex].SetActive(true);
        m_inAnimation = true;

        vampAudio.transform.position = vampires[m_curIndex].transform.position;
        vampAudio.Play();
    }
}