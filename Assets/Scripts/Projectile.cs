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
}
