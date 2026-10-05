using UnityEngine;

public class GoblinRangedAnimEvent : EnemyAnimationEvent<GoblinRanged>
{
    public void ShootArrow()
    {
        m_enemyScript.LaunchArrow();
    }

    public void DrawString()
    {
        m_enemyScript.DrawString();
    }
}
