using UnityEngine;
using UnityEngine.AI;

public class EnemyStunned : EnemyState
{
    private float m_elapsed;
    private float m_stunDuration;

    public EnemyStunned(Enemy enemyScript, Animator animator, float stunDuration) : base(enemyScript, animator)
    {
        NavMeshAgent navAgent = enemyScript.GetComponent<NavMeshAgent>();
        navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;

        m_stunDuration = stunDuration;

        animator.SetBool("isStunned", true);
        animator.speed = 1;
    }

    public override void Execute()
    {
        m_elapsed += Time.deltaTime;
        if (m_elapsed > m_stunDuration)
        {
            m_animator.SetBool("isStunned", false);
            m_enemyScript.ResetStun();
            m_enemyScript.ChangeState(new EnemyChase(m_enemyScript, m_animator));
        }
    }
}
