using UnityEngine;

public class Chest : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().AcquireGrapple();
            Destroy(this.gameObject);
        }
    }
}
