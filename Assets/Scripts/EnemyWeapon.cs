using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    private Enemy m_enemyScript;
    private float m_damageDealt = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_enemyScript = transform.root.gameObject.GetComponent<Enemy>();
        m_damageDealt = m_enemyScript.GetDamage();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player playerScript = other.gameObject.GetComponent<Player>();
            playerScript.Hurt(m_damageDealt);
        }
    }
}
