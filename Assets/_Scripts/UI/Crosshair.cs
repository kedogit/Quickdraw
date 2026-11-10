using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private RectTransform m_reticleUp;
    [SerializeField] private RectTransform m_reticleDown;
    [SerializeField] private RectTransform m_reticleLeft;
    [SerializeField] private RectTransform m_reticleRight;

    [SerializeField] private float m_totalRotation = 45f;
    [SerializeField] private float m_startOffset = 10f;
    [SerializeField] private float m_endOffset = 2f;

    private Player m_playerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        float bowCharge = m_playerScript.BowChargeLevel;

        Vector3 rotation = transform.eulerAngles;
        rotation.z = bowCharge * m_totalRotation;
        transform.eulerAngles = rotation;

        Vector2 position = m_reticleUp.anchoredPosition;
        position.y = m_startOffset - bowCharge * (m_startOffset - m_endOffset);
        m_reticleUp.anchoredPosition = position;

        position = m_reticleRight.anchoredPosition;
        position.x = m_startOffset - bowCharge * (m_startOffset - m_endOffset);
        m_reticleRight.anchoredPosition = position;

        position = m_reticleDown.anchoredPosition;
        position.y = -(m_startOffset - bowCharge * (m_startOffset - m_endOffset));
        m_reticleDown.anchoredPosition = position;

        position = m_reticleLeft.anchoredPosition;
        position.x = -(m_startOffset - bowCharge * (m_startOffset - m_endOffset));
        m_reticleLeft.anchoredPosition = position;
    }
}
