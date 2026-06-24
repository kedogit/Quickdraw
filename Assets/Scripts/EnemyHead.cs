using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    [SerializeField] private Enemy m_enemyScript;
    [SerializeField] private float m_damageMult = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Projectile"))
        {
            m_enemyScript.Hurt(other.gameObject.GetComponent<Projectile>().GetDamage() * m_damageMult);
        }
    }
}
