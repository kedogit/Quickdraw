using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Dictionary<string, object> eventParams = new Dictionary<string,object>();
            eventParams.Add("Target", this);

            Observer.GetInstance().TriggerEvent(EVENT.ON_INTERACT_NEARBY, eventParams);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Observer.GetInstance().TriggerEvent(EVENT.ON_INTERACT_EXIT_RANGE);
        }
    }

    protected void DisableInteractable()
    {
        //disables collider and removes interact target
        GetComponent<BoxCollider>().enabled = false;
        Observer.GetInstance().TriggerEvent(EVENT.ON_INTERACT_EXIT_RANGE);
        GetComponent<MeshRenderer>().materials[1] = null;
    }

    public abstract void InteractAction();
}
