using System.Collections;
using UnityEngine;

public class WallHitDecal : MonoBehaviour
{
    public Renderer meshRenderer;
    public float animTime = 1f;

    public AnimationCurve scaleCurve;

    private Color m_startColor;
    private Color m_endColor;
    private float m_timer;

    private Vector3 m_startScale;

    private void OnEnable()
    {
        m_startColor = meshRenderer.material.color;
        m_endColor = m_startColor;
        m_endColor.a = 0f;

        m_startScale = meshRenderer.transform.localScale;

        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        while (m_timer < animTime / 2)
        {
            yield return null;
            m_timer += Time.deltaTime;
            meshRenderer.transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, m_startScale, scaleCurve.Evaluate(m_timer / (animTime / 2)));
        }

        yield return new WaitForSeconds(animTime / 2);

        m_timer = 0f;

        while (m_timer < animTime)
        {
            yield return null;
            m_timer += Time.deltaTime;
            meshRenderer.material.color = Color.Lerp(m_startColor, m_endColor, m_timer / animTime);
        }

        Destroy(gameObject);
    }
}