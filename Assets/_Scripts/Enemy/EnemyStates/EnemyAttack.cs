using UnityEngine;

public class EnemyAttack : EnemyState
{
    public EnemyAttack(Enemy enemyScript, Animator animator) : base(enemyScript, animator)
    {
        //simply calls the enemy's attack function once to allow it to do whatever special/unique attack behavior it has
        m_animator.SetBool("isRunning", false);
        m_enemyScript.Attack();
    }

    public override void Execute()
    {
        m_enemyScript.transform.LookAt(new Vector3(m_enemyScript.Target.position.x, m_enemyScript.transform.position.y, m_enemyScript.Target.position.z));
    }
}
