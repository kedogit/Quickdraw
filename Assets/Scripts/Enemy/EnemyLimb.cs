using UnityEngine;

public class EnemyLimb : MonoBehaviour
{
    [SerializeField] private Enemy m_enemyScript;
    [SerializeField] private BodyPart m_bodyPart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Projectile"))
        {
            m_enemyScript.LodgeArrow(GetComponent<BoxCollider>().bounds.center, other.gameObject);
            m_enemyScript.Hurt(other.gameObject.GetComponent<Projectile>().GetDamage(), m_bodyPart);
        }
    }
}
