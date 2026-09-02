using UnityEngine;

public class EnemyAttack : EnemyState
{
    private float m_elapsed;
    private float m_attackSpeed;

    public EnemyAttack(Enemy enemyScript, Animator animator) : base(enemyScript, animator)
    {
        m_elapsed = Mathf.Infinity;
        m_attackSpeed = enemyScript.AttackSpeed;
    }

    public override void Execute()
    {
        Debug.Log("attacking");
        m_enemyScript.transform.LookAt(new Vector3(m_enemyScript.Target.position.x, m_enemyScript.transform.position.y, m_enemyScript.Target.position.z));
        m_elapsed += Time.deltaTime;
        if (m_elapsed >= m_attackSpeed)
        {
            m_enemyScript.Attack();
            m_elapsed = 0f;
        }
    }
}
