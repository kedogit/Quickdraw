using System;
using System.Collections;
using UnityEngine;

public class Orc : Enemy
{
    //TODO!!!: create child of Enemy called MeleeEnemy, include everything related to enabling and disabling weapon hitboxes.
    //make orc and meleegoblin children of MeleeEnemy. too much shared code

    [SerializeField] private AudioClip m_swingSFX;
    [SerializeField] private BoxCollider m_weaponHitbox;
    [SerializeField] private float m_attackDuration = 1f;

    private void Awake()
    {
        m_enemySize = EnemySize.BIG;
    }

    public override void Attack()
    {
        m_animator.SetTrigger("Attack");
        StartCoroutine(DisableHitbox());
    }

    public void EnableHitbox()
    {
        //enables hitbox and plays sfx (called by animevent)
        m_audioSource.clip = m_swingSFX;
        m_audioSource.Play();
        m_weaponHitbox.enabled = true;
    }

    private IEnumerator DisableHitbox()
    {
        yield return new WaitForSeconds(m_attackDuration);
        m_weaponHitbox.enabled = false;
        m_animator.SetBool("isRunning", false);
    }
}
