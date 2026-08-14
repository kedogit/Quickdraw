using UnityEngine;

public class EnemyIdle : EnemyState
{
    public EnemyIdle(Enemy enemyScript) : base(enemyScript)
    {
    }

    public override void Execute()
    {
        Debug.Log("idle");
    }

    public override void TriggerStart(Collider target)
    {
        //m_enemyScript.ChangeState(new EnemyChase(m_enemyScript, target.transform));
    }
}
