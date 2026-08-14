using System.Collections;
using UnityEngine;

public class EnemyAttack : EnemyState
{
    protected Transform m_target;

    public EnemyAttack(Enemy enemyScript, Transform target) : base(enemyScript)
    {
        //m_enemyScript.StartAttacking(target);
        //m_target = target;
    }

    public override void Execute()
    {
        //m_enemyScript.Attack();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
