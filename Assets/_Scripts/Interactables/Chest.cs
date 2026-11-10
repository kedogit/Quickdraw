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
        //give the player the grapple, play the animation and disable the interactable (parent func)
        Observer.GetInstance().TriggerEvent(EVENT.ACQUIRED_ITEM_GRAPPLE);
        m_animator.Play();
        AudioManager.GetInstance().PlaySFX(SFX.CHEST_OPEN, transform.position);
        AudioManager.GetInstance().PlaySFX(SFX.ITEM_ACQUIRED, transform.position);
        DisableInteractable();
    }
}
