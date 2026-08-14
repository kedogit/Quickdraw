using UnityEngine;
using UnityEngine.AI;

public class EnemyHurt : EnemyState
{
    public float m_elapsed;
    public const float m_staggerTime = 0.5f;
    public EnemyHurt(Enemy enemyScript, Animator animator) : base(enemyScript, animator)
    {
        NavMeshAgent navAgent = enemyScript.GetComponent<NavMeshAgent>();
        navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;

        animator.SetTrigger("Hurt");
        animator.speed = 1;
    }

    public override void Execute()
    {
        m_elapsed += Time.deltaTime;
        if (m_elapsed > m_staggerTime)
        {
            m_enemyScript.ChangeState(new EnemyChase(m_enemyScript, m_animator));
        }
    }
}
