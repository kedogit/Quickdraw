using UnityEngine;

public class GoblinMeleeAnimEvent : MonoBehaviour
{
    private GoblinMelee m_enemyScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_enemyScript = transform.root.gameObject.GetComponent<GoblinMelee>();
    }

    public void HitboxStart()
    {
        m_enemyScript.EnableHitbox();
    }
}
