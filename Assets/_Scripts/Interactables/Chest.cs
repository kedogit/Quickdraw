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
            //give the player the grapple, play the animation and disable the interactable (parent func)
            m_player.AcquireGrapple();
            m_animator.Play();
            DisableInteractable();
        }
    }
}
