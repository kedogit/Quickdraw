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
        StartCoroutine(DisableHitbox());
    }

    public void EnableHitbox()
    {
        m_clubHitbox.enabled = true;
    }

    private IEnumerator DisableHitbox()
    {
        yield return new WaitForSeconds(m_attackDuration);
        m_clubHitbox.enabled = false;
        m_canMove = true;
    }
}
