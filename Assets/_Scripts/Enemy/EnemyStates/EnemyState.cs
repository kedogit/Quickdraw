using UnityEngine;

public abstract class EnemyState
{
    protected Enemy m_enemyScript;

    public EnemyState(Enemy enemyScript)
    {
        m_enemyScript = enemyScript;
    }

    public abstract void Execute();

    public virtual void TriggerStart(Collider target)
    {

    }

    public virtual void TriggerEnd()
    {

    }

}
