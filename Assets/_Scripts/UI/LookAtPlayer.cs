using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform m_player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(m_player);
    }
}
