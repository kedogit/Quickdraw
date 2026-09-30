using UnityEngine;

public class OrcSlam : EnemyAttackScript
{
    [SerializeField] private float m_launchForce = 500f;

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player playerScript = other.gameObject.GetComponent<Player>();
            other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * m_launchForce);
            playerScript.Hurt(m_attackDamage);
        }
    }
}
