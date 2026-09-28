using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BodyPart
{
    HEAD,
    BODY
}

public enum EnemySize
{
    SMALL,
    BIG
}

public abstract class Enemy : MonoBehaviour
{
    [Header("Stun")]
    [SerializeField] private float m_maxStun = 100f;
    [SerializeField] private float m_stunDuration = 2f;
    [SerializeField] private float m_stunRecoveryDelay = 2f;
    [SerializeField] private float m_stunRecoveryTickRate = 0.2f;
    [SerializeField] private float m_stunRecoveryPerTick = 5f;

    [Header("Stats")]
    [SerializeField] private float m_maxHP = 100f;
    [SerializeField] protected float m_attackSpeed = 1f;
    [SerializeField] private float m_deathCleanupTime = 15f;

    [Header("References")]
    [SerializeField] protected Animator m_animator;
    [SerializeField] private SFX m_hurtSFX;
    [SerializeField] private SFX m_deathSFX;
    [SerializeField] private GameObject m_bars;
    [SerializeField] private GameObject m_stunParticles;

    [Header("Body Parts")]
    [SerializeField] private float m_bodyMultiplier = 1f;
    [SerializeField] private float m_headshotMultiplier = 2f;

    public float AttackSpeed => m_attackSpeed;
    public Transform Target => m_target;

    protected NavMeshAgent m_agent;
    protected AudioSource m_audioSource;
    protected Transform m_target;

    private float m_currStun;
    private bool m_ongoingStunRecovery;
    private bool m_ongoingStunDelay;
    private Coroutine m_stunDelayCoroutine;
    private Coroutine m_stunRecoveryCoroutine;

    private float m_currentHP;
    protected EnemySize m_enemySize;

    private Animator m_hpBarAnimator;
    private Animator m_stunFillAnimator;
    private Animation m_stunFlashAnimator;

    private ArenaTrigger m_arenaScript;

    protected Dictionary<BodyPart, float> bodyDamageMultipliers;

    protected EnemyState m_currentState;

    public abstract void Attack();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (m_target == null)
        {
            m_currentState = new EnemyIdle(this, m_animator);
        }
        else
        {
            m_currentState = new EnemyChase(this, m_animator);
        }
        m_currentHP = m_maxHP;
        m_agent = GetComponent<NavMeshAgent>();
        m_audioSource = GetComponent<AudioSource>();

        m_hpBarAnimator = m_bars.transform.Find("HPBar/Fill").GetComponent<Animator>();
        m_stunFillAnimator = m_bars.transform.Find("StunBar/Fill").GetComponent<Animator>();
        m_stunFlashAnimator = m_bars.transform.Find("StunBar/Border").GetComponent<Animation>();

