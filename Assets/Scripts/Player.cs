using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //drag and drop
    [SerializeField] private Camera m_playerCam;
    [SerializeField] private InputActionAsset m_actions;

    //walk
    [SerializeField] private float m_moveSpeed = 5f;

    //jump
    [SerializeField] private float m_jumpForce = 5f;
    [SerializeField] private float m_doubleJumpUpForce = 2f;
    [SerializeField] private float m_doubleJumpForwardForce = 4f;

    //dash
    [SerializeField] private float m_dashForce = 5f;
    [SerializeField] private float m_dashCooldown = 2f;

    //look
    [SerializeField] private float m_clamp = 90f;
    [SerializeField] private float m_mouseSensHor = 100f;
    [SerializeField] private float m_mouseSensVert = 1.0f;

    private float m_dashTimer;
    private bool m_dashReady;

    private Vector3 m_moveVector;

    private InputAction m_move;
    private InputAction m_look;
    private InputAction m_jump;
    private InputAction m_dash;
    private Rigidbody m_body;

    private int m_jumpCount;
    private bool m_isGrounded;
    private float m_verticalRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_look = m_actions.FindAction("Look");
        m_jump = m_actions.FindAction("Jump");
        m_dash = m_actions.FindAction("Dash");
        m_move = m_actions.FindAction("Move");
        m_body = GetComponent<Rigidbody>();
        m_body.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateVectors();
        Look();
        Walk();
        Jump();
        Dash();
    }

    private void CalculateVectors()
    {
        //get the transforms to gauge current direction
        Vector3 forwardDirection = transform.forward;
        Vector3 sideDirection = transform.right;

        //get the player's inputs
        Vector2 inputVector = m_move.ReadValue<Vector2>();

        //create a movement vector by multiplying the inputs with their directions
        m_moveVector = (forwardDirection * inputVector.y + sideDirection * inputVector.x);
    }

    private void Walk()
    {
        //move the body's position
        m_body.MovePosition(m_body.position + m_moveVector * m_moveSpeed * Time.deltaTime);
    }

    private void Look()
    {
        Vector3 lookVector = m_look.ReadValue<Vector2>();

        //horizontal
        float lookX = lookVector.x * m_mouseSensHor;
        transform.Rotate(Vector3.up, lookX);

        //vertical
        float lookY = lookVector.y * m_mouseSensVert;
        m_verticalRotation -= lookY;
        m_verticalRotation = Mathf.Clamp(m_verticalRotation, -m_clamp, m_clamp);
        m_playerCam.transform.localRotation = Quaternion.Euler(m_verticalRotation, 0, 0);
    }

    private void Jump()
    {
        if (m_jump.WasPressedThisFrame())
        {
            //if on the ground, regular jump
            if (m_isGrounded)
            {
                Vector3 up = Vector3.up * m_jumpForce;
                m_body.AddForce(up, ForceMode.Impulse);
                m_isGrounded = false;
                m_jumpCount++;
            }
            //if not on the ground, double jump force (forward force)
            else if (m_jumpCount < 2)
            {
                Vector3 upAndForward = Vector3.up * m_doubleJumpUpForce + transform.forward * m_doubleJumpForwardForce;
                m_body.AddForce(upAndForward, ForceMode.Impulse);
                m_jumpCount++;
            }
        }
    }

    private void Dash()
    {
        if (!m_dashReady)
        {
            m_dashTimer += Time.deltaTime;
        }

        if (m_dashTimer >= m_dashCooldown)
        {
            m_dashReady = true;
        }

        if (m_dash.WasPressedThisFrame() && m_dashReady)
        {
            Vector2 inputVector = m_move.ReadValue<Vector2>();
            Vector3 dashDirection;

            //if not pressing any movement keys, dash forward. otherwise, dash in the input direction
            if (inputVector == Vector2.zero)
            {
                dashDirection = transform.forward;
            }
            else
            {
                dashDirection = m_moveVector;
            }
            dashDirection *= m_dashForce;

            //currently goes forward, should go in movement input direction
            //Vector3 forward = transform.forward * m_dashForce;
            m_body.AddForce(dashDirection, ForceMode.Impulse);
            m_dashTimer = 0;
            m_dashReady = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_isGrounded = true;
            m_jumpCount = 0;
        }
    }
}
