using System;
using System.Collections;
using UnityEngine;

public class Orc : EnemyMeleeArmed
{
    [Header("Orc")]
    [SerializeField] private float m_swingDamage = 25f;
    [SerializeField] private float m_slamDamage = 30f;
    [SerializeField] private AudioClip m_swingSFX;
    [SerializeField] private AnimationClip m_swingAnimation;

    private const float swingActiveHitboxDuration = 0.7f;

    private void Awake()
    {
        EnemyAttackScript swordScript = GetComponentInChildren<EnemyAttackScript>();
        swordScript.SetAttackDamage(m_swingDamage);

        OrcSlam slamScript = GetComponentInChildren<OrcSlam>();
        slamScript.SetAttackDamage(m_slamDamage);

        m_enemySize = EnemySize.BIG;
    }

    public override void Attack()
    {
        //int rngFactor = UnityEngine.Random.Range(0, 2);
        int rngFactor = 1;

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
