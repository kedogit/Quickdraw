using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    protected float m_attackDamage = 0;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player playerScript = other.gameObject.GetComponent<Player>();
            playerScript.Hurt(m_attackDamage);
        }
    }

    //public func to centralize weapon damage variable to enemy script
    public void SetAttackDamage(float attackDamage)
    {
        m_attackDamage = attackDamage;
    }
}
