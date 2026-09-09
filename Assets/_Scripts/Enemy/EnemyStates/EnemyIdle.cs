using UnityEngine;

public class EnemyIdle : EnemyState
{
    public EnemyIdle(Enemy enemyScript, Animator animator) : base(enemyScript, animator)
    {
    }

    public override void Execute()
    {

    }

    public override void TriggerStart(Collider target)
    {
        m_enemyScript.ChangeState(new EnemyChase(m_enemyScript, m_animator));
    }
}
