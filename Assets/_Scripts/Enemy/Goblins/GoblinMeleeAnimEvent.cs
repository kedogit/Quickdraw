using UnityEngine;

public class GoblinMeleeAnimEvent : EnemyAnimationEvent<GoblinMelee>
{
    [Header("Club")]
    [SerializeField] private BoxCollider m_clubHitbox;
    [SerializeField] private SFX m_swingSFX;
    [SerializeField] private TrailRenderer m_clubTrail;
    [SerializeField] private float m_swingHitboxDuration = 0.2f;
    [SerializeField] private float m_swingTrailDuration = 0.3f;

    public void HitboxStart()
    {
        m_attackSpeed = m_enemyScript.AttackSpeed;
        m_enemyScript.HandleAttackEffects(m_swingSFX, m_clubHitbox, m_swingHitboxDuration/m_attackSpeed, m_clubTrail, m_swingTrailDuration/m_attackSpeed);
    }
}
