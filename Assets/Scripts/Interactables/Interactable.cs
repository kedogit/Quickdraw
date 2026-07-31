using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected Player m_player;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_player = collision.gameObject.GetComponent<Player>();
            m_player.SetInteractTarget(this);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_player.SetInteractTarget(null);
        }
    }

    protected void DisableInteractable()
    {
        GetComponent<BoxCollider>().enabled = false;
        m_player.SetInteractTarget(null);
    }

    public abstract void InteractAction();
}
