using System;
using System.Collections;
using UnityEngine;

public class GoblinMelee : EnemyMelee
{
    [Header("Melee Goblin")]
    [SerializeField] private float m_swingDamage = 25f;

    private void Awake()
    {
        EnemyAttackScript clubScript = GetComponentInChildren<EnemyAttackScript>();
        clubScript.SetAttackDamage(m_swingDamage);
        m_enemySize = EnemySize.SMALL;
    }

    public override void Attack()
    {
        Swing();
    }

    private void Swing()
    {
        m_animator.speed = m_attackSpeed;
        m_animator.SetTrigger("Swing");
    }
}
