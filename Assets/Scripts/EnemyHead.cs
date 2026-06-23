using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    [SerializeField] private Enemy m_enemyScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Projectile"))
        {
            m_enemyScript.Hurt(other.gameObject.GetComponent<Projectile>().GetDamage());
        }
    }
}
