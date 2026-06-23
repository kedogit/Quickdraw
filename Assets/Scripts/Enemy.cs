using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private float m_maxHP = 100f;
    [SerializeField] private float m_aggroRange = 10f;
    [SerializeField] private float m_attackRange = 2.5f;
    [SerializeField] private float m_attackDelay = 1f;
    [SerializeField] private float m_deathCleanupTime = 15f;
    [SerializeField] protected Animator m_animator;
    [SerializeField] private AudioClip m_hurtSFX;
    [SerializeField] private AudioClip m_deathSFX;

    private NavMeshAgent m_agent;
    private BoxCollider m_collider;
    protected AudioSource m_audioSource;

    private GameObject m_player;
    private float m_attackTimer;
    private float m_currentHP;
    private float m_distanceFromPlayer;
    protected Vector3 m_playerPosition;
    private bool m_isDead = false;
    protected bool m_canMove = true;

    protected abstract void Attack();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        m_currentHP = m_maxHP;
        m_agent = GetComponent<NavMeshAgent>();
        m_collider = GetComponent<BoxCollider>();
        m_audioSource = GetComponent<AudioSource>();
        m_player = GameObject.FindWithTag("Player");
        m_agent.stoppingDistance = m_attackRange;
        m_attackTimer = m_attackDelay;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdatePositions();

        if (!m_isDead)
        {
            if (m_canMove)
            {
                Move();
            }
            if (m_distanceFromPlayer <= m_attackRange)
            {
                if (m_attackTimer >= m_attackDelay)
                {
                    Attack();
                    m_attackTimer = 0;
                }
            }
        }
    }

    private void UpdatePositions()
    {
        m_playerPosition = m_player.transform.position;
        m_distanceFromPlayer = Vector3.Distance(transform.position, m_playerPosition);
        if (m_attackTimer < m_attackDelay)
        {
            m_attackTimer += Time.deltaTime;
        }
    }

    private void Move()
    {
        //if in aggro range, move towards player
        if (m_distanceFromPlayer <= m_aggroRange)
        {
            m_agent.SetDestination(m_playerPosition);
            m_animator.SetBool("isRunning", true);
        }
        //if at its destination, stop the animation
        if (m_agent.remainingDistance - m_agent.stoppingDistance <= 0)
        {
            m_animator.SetBool("isRunning", false);
        }
    }

    public void Hurt(float damageAmount)
    {
        if (!m_isDead)
        {
            //play hurt sfx
            m_audioSource.clip = m_hurtSFX;
            m_audioSource.Play();

            //play animation and update HP
            Debug.Log("damage taken:" + damageAmount);
            m_animator.SetTrigger("Hurt");
            m_currentHP -= damageAmount;

            //if hp at 0, die
            if (m_currentHP <= 0)
            {
                Die();
            } 
        }
    }

    private void Die()
    {
        //play death sfx
        m_audioSource.clip = m_deathSFX;
        m_audioSource.Play();

        //start timer to remove body, play animation and disable components
        StartCoroutine(DeathCleanup());
        m_isDead = true;
        m_animator.SetTrigger("Death");
        m_collider.enabled = false;
        m_agent.enabled = false;
    }

    private IEnumerator DeathCleanup()
    {
        yield return new WaitForSeconds(m_deathCleanupTime);
        Destroy(this.gameObject);
    }
}
