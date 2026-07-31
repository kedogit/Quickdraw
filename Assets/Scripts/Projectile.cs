using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float m_projectileDamage = 0f;
    private float m_despawnTime = 5f;

    public void SetDamage(float damage)
    {
        m_projectileDamage = damage;
    }

    public float GetDamage()
    {
        return m_projectileDamage;
    }

    public void SetActive()
    {
        StartCoroutine(DespawnArrow());
    }

    private IEnumerator DespawnArrow()
    {
        yield return new WaitForSeconds(m_despawnTime);
        Destroy(this.gameObject);
    }

    public void LodgeArrow(Vector3 stickPosition, Transform target)
    {
        //destroy arrow's components
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());
        Destroy(GetComponent<TrailRenderer>());

        //stick it to the target
        transform.SetParent(target);

        //downscale the arrow
        transform.localScale *= 0.5f;

        //set its position to the point of impact
        transform.position = stickPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        //&& other.gameObject.layer != LayerMask.NameToLayer("Invisible")
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy") && !other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
