using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.Controls.AxisControl;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_inputActionAsset;

    private Player m_playerScript;
    private InputAction m_look;
    private Transform m_playerTransform;
    private float m_mouseSens;
    private float m_verticalRotation;
    private float m_horizontalRotation;

    private const float m_clamp = 90f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        m_playerTransform = playerGO.transform;
        m_playerScript = playerGO.GetComponent<Player>();
        m_look = m_inputActionAsset.FindAction("Look");
        UpdateMouseSens();

        m_verticalRotation = 0f;
        m_horizontalRotation = m_playerTransform.eulerAngles.y;

        Observer.GetInstance()?.SubscribeTo(EVENT.ON_SENS_CHANGE, UpdateMouseSens);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = m_playerTransform.position;
        Vector3 lookVector = m_look.ReadValue<Vector2>();

        //vertical rotation
        float lookY = lookVector.y * m_mouseSens;
        m_verticalRotation -= lookY;
        m_verticalRotation = Mathf.Clamp(m_verticalRotation, -m_clamp, m_clamp);
        transform.localRotation = Quaternion.Euler(m_verticalRotation, 0, 0);

        //horizontal rotation
        float lookX = lookVector.x * m_mouseSens;
        m_horizontalRotation += lookX;

        //rotate
        transform.rotation = Quaternion.Euler(m_verticalRotation, m_horizontalRotation, 0f);
        m_playerTransform.rotation = Quaternion.Euler(0f, m_horizontalRotation, 0f);
    }

    void UpdateMouseSens()
    {
        //m_mouseSens = m_playerScript.MouseSens;
    }
}
