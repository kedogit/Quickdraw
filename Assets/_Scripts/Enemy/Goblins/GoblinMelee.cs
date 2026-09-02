using System;
using System.Collections;
using UnityEngine;

public class GoblinMelee : Enemy
{
    [SerializeField] private float m_swingDamage = 25f;
    [SerializeField] private BoxCollider m_clubHitbox;
    [SerializeField] private AudioClip m_swingSFX;
    [SerializeField] private AnimationClip m_swingAnimation;
    [SerializeField] private TrailRenderer m_swingTrail;

    //disables hitbox at the given percentage of animation length. if 0.6f, turns off hitbox 60% of the way through animation
    private const float swingActiveHitboxDuration = 0.7f;

    private void Awake()
    {
        EnemyWeapon clubScript = GetComponentInChildren<EnemyWeapon>();
        clubScript.SetWeaponDamage(m_swingDamage);
        m_enemySize = EnemySize.SMALL;
    }

    public override void Attack()
    {
        m_animator.SetBool("isRunning", false);
        Swing();
    }

    private void Swing()
    {
        m_animator.speed = m_swingAnimation.length / m_attackSpeed;
        m_animator.SetBool("isAttacking", true);
        StartCoroutine(DisableHitbox(m_attackSpeed * swingActiveHitboxDuration));
    }

    public void EnableHitbox()
    {
        //enables hitbox and plays sfx (called by animevent)
        m_swingTrail.enabled = true;
        m_audioSource.clip = m_swingSFX;
        m_audioSource.Play();
        m_clubHitbox.enabled = true;
    }

    private IEnumerator DisableHitbox(float hitboxDuration)
    {
        yield return new WaitForSeconds(hitboxDuration);
        m_swingTrail.enabled = false;
        m_clubHitbox.enabled = false;
    }
}
