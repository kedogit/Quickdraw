using UnityEngine;

public abstract class EnemyState
{
    protected Enemy m_enemyScript;
    protected Animator m_animator;

    public EnemyState(Enemy enemyScript, Animator animator)
    {
        m_enemyScript = enemyScript;
        m_animator = animator;
    }

    public abstract void Execute();

    public virtual void TriggerStart(Collider target)
    {

    }

    public virtual void TriggerEnd()
    {

    }

}
