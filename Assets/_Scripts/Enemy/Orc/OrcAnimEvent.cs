using UnityEngine;

public class OrcAnimEvent : MonoBehaviour
{
    [SerializeField] private Orc m_enemyScript;

    public void HitboxStart()
    {
        m_enemyScript.EnableWeaponHitbox(SFX.ORC_ATTACK1);
    }

    public void CheckDistance()
    {
        m_enemyScript.CheckDistance();
    }
}
