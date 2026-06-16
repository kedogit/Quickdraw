using System.Collections;
using UnityEngine;

public class GoblinRanged : Enemy
{
    [SerializeField] private GameObject m_arrow;
    [SerializeField] private float m_arrowForce = 50f;
    [SerializeField] private float m_arrowCleanupTime = 5f;

    const float m_arrowRescale = 5f;

    private GameObject m_currentArrow;


    protected override void Attack()
    {
        m_canMove = false;
        m_animator.SetTrigger("Attack");
        GameObject rightHand = GameObject.Find("R_equip_joint");
        m_currentArrow = Instantiate(m_arrow, rightHand.transform.position, rightHand.transform.rotation, rightHand.transform);
        m_currentArrow.transform.localScale *= m_arrowRescale;
    }

    public void LaunchArrow()
    {
        Vector3 target = m_playerPosition;

        Vector3 directionVector = target - m_currentArrow.transform.position;
        directionVector.Normalize();

        Rigidbody arrowBody = m_currentArrow.AddComponent<Rigidbody>();
        arrowBody.useGravity = false;
        m_currentArrow.transform.LookAt(target);
        m_currentArrow.transform.parent = null;
        arrowBody.AddForce(directionVector * m_arrowForce, ForceMode.Impulse);

        BoxCollider arrowCollider = m_currentArrow.GetComponent<BoxCollider>();
        arrowCollider.enabled = true;

        StartCoroutine(DestroyArrow(m_currentArrow));
    }

    private IEnumerator DestroyArrow(GameObject arrow)
    {
        yield return new WaitForSeconds(m_arrowCleanupTime);
        Destroy(arrow);
    }
}
