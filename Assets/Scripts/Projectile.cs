using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float m_projectileDamage = 0f;
    private float m_despawnTime = 5f;
    private float m_stuckDownscale = 0.5f;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            //enemy hit
            Enemy enemyScript = other.gameObject.GetComponent<Enemy>();
            enemyScript.Hurt(m_projectileDamage);

            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<BoxCollider>());

            Vector3 otherPosition = other.gameObject.transform.position;
            otherPosition.y = transform.position.y;
            transform.position = otherPosition;

            transform.localScale *= m_stuckDownscale;

            Transform rootSkeleton = other.transform.Find("rootSkeleton");
            transform.SetParent(rootSkeleton);
        }
    }

    private IEnumerator DespawnArrow()
    {
        yield return new WaitForSeconds(m_despawnTime);
        Destroy(this.gameObject);
    }
}
