using System;
using System.Collections;
using UnityEngine;

public class GoblinMelee : EnemyMeleeArmed
{
    [Header("Melee Goblin")]
    [SerializeField] private float m_swingDamage = 25f;
    [SerializeField] private AudioClip m_swingSFX;
    [SerializeField] private AnimationClip m_swingAnimation;

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
        Swing();
    }

    private void Swing()
    {
        m_animator.speed = m_swingAnimation.length / m_attackSpeed;
        m_animator.SetTrigger("Swing");
        StartCoroutine(DisableWeaponHitbox(m_attackSpeed * swingActiveHitboxDuration));
    }
}
