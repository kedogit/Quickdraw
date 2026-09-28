using UnityEngine;
using UnityEngine.InputSystem;

public class ViewBobbing : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_inputActionAsset;

    [Header("Controls")]
    [SerializeField] private bool m_doSway = true;
    [SerializeField] private bool m_doSwayRotation = true;
    [SerializeField] private bool m_doBob = true;
    [SerializeField] private bool m_doBobRotation = true;

    [Header("Sway")]
    [SerializeField] private float m_swayIntensity = 0.01f;
    [SerializeField] private float m_maxSwayDistance = 0.06f;

    [Header("Sway Rotation")]
    [SerializeField] private float m_swayRotateIntensity = 4f;
    [SerializeField] private float m_maxSwayRotation = 5f;

    [Header("Smoothing")]
    [SerializeField] private float m_moveSmoothing = 10f;
    [SerializeField] private float m_rotationSmoothing = 10f;

    [Header("Bob")]
    [SerializeField] private Vector3 m_bobIntensity = Vector3.one * 0.025f;
    [SerializeField] private Vector3 m_maxBob = Vector3.one * 0.01f;

    [Header("Bob Rotation")]
    [SerializeField] private Vector3 m_bobRotationIntensity;

    private InputAction m_moveAction;
    private InputAction m_lookAction;
    private Rigidbody m_playerBody;
    private Player m_playerScript;

    private Vector2 m_moveVector;
    private Vector2 m_lookVector;

    private Vector3 m_swayPosition;
    private Vector3 m_swayRotation;

    private float m_speedCurve;
    private Vector3 m_bobPosition;
    private Vector3 m_bobRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_moveAction = m_inputActionAsset.FindAction("Move");
        m_lookAction = m_inputActionAsset.FindAction("Look");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_playerBody = player?.GetComponent<Rigidbody>();
        m_playerScript = player?.GetComponent<Player>();
    }


    private void UpdateVectors()
    {
        m_moveVector = m_moveAction.ReadValue<Vector2>().normalized;
        m_lookVector = m_lookAction.ReadValue<Vector2>().normalized;
    }

    private void CalculateSwayMovement()
    {
        if (!m_doSway)
        {
            m_swayPosition = Vector3.zero;
            return;
        }

        Vector3 invertedLook = m_lookVector * -m_swayIntensity;
        invertedLook.x = Mathf.Clamp(invertedLook.x, -m_maxSwayDistance, m_maxSwayDistance);
        invertedLook.y = Mathf.Clamp(invertedLook.y, -m_maxSwayDistance, m_maxSwayDistance);

        m_swayPosition = invertedLook;
    }

    private void CalculateSwayRotation()
    {
        if (!m_doSwayRotation)
        {
            m_swayRotation = Vector3.zero;
            return;
        }

        Vector2 invertedLook = m_lookVector * -m_swayRotateIntensity;
        invertedLook.x = Mathf.Clamp(invertedLook.x, -m_maxSwayRotation, m_maxSwayRotation);
        invertedLook.y = Mathf.Clamp(-invertedLook.y, -m_maxSwayRotation, m_maxSwayRotation);

        m_swayRotation = new Vector3(invertedLook.y, invertedLook.x, invertedLook.x);
    }

    private float GetSin()
    {
        return Mathf.Sin(m_speedCurve);
    }

    private float GetCos()
    {
        return Mathf.Cos(m_speedCurve);
    }

    private void CalculateBobMovement()
    {
        bool playerGrounded = m_playerScript.CheckGrounded();

        m_speedCurve += Time.deltaTime * (playerGrounded ? m_playerBody.linearVelocity.magnitude : 1f) + 0.01f;

        if (!m_doBob)
        {
            m_bobPosition = Vector3.zero;
            return;
        }

        m_bobPosition.x = (GetCos() * m_maxBob.x * (playerGrounded ? 1 : 0)) - (m_moveVector.x * m_bobIntensity.x);
        m_bobPosition.y = (GetSin() * m_maxBob.y) - (m_playerBody.linearVelocity.y * m_bobIntensity.y);
        m_bobPosition.z = -(m_moveVector.y * m_maxBob.z);
    }

    private void CalculateBobRotation()
    {
        if (!m_doBobRotation)
        {
            m_bobRotation = Vector3.zero;
            return;
        }

        m_bobRotation.x = (m_moveVector != Vector2.zero ? m_bobRotationIntensity.x * (Mathf.Sin(2 * m_speedCurve)) : m_bobRotationIntensity.x * (Mathf.Sin(2 * m_speedCurve) / 2));
        m_bobRotation.y = (m_moveVector != Vector2.zero ? m_bobRotationIntensity.y * GetCos() : 0);
        m_bobRotation.z = (m_moveVector != Vector2.zero ? m_bobRotationIntensity.z * GetCos() * m_moveVector.x : 0);
    }

    private void ApplySwayAndBob()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, m_swayPosition + m_bobPosition, Time.deltaTime * m_moveSmoothing);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(m_swayRotation) * Quaternion.Euler(m_bobRotation), Time.deltaTime * m_rotationSmoothing);
    }

    void Update()
    {
        UpdateVectors();
        CalculateSwayMovement();
        CalculateSwayRotation();
        CalculateBobMovement();
        CalculateBobRotation();
        ApplySwayAndBob();
    }
}
