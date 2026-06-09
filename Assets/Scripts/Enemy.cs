using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private float m_maxHP = 100f;
    [SerializeField] private float m_aggroRange = 10f;
    [SerializeField] private float m_attackRange = 1f;
    [SerializeField] private float m_deathCleanupTime = 15;
    [SerializeField] private Animator m_animator;

    private NavMeshAgent m_agent;

    private GameObject m_player;
    private float m_elapsed;
    private float m_currentHP;
    private float m_distanceFromPlayer;
    private Vector3 m_playerPosition;
    private bool m_isDead = false;

    protected abstract void Attack();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        m_currentHP = m_maxHP;
        m_agent = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdatePositions();

        if (!m_isDead)
        {
            Move();
            if (m_distanceFromPlayer <= m_attackRange)
            {
                Attack();
            }
            HitboxUpdate();
        }
        else
        {
            m_elapsed += Time.deltaTime;
            //waits a certain amount of time before destroying the body
            if (m_elapsed >= m_deathCleanupTime)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void UpdatePositions()
    {
        m_playerPosition = m_player.transform.position;
        m_distanceFromPlayer = Vector3.Distance(transform.position, m_playerPosition);
        m_elapsed += Time.deltaTime;
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

    private void HitboxUpdate()
    {

    }


}
