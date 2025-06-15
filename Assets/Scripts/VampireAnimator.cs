using System.Collections;
using UnityEngine;

public class VampireAnimator : MonoBehaviour
{
    public Renderer[] renderers;
    public ParticleSystem particles;

    public float animTime = 0.4f;

    private float m_timer;

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        particles.transform.parent = transform;
        yield return new WaitForSeconds(0.35f);

        particles.gameObject.SetActive(true);
        particles.transform.parent = null;
        yield return new WaitForSeconds(0.35f);

        while (m_timer < animTime)
        {
            yield return null;

            foreach (var r in renderers)
            {
                r.material.SetFloat("_Dissolve", m_timer / animTime);
            }

            m_timer += Time.deltaTime;
        }

        gameObject.SetActive(false);
    }    
}