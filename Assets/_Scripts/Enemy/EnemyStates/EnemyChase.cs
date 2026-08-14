using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : EnemyState
{
    private NavMeshAgent m_navAgent;

    public EnemyChase(Enemy enemyScript, Animator animator) : base(enemyScript, animator)
    {
        m_navAgent = enemyScript.GetComponent<NavMeshAgent>();
        animator.SetBool("isRunning", true);
        animator.speed = 1;
    }

    public override void Execute()
    {
        Vector3 targetPos = m_enemyScript.Target.position;
        Debug.Log("chasing target at " + targetPos);
        m_navAgent.SetDestination(targetPos);
        if (Vector3.Distance(m_enemyScript.transform.position, targetPos) <= m_navAgent.stoppingDistance)
        {
            Debug.Log("target in range");
            m_navAgent.ResetPath();
            m_navAgent.velocity = Vector3.zero;
            m_enemyScript.ChangeState(new EnemyAttack(m_enemyScript, m_animator));
        }
    }
}