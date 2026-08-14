using UnityEngine;

public class OrcAnimEvent : MonoBehaviour
{
    [SerializeField] private Orc m_enemyScript;

    public void HitboxStart()
    {
        m_enemyScript.EnableHitbox();
    }
}
