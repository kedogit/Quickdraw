using UnityEngine;

public abstract class EnemyAnimationEvent<T> : MonoBehaviour where T : Enemy
{
    [Header("Main Script")]
    [SerializeField] protected T m_enemyScript;

    protected float m_attackSpeed;

    private void Start()
    {
        m_attackSpeed = m_enemyScript.AttackSpeed;
    }

    public void DecideNextMove()
    {
        m_enemyScript.DecideNextMove();
    }
}
