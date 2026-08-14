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
    [SerializeField] private float m_maxHP = 100f;
    [SerializeField] protected float m_attackSpeed = 1f;
    [SerializeField] private float m_deathCleanupTime = 15f;
    [SerializeField] protected Animator m_animator;
    [SerializeField] private AudioClip m_hurtSFX;
    [SerializeField] private AudioClip m_deathSFX;
    [SerializeField] private float m_bodyMultiplier = 1f;
    [SerializeField] private float m_headshotMultiplier = 2f;

    public float AttackSpeed => m_attackSpeed;
    public Transform Target => m_target;

    protected NavMeshAgent m_agent;
    protected AudioSource m_audioSource;
    protected Transform m_target;

    private float m_currentHP;
    protected EnemySize m_enemySize;

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
        if (Vector3.Distance(this.transform.position, m_target.position) > m_agent.stoppingDistance)
        {
            ChangeState(new EnemyChase(this, m_animator));
        }
    }

    public void SetTarget(Transform player)
    {
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

            //update HP
            m_currentHP -= damageAmount * bodyDamageMultipliers[partHit];

            //if hp at 0, die
            if (m_currentHP <= 0)
            {
                Die();
            }
            else
            {
                //play hurt sfx
                m_audioSource.clip = m_hurtSFX;
                m_audioSource.Play();

                //set stagger state if small enemy
                if (m_enemySize == EnemySize.SMALL)
                {
                    //needed in case the player hurts an enemy that never spotted them
                    if (m_target == null)
                    {
                        SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
                    }

                    ChangeState(new EnemyHurt(this, m_animator));
                }
            }
        }
    }

    private void Die()
    {
        //if arena enemy, tell arena to increment kill count
        m_arenaScript?.RegisterKill();

        //play death sfx
        m_audioSource.clip = m_deathSFX;
        m_audioSource.Play();

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
