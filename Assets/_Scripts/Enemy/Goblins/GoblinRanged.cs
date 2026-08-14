using System.Collections;
using UnityEngine;

public class GoblinRanged : Enemy
{
    [SerializeField] float m_arrowDamage = 10f;
    [SerializeField] private GameObject m_arrow;
    [SerializeField] private float m_arrowForce = 50f;
    [SerializeField] private float m_arrowCleanupTime = 5f;
    [SerializeField] private AudioClip m_shootSFX;
    [SerializeField] private AudioClip m_drawSFX;

    const float m_arrowRescale = 5f;

    private GameObject m_currentArrow;


    private void Awake()
    {
        m_enemySize = EnemySize.SMALL;
    }

    public override void Attack()
    {
        //stop movement and start attack animation
        m_agent.velocity = Vector3.zero;

        m_animator.SetTrigger("Attack");
        
        //find the righthand's transform and instantiate the arrow there. rescale arrow
        Transform rightHand = transform.Find("rootSkeleton/pelvis_joint/waist_joint/chest_joint/R_clavicle_joint/R_shoulder_joint/R_elbow_joint/R_wrist_joint/R_equip_joint");
        m_currentArrow = Instantiate(m_arrow, rightHand.position, rightHand.rotation, rightHand);
        m_currentArrow.GetComponent<EnemyWeapon>().SetWeaponDamage(m_arrowDamage);
        m_currentArrow.transform.localScale *= m_arrowRescale;
    }

    public void LaunchArrow()
    {
        //play shoot sfx
        m_audioSource.clip = m_shootSFX;
        m_audioSource.Play();

        //set target, find direction vector
        Vector3 target = m_target.position;
        Vector3 directionVector = target - m_currentArrow.transform.position;
        directionVector.Normalize();

        //turn off gravity, orient properly, detach from hand and send it forward
        Rigidbody arrowBody = m_currentArrow.AddComponent<Rigidbody>();
        arrowBody.useGravity = false;
        m_currentArrow.transform.LookAt(target);
        m_currentArrow.transform.parent = null;
        arrowBody.AddForce(directionVector * m_arrowForce, ForceMode.Impulse);

        //enable trail
        m_currentArrow.GetComponent<TrailRenderer>().enabled = true;

        //active the collider on the arrow
        BoxCollider arrowCollider = m_currentArrow.GetComponent<BoxCollider>();
        arrowCollider.enabled = true;

        //start destroy timer
        StartCoroutine(DestroyArrow(m_currentArrow));
    }

    public void DrawString()
    {
        m_audioSource.clip = m_drawSFX;
        m_audioSource.Play();
    }

    private IEnumerator DestroyArrow(GameObject arrow)
    {
        yield return new WaitForSeconds(m_arrowCleanupTime);
        Destroy(arrow);
    }
}
