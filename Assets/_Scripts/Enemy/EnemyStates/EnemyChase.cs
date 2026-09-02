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
        Debug.Log("chasing");
        Vector3 targetPos = m_enemyScript.Target.position;
        m_navAgent.SetDestination(targetPos);
        if (m_navAgent.remainingDistance <= m_navAgent.stoppingDistance && m_navAgent.remainingDistance > 0)
        {
            m_navAgent.ResetPath();
            m_navAgent.velocity = Vector3.zero;
            m_enemyScript.ChangeState(new EnemyAttack(m_enemyScript, m_animator));
        }
    }
}