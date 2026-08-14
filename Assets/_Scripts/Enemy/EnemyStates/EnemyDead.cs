using UnityEngine;
using UnityEngine.AI;

public class EnemyDead : EnemyState
{
    private float m_elapsed;
    private float m_cleanupTime;

    public EnemyDead(Enemy enemyScript, Animator animator, float cleanupTime) : base(enemyScript, animator)
    {
        m_cleanupTime = cleanupTime;

        NavMeshAgent navAgent = enemyScript.GetComponent<NavMeshAgent>();
        navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;

        animator.SetTrigger("Death");
        animator.speed = 1;
    }

    public override void Execute()
    {
        m_elapsed += Time.deltaTime;
        if (m_elapsed >= m_cleanupTime)
        {
            MonoBehaviour.Destroy(m_enemyScript.gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
