using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 m_target;
    private float m_projectileSpeed;
    private float m_distanceTravelled;
    private float m_maxRange;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.LookAt(m_target);
    }

    // Update is called once per frame
    void Update()
    {
        m_distanceTravelled += m_projectileSpeed * Time.deltaTime;
        Debug.Log(m_distanceTravelled);

        if (m_distanceTravelled >= m_maxRange)
        {
            Destroy(this.gameObject);
        }


        float step = m_projectileSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, m_target, step);
    }

    public void SetTarget(Vector3 target)
    {
        m_target = target;
    }

    public void SetSpeed(float speed)
    {
        m_projectileSpeed = speed;
    }

    public void SetRange(float maxRange)
    {
        m_maxRange = maxRange;
    }
}
