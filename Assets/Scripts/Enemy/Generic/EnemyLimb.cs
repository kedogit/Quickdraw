using UnityEngine;

public class EnemyLimb : MonoBehaviour
{
    [SerializeField] private Enemy m_enemyScript;
    [SerializeField] private BodyPart m_bodyPart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Projectile"))
        {
            //lodge arrow in body and hurt the enemy
            other.GetComponent<Projectile>().LodgeArrow(GetComponent<CapsuleCollider>().bounds.center, this.transform);
            m_enemyScript.Hurt(other.gameObject.GetComponent<Projectile>().GetDamage(), m_bodyPart);
        }
    }
}
