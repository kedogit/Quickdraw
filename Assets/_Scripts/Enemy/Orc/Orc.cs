using System;
using System.Collections;
using UnityEngine;

public class Orc : EnemyMeleeArmed
{
    [Header("Orc")]
    [SerializeField] private float m_swingDamage = 25f;
    [SerializeField] private AudioClip m_swingSFX;
    [SerializeField] private AnimationClip m_swingAnimation;

    private const float swingActiveHitboxDuration = 0.7f;

    private void Awake()
    {
        EnemyWeapon clubScript = GetComponentInChildren<EnemyWeapon>();
        clubScript.SetWeaponDamage(m_swingDamage);
        m_enemySize = EnemySize.BIG;
    }

    public override void Attack()
    {
        int rngFactor = UnityEngine.Random.Range(0, 2);
        //int rngFactor = 2;

        if (rngFactor == 0)
        {
            Swing();
        }
        else if (rngFactor == 1)
        {
            JumpAttack();
        }
    }

    private void Swing()
    {
        m_animator.speed = m_swingAnimation.length / m_attackSpeed;
        m_animator.SetTrigger("Swing");
        StartCoroutine(DisableWeaponHitbox(m_attackSpeed * swingActiveHitboxDuration));
    }

    private void JumpAttack()
    {
        m_animator.SetTrigger("JumpAttack");
    }
}