        //create the body part dictionary for hurt function
        bodyDamageMultipliers = new Dictionary<BodyPart, float>();
        bodyDamageMultipliers.Add(BodyPart.BODY, m_bodyMultiplier);
        bodyDamageMultipliers.Add(BodyPart.HEAD, m_headshotMultiplier);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        m_currentState.Execute();
    }

    public void CheckDistance()
    {
        if (m_currentState is EnemyAttack)
        {
            if (Vector3.Distance(this.transform.position, m_target.position) > m_agent.stoppingDistance)
            {
                ChangeState(new EnemyChase(this, m_animator));
            }
            else
            {
                Attack();
            }
        }
    }

    public void SetTarget(Transform player)
    {
        //most importantly used by arena script to target player
        m_target = player;
    }

    public void SetArenaScript(ArenaTrigger script)
    {
        m_arenaScript = script;
    }

    public void Hurt(float damageAmount, BodyPart partHit)
    {
        if (m_currentState is not EnemyDead)
        {
            //trigger event for hitmarker
            Observer.GetInstance().TriggerEvent(EVENT.ON_ENEMY_HURT);

            //show bars
            m_bars.SetActive(true);

            //update HP
            UpdateHP(m_currentHP - damageAmount * bodyDamageMultipliers[partHit]);

            //increment stun gauge
            AccumulateStun(damageAmount);

            //if hp at 0, die
            if (m_currentHP <= 0)
            {
                Die();
            }
            else
            {
                //play hurt sfx
                AudioManager.GetInstance()?.PlaySFX(m_hurtSFX, transform.position);

                //needed in case the player hurts an enemy that never spotted them
                if (m_target == null)
                {
                    SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
                }

                //if enemy is not stunned and is small, hurt state. if big, chasing state.
                if (m_currentState is not EnemyStunned)
                {
                    if (m_enemySize == EnemySize.SMALL)
                    {
                        ChangeState(new EnemyHurt(this, m_animator));
                    }
                    else
                    {
                        ChangeState(new EnemyChase(this, m_animator));
                    }
                }
            }
        }
    }

    private void UpdateHP(float newHP)
    {
        m_currentHP = newHP;
        m_hpBarAnimator.SetFloat("Fill", m_currentHP / m_maxHP * 100);
    }

    private void UpdateStun(float newStun)
    {
        m_currStun = newStun;
        m_stunFillAnimator.SetFloat("Fill", m_currStun / m_maxStun * 100);
    }

    private void AccumulateStun(float stunAmount)
    {
        //add the stun to the stun gauge
        UpdateStun(m_currStun + stunAmount);

        //stop ongoing stun recovery routines if any
        if (m_ongoingStunDelay)
        {
            StopCoroutine(m_stunDelayCoroutine);
            m_ongoingStunDelay = false;
        }
        if (m_ongoingStunRecovery)
        {
            StopCoroutine(m_stunRecoveryCoroutine);
            m_ongoingStunRecovery = false;
        }

        //if current stun is higher than max, change state to stunned
        if (m_currStun >= m_maxStun)
        {
            m_currStun = m_maxStun;
            m_stunParticles.SetActive(true);

            //this line's goal was to set the lifetime of the stun particles appropriately so they shrink fully depending on the set stun duration
            //it broke the particle trail because trail lifetime is tied to particle lifetime. dunno how to fix, not important
            //var stunParticles = m_stunParticles.GetComponent<ParticleSystem>().main;
            //stunParticles.startLifetime = m_stunDuration;

            m_stunFlashAnimator.Play();
            ChangeState(new EnemyStunned(this, m_animator, m_stunDuration));
        }
        //if stun not at max, restart the delay for stun recovery and stop any active recovery
        else
        {
            m_stunDelayCoroutine = StartCoroutine(StunRecoveryDelay());
        }
    }

    private IEnumerator StunRecoveryDelay()
    {
        m_ongoingStunDelay = true;

        yield return new WaitForSeconds(m_stunRecoveryDelay);
        if (!m_ongoingStunRecovery)
        {
            m_stunRecoveryCoroutine = StartCoroutine(StartStunRecovery());
        }

        m_ongoingStunDelay = false;
    }

    private IEnumerator StartStunRecovery()
    {
        m_ongoingStunRecovery = true;

        //initial recovery tick
        m_currStun -= m_stunRecoveryPerTick;

        //while stun is above 0, keep recovering
        while (m_currStun > 0)
        {
            yield return new WaitForSeconds(m_stunRecoveryTickRate);
            m_currStun -= m_stunRecoveryPerTick;
            UpdateStun(m_currStun - m_stunRecoveryPerTick);
        }

        //sets the stun to exactly 0 to make sure it is not a negative number
        ResetStun();
        m_ongoingStunRecovery = false;
    }

    public void ResetStun()
    {
        UpdateStun(0);
        m_stunParticles.SetActive(false);
        m_stunFlashAnimator.Stop();
    }

    private void Die()
    {
        //if stunned, disable stun animation and particles
        m_animator.SetBool("isStunned", false);
        m_stunParticles.SetActive(false);

        //turn off overhead bars
        m_bars.SetActive(false);

        //if arena enemy, tell arena to increment kill count
        m_arenaScript?.RegisterKill();

        //play death sfx
        AudioManager.GetInstance()?.PlaySFX(m_deathSFX, transform.position);

        //change state and disable agent
        ChangeState(new EnemyDead(this, m_animator, m_deathCleanupTime));
        m_agent.enabled = false;

        //turn off each limb collider
        CapsuleCollider[] colliders = GetComponentsInChildren<CapsuleCollider>();
        foreach (CapsuleCollider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetTarget(other.transform);
            m_currentState.TriggerStart(other);
        }
    }

    public void ChangeState(EnemyState newState)
    {
        m_currentState = newState;
    }
}
