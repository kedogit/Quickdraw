using UnityEngine;

public class GoblinRangedAnimEvent : MonoBehaviour
{
    [SerializeField] private GoblinRanged m_enemyScript;

    public void ShootArrow()
    {
        m_enemyScript.LaunchArrow();
    }

    public void DrawString()
    {
        m_enemyScript.DrawString();
    }

    public void DecideNextMove()
    {
        m_enemyScript.DecideNextMove();
    }
}
