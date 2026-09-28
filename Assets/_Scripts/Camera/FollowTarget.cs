using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform m_followTarget;
    public Vector3 m_offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = m_followTarget.position + m_offset;
    }
}
