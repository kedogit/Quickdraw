using UnityEngine;

public class GoblinMeleeAnimEvent : MonoBehaviour
{
    [SerializeField] private GoblinMelee m_enemyScript;

    public void HitboxStart()
    {
        m_enemyScript.EnableHitbox();
    }

    public void CheckDistance()
    {
        m_enemyScript.CheckDistance();
    }
}
