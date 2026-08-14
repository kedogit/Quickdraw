using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : EnemyState
{
    private Transform m_chaseTarget;
    private NavMeshAgent m_navAgent;

    public EnemyChase(Enemy enemyScript, Transform target) : base(enemyScript)
    {
        m_navAgent = enemyScript.GetComponent<NavMeshAgent>();
        m_chaseTarget = target;
        //enemyScript.m_animator.SetBool("isRunning", true);
    }

    public override void Execute()
    {
        m_navAgent.SetDestination(m_chaseTarget.position);
        if (Vector3.Distance(m_enemyScript.transform.position, m_chaseTarget.position) <= m_navAgent.stoppingDistance)
        {
            //m_enemyScript.m_animator.SetBool("isRunning", false);
            //m_enemyScript.ChangeState(new EnemyAttack(m_enemyScript, m_chaseTarget));
        }
    }
}