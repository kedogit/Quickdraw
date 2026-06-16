using System.Collections;
using UnityEngine;

public class GoblinMelee : Enemy
{
    [SerializeField] private BoxCollider m_clubHitbox;
    [SerializeField] private float m_attackDuration = 1f;

    protected override void Attack()
    {
        m_canMove = false;
        m_animator.SetTrigger("Attack");

        //m_clubHitbox.enabled = true;
        //REPLACE THIS WITH ANIM EVENT LATER
        StartCoroutine(TEMPORARYHITBOXFIX());

        StartCoroutine(DisableHitbox());
    }

    private IEnumerator TEMPORARYHITBOXFIX()
    {
        yield return new WaitForSeconds(0.7f);
        m_clubHitbox.enabled = true;
    }

    private IEnumerator DisableHitbox()
    {
        yield return new WaitForSeconds(m_attackDuration);
        m_clubHitbox.enabled = false;
        m_canMove = true;
    }
}
