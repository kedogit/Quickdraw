using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera m_playerCam;
    [SerializeField] private InputActionAsset m_actions;
    [SerializeField] private GameObject m_hookHead;

    [Header("Run")]
    [SerializeField] private float m_moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float m_jumpForce = 5f;
    [SerializeField] private float m_doubleJumpUpForce = 2f;
    [SerializeField] private float m_doubleJumpForwardForce = 4f;

    [Header("Dash")]
    [SerializeField] private float m_dashGroundedUpForce = 3f;
    [SerializeField] private float m_dashForce = 5f;
    [SerializeField] private float m_dashCooldown = 2f;

    [Header("Camera")]
    [SerializeField] private float m_clamp = 90f;
    [SerializeField] private float m_mouseSensHor = 100f;
    [SerializeField] private float m_mouseSensVert = 1.0f;

    [Header("Hook")]
    [SerializeField] private float m_hookDrawSpeed = 5f;
    [SerializeField] private float m_hookRange = 20f;
    [SerializeField] private float m_jointMinDistance = 0.8f;
    [SerializeField] private float m_jointMaxDistance = 0.25f;
    [SerializeField] private float m_jointSpring = 4.5f;
    [SerializeField] private float m_jointDamper = 7f;
    [SerializeField] private float m_jointMassScale = 4.5f;

    private float m_dashTimer;
    private bool m_dashReady;

    private Vector3 m_moveVector;

    private InputAction m_move;
    private InputAction m_look;
    private InputAction m_jump;
    private InputAction m_dash;
    private InputAction m_hook;
    private Rigidbody m_body;

    private int m_jumpCount;
    private bool m_isGrounded;
    private float m_verticalRotation = 0f;

    private Vector3 m_currentHookPosition;
    private Vector3 m_hookAnchor;
    private SpringJoint m_joint;
    private Transform m_hookStartPoint;
    private LineRenderer m_hookLineRenderer;
    private Transform m_hookHeadInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_look = m_actions.FindAction("Look");
        m_jump = m_actions.FindAction("Jump");
        m_dash = m_actions.FindAction("Dash");
        m_move = m_actions.FindAction("Move");
        m_hook = m_actions.FindAction("Hook");

        m_body = GetComponent<Rigidbody>();
        m_hookLineRenderer = GetComponent<LineRenderer>();
        m_hookStartPoint = transform.Find("HookStart");

        m_body.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateVectors();
        Look();
        Walk();
        JumpHandling();
        Dash();
        Grapple();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        //if a joint exists (grapple is active)
        if (m_joint)
        {
            //lerp for gradual creation of the line
            m_currentHookPosition = Vector3.Lerp(m_currentHookPosition, m_hookAnchor, Time.deltaTime * m_hookDrawSpeed);
            
            //set start and end point for the line renderer
            m_hookLineRenderer.SetPosition(0, m_hookStartPoint.position);
            m_hookLineRenderer.SetPosition(1, m_currentHookPosition);

            m_hookHeadInstance.position = m_currentHookPosition;
        }
    }

    private void Grapple()
    {
        if (m_hook.WasPressedThisFrame())
        {
            if (!m_joint)
            {
                //send a raycast to find a hit target (ADD LAYERMASK TO THE RAYCAST CALL LATER)
                RaycastHit hit;
                if (Physics.Raycast(m_playerCam.transform.position, m_playerCam.transform.forward, out hit, m_hookRange))
                {
                    //store the point of impact, add a spring joint to the player and configure parameters
                    m_hookAnchor = hit.point;
                    m_joint = gameObject.AddComponent<SpringJoint>();
                    m_joint.autoConfigureConnectedAnchor = false;
                    m_joint.connectedAnchor = m_hookAnchor;

                    float distanceFromPoint = Vector3.Distance(transform.position, m_hookAnchor);

                    m_joint.maxDistance = distanceFromPoint * m_jointMinDistance;
                    m_joint.minDistance = distanceFromPoint * m_jointMaxDistance;

                    m_joint.spring = m_jointSpring;
                    m_joint.damper = m_jointDamper;
                    m_joint.massScale = m_jointMassScale;

                    //set line renderer parameters and vars to ready it for the drawrope function
                    m_hookLineRenderer.positionCount = 2;
                    m_currentHookPosition = m_hookStartPoint.position;

                    m_hookHeadInstance = Instantiate(m_hookHead, m_currentHookPosition, Quaternion.identity).transform;
                    m_hookHeadInstance.LookAt(m_hookAnchor);
                }
            }
            //if click again and hook is already present, destroy
            else
            {
                m_hookLineRenderer.positionCount = 0;
                Destroy(m_joint);
                Destroy(m_hookHeadInstance.gameObject);
            }
        }
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

    private void JumpHandling()
    {
        if (m_jump.WasPressedThisFrame())
        {
            //if on the ground, regular jump
            if (m_isGrounded)
            {
                Vector3 up = Vector3.up * m_jumpForce;
                Jump(up);
                m_isGrounded = false;
            }
            //if not on the ground, double jump force (forward force)
            else if (m_jumpCount < 2)
            {
                Vector3 upAndForward = Vector3.up * m_doubleJumpUpForce + transform.forward * m_doubleJumpForwardForce;
                Jump(upAndForward);
            }
        }
    }

    private void ResetVelocity(char axis)
    {
        Vector3 linearVelocity = m_body.linearVelocity;
        switch (axis)
        {
            case 'X':
                linearVelocity.x = 0;
                break;
            case 'Y':
                linearVelocity.y = 0;
                break;
            case 'Z':
                linearVelocity.z = 0;
                break;
        }
        m_body.linearVelocity = linearVelocity;
    }

    private void Jump(Vector3 force)
    {
        if (m_body.linearVelocity.y < 0)
        {
            ResetVelocity('Y');
        }
        m_body.AddForce(force, ForceMode.Impulse);
        m_jumpCount++;
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
            if (m_isGrounded)
            {
                dashDirection += Vector3.up * m_dashGroundedUpForce;
            }


            //currently goes forward, should go in movement input direction
            //Vector3 forward = transform.forward * m_dashForce;
            if (m_body.linearVelocity.x < 0)
            {
                ResetVelocity('X');
            }
            if (m_body.linearVelocity.z < 0)
            {
                ResetVelocity('Z');
            }

            //dot product tests, implement instead of the linearvelocity checks
            if (Vector3.Dot(m_body.linearVelocity, dashDirection) < -0.8)
            {
                Debug.Log("opposite direction");
            }
            if (Vector3.Dot(m_body.linearVelocity, dashDirection) > 0.8)
            {
                Debug.Log("same direction");
            }
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
