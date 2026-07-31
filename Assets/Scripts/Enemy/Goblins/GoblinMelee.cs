using System.Collections;
using UnityEngine;

public class GoblinMelee : Enemy
{
    [SerializeField] private float m_swingDamage = 25f;
    [SerializeField] private BoxCollider m_clubHitbox;
    [SerializeField] private float m_attackDuration = 1f;
    [SerializeField] private AudioClip m_swingSFX;

    private void Awake()
    {
        EnemyWeapon clubScript = GetComponentInChildren<EnemyWeapon>();
        clubScript.SetWeaponDamage(m_swingDamage);
    }

    protected override void Attack()
    {
        //stop movement, start animation and start timer to disable hitbox
        m_canMove = false;
        m_animator.SetTrigger("Attack");
        StartCoroutine(DisableHitbox());
    }

    public void EnableHitbox()
    {
        //enables hitbox and plays sfx (called by animevent)
        m_audioSource.clip = m_swingSFX;
        m_audioSource.Play();
        m_clubHitbox.enabled = true;
    }

    private IEnumerator DisableHitbox()
    {
        yield return new WaitForSeconds(m_attackDuration);
        m_clubHitbox.enabled = false;
        m_canMove = true;
        m_animator.SetBool("isRunning", false);
    }
}
