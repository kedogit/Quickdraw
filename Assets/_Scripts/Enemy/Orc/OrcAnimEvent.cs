using Unity.Cinemachine;
using UnityEngine;

public class OrcAnimEvent : MonoBehaviour
{
    [Header("Main Script")]
    [SerializeField] private Orc m_enemyScript;

    [Header("Slam")]
    [SerializeField] private BoxCollider m_slamHitbox;
    [SerializeField] private SFX m_slamSFX;
    [SerializeField] private float m_slamHitboxDuration = 0.1f;

    [Header("Sword")]
    [SerializeField] private BoxCollider m_swordHitbox;
    [SerializeField] private SFX m_swingSFX;
    [SerializeField] private TrailRenderer m_swordTrail;
    [SerializeField] private float m_swingHitboxDuration = 0.3f;
    [SerializeField] private float m_swingTrailDuration = 0.4f;

    private CinemachineImpulseSource m_impulseComponent;

    private void Start()
    {
        m_impulseComponent = GetComponent<CinemachineImpulseSource>();
    }

    public void SwingStart()
    {
        m_enemyScript.HandleAttackEffects(m_swingSFX, m_swordHitbox, m_swingHitboxDuration, m_swordTrail, m_swingTrailDuration);
    }

    public void DecideNextMove()
    {
        m_enemyScript.DecideNextMove();
    }

    public void SlamLanding()
    {
        //enable hitbox
        m_enemyScript.HandleAttackEffects(m_slamSFX, m_slamHitbox, m_slamHitboxDuration);

        //shake camera
        m_impulseComponent.GenerateImpulse();
    }
}
