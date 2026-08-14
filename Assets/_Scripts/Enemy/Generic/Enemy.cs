using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BodyPart
{
    HEAD,
    BODY
}

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private float m_maxHP = 100f;
    [SerializeField] private float m_aggroRange = 10f;
    [SerializeField] private float m_attackDelay = 1f;
    [SerializeField] private float m_deathCleanupTime = 15f;
    [SerializeField] protected Animator m_animator;
    [SerializeField] private AudioClip m_hurtSFX;
    [SerializeField] private AudioClip m_deathSFX;
    [SerializeField] private float m_bodyMultiplier = 1f;
    [SerializeField] private float m_headshotMultiplier = 2f;

    protected NavMeshAgent m_agent;
    protected AudioSource m_audioSource;
    protected Transform m_target;

    private float m_attackTimer;
    private float m_currentHP;
    private float m_distanceFromPlayer;
    private bool m_isDead = false;
    protected bool m_canMove = true;

    private ArenaTrigger m_arenaScript;

    protected Dictionary<BodyPart, float> bodyDamageMultipliers;

    protected abstract void Attack();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        m_currentHP = m_maxHP;
        m_agent = GetComponent<NavMeshAgent>();
        m_audioSource = GetComponent<AudioSource>();

        //set timer so the enemy can attack immediately
        m_attackTimer = m_attackDelay;

        //create the body part dictionary for hurt function
        bodyDamageMultipliers = new Dictionary<BodyPart, float>();
        bodyDamageMultipliers.Add(BodyPart.BODY, m_bodyMultiplier);
        bodyDamageMultipliers.Add(BodyPart.HEAD, m_headshotMultiplier);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdatePositions();

        if (!m_isDead && m_target != null)
        {
            if (m_distanceFromPlayer <= m_agent.stoppingDistance)
            {
                LookAtPlayer();
                if (m_attackTimer >= m_attackDelay)
                {
                    Attack();
                    m_attackTimer = 0;
                }
            }

            if (m_canMove)
            {
                Move();
            }
        }
    }

    public void SetAggroRange(float aggroRange)
    {
        m_aggroRange = aggroRange;
    }

    public void SetArenaScript(ArenaTrigger script)
    {
        m_arenaScript = script;
    }

    private void UpdatePositions()
    {
        if (m_target != null)
        {
            m_distanceFromPlayer = Vector3.Distance(transform.position, m_target.position);
        }

        if (m_attackTimer < m_attackDelay)
        {
            m_attackTimer += Time.deltaTime;
        }
    }

    private void LookAtPlayer()
    {
        transform.LookAt(new Vector3(m_target.position.x, transform.position.y, m_target.position.z));
    }

    private void Move()
    {
        //if in aggro range, move towards player
        if (m_distanceFromPlayer <= m_aggroRange)
        {
            m_agent.isStopped = false;
            m_agent.SetDestination(m_target.position);
            m_animator.SetBool("isRunning", true);
        }
        else
        {
            //otherwise, clear path and go back to idle
            m_agent.ResetPath();
            m_agent.isStopped = true;
            m_animator.SetBool("isRunning", false);
        }
    }

    public void Hurt(float damageAmount, BodyPart partHit)
    {
        if (!m_isDead)
        {
            Observer.GetInstance().TriggerEvent(EVENT.ON_ENEMY_HURT);

            //play hurt sfx
            m_audioSource.clip = m_hurtSFX;
            m_audioSource.Play();

            //play animation and update HP
            Debug.Log("damage taken:" + (damageAmount * bodyDamageMultipliers[partHit]));
            m_animator.SetTrigger("Hurt");
            m_currentHP -= damageAmount * bodyDamageMultipliers[partHit];

            //if hp at 0, die
            if (m_currentHP <= 0)
            {
                Die();
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

        //start timer to remove body, play animation and disable components
        StartCoroutine(DeathCleanup());
        m_isDead = true;
        m_animator.SetTrigger("Death");
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
            m_target = other.transform;
        }
    }

    private IEnumerator DeathCleanup()
    {
        yield return new WaitForSeconds(m_deathCleanupTime);
        Destroy(this.gameObject);
    }
}
