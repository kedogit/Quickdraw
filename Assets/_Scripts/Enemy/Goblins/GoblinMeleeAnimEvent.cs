using UnityEngine;

public class GoblinMeleeAnimEvent : MonoBehaviour
{
    [SerializeField] private GoblinMelee m_enemyScript;

    public void HitboxStart()
    {
        m_enemyScript.EnableWeaponHitbox(SFX.GOBLIN_ATTACK1);
    }

    public void DecideNextMove()
    {
        m_enemyScript.DecideNextMove();
    }
}
