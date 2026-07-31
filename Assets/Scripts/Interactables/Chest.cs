using UnityEngine;

public class Chest : Interactable
{
    private Animation m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animation>();
    }
    public override void InteractAction()
    {
        if (m_player != null)
        {
            m_player.AcquireGrapple();
            m_animator.Play();
            DisableInteractable();
        }
    }
}
