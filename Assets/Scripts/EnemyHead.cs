using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    private Enemy m_enemyScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_enemyScript = transform.root.gameObject.GetComponent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Projectile"))
        {
            m_enemyScript.Hurt(other.gameObject.GetComponent<Projectile>().GetDamage());
        }
    }
}
