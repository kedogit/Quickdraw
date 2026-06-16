using UnityEngine;

public class GoblinRangedAnimEvent : MonoBehaviour
{
    private GoblinRanged m_enemyScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_enemyScript = transform.root.gameObject.GetComponent<GoblinRanged>();
    }

    public void ShootArrow()
    {
        m_enemyScript.LaunchArrow();
    }
}
