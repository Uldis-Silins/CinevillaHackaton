using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// <para>Quick mockup of hand gesture projectile firing class</para>
/// Charge shot with all fingers closed pose<br/>
/// Fire when velocity state is selected and all fingers open pose trigger is fired<br/>
/// </summary>
public class HandInputController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<float> onHandFired; // T0: timestamp

    [SerializeField] private ActiveStateSelector m_handOpenSelector;
    [SerializeField] private ActiveStateSelector m_handClosedSelector;

    [SerializeField] private ActiveStateSelector m_handVelocitySelector;

    /// <summary>
    /// Has the hand reached the desired activation velocity?<br/>
    /// Set true when hand velocity ActiveStateSelector fires selected event, false when unselected
    /// </summary>
    private bool m_handVelocityReached;

    /// <summary>
    /// 
    /// </summary>
    private bool m_handCharged;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        m_handOpenSelector.WhenSelected += WhenSelectedHandOpen;
        m_handOpenSelector.WhenUnselected += WhenUnselectedHandOpen;
        m_handClosedSelector.WhenSelected += WhenSelectedHandClosed;
        m_handVelocitySelector.WhenSelected += WhenSelectedHandVelocity;
        m_handVelocitySelector.WhenUnselected += WhenUnselectedHandVelocity;
    }

    private void OnDisable()
    {
        m_handOpenSelector.WhenSelected -= WhenSelectedHandOpen;
        m_handOpenSelector.WhenUnselected -= WhenUnselectedHandOpen;
        m_handClosedSelector.WhenSelected -= WhenSelectedHandClosed;
        m_handVelocitySelector.WhenSelected -= WhenSelectedHandVelocity;
        m_handVelocitySelector.WhenUnselected -= WhenUnselectedHandVelocity;
    }

    private void WhenSelectedHandOpen()
    {
        if(m_handVelocityReached && m_handCharged)
        {
            onHandFired.Invoke(Time.time);
            m_handCharged = false;
        }
    }

    private void WhenUnselectedHandOpen()
    {
        if( m_handCharged)
        {
            m_handCharged = false;
        }
    }

    private void WhenSelectedHandClosed()
    {
        if(!m_handCharged)
        {
            m_handCharged = true;
            // TODO: fire charged event
        }
    }

    private void WhenUnselectedHandClosed()
    {
    }

    private void WhenSelectedHandVelocity()
    {
        m_handVelocityReached = true;
    }

    private void WhenUnselectedHandVelocity()
    {
        m_handVelocityReached = false;
    }
}
