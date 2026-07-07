using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("References - General")]
    [SerializeField] private Camera m_playerCam;
    [SerializeField] private InputActionAsset m_actions;
    [SerializeField] private GameObject m_hookHead;
    [SerializeField] private GameObject m_arrowPrefab;
    [SerializeField] private Animator m_bowAnimator;
    [SerializeField] private PhysicsMaterial m_swingPhysicsMaterial;
    [SerializeField] private GameHUD m_gameHUD;

    [Header("References - Audio")]
    [SerializeField] private AudioClip m_shootBowSFX;
    [SerializeField] private AudioClip m_pullStringSFX;
    [SerializeField] private AudioClip m_dashSFX;
    [SerializeField] private AudioClip m_hurtSFX;
    [SerializeField] private AudioClip m_jumpSFX;
    [SerializeField] private AudioClip m_hookShootSFX;
    [SerializeField] private AudioClip m_hookLandSFX;
    [SerializeField] private AudioSource m_bowAudioSource;


    [Header("Health")]
    [SerializeField] private float m_maxHP = 100f;
    [SerializeField] private float m_invulnTime = 0.5f;

    [Header("WASD")]
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float m_airAcceleration = 1f;
    [SerializeField] private float m_airTurnSpeed = 1f;
    [SerializeField] private float m_grappleAcceleration = 1f;

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

    [Header("Hook - Swing")]
    [SerializeField] private float m_jointMinDistance = 0.8f;
    [SerializeField] private float m_jointMaxDistance = 0.25f;
    [SerializeField] private float m_jointSpring = 4.5f;
    [SerializeField] private float m_jointDamper = 7f;
    [SerializeField] private float m_jointMassScale = 4.5f;

    [Header("Hook - Zip")]
    [SerializeField] private float m_zipSpeed = 1.8f;
    [SerializeField] private float m_zipHopHeight = 6f;

    [Header("Bow")]
    [SerializeField] private float m_bowFullChargeTime = 2.0f;
    [SerializeField] private float m_arrowForce = 60f;
    [SerializeField] private float m_bowTargetDistance = 15f;
    [SerializeField] private float m_reloadTime = 1.0f;
    [SerializeField] private float m_arrowDamage = 55f;

    //public getters for player states
    public bool IsGrounded => m_isGrounded;
    public float GrappleAcceleration => m_grappleAcceleration;
    public float AirAcceleration => m_airAcceleration;
    public float AirTurnSpeed => m_airTurnSpeed;
    public float MoveSpeed => m_moveSpeed;
    public Rigidbody RigidBody => m_body;
    public Vector3 MoveVector => m_moveVector;
    public Vector3 InputVector => m_inputVector;


    //input actions
    private InputAction m_move;
    private InputAction m_look;
    private InputAction m_jump;
    private InputAction m_dash;
    private InputAction m_hook;
    private InputAction m_shoot;
    private InputAction m_escMenu;

    //components
    private Rigidbody m_body;
    private CapsuleCollider m_collider;
    private AudioSource m_playerAudioSource;

    //menu
    private bool m_isPaused;

    //hp
    private float m_currentHP;
    private bool m_isInvuln;

    //movement
    private BasePlayerState m_currentState;
    private Vector3 m_moveVector;
    private Vector3 m_inputVector;

    //jump related
    private bool m_playerHasDoubleJump;
    private int m_jumpCount;
    private bool m_isGrounded;

    //dash related
    private float m_dashTimer;
    private bool m_dashReady;

    //camera related
    private float m_verticalRotation = 0f;

    //hook related
    private bool m_playerHasGrapple;
    private bool m_grappleInProgress;
    private float m_drawRopeTimer;
    private Vector3 m_currentHookPosition;
    private Vector3 m_hookAnchor;
    private SpringJoint m_joint;
    private Transform m_hookStartPoint;
    private LineRenderer m_hookLineRenderer;
    private Transform m_hookHeadInstance;
    private Action m_grappleAction;
    private Coroutine m_ropeCoroutine;

    //checkpoint
    private Vector3 m_checkpointPos;

    //bow related
    private const float m_readyToShootThreshold = 3;
    private float m_bowChargeTime;
    private float m_bowChargeNormalized;
    private GameObject m_currentArrow;
    private bool m_readyToFire;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_currentState = new PlayerStateGrounded(this);

        m_look = m_actions.FindAction("Look");
        m_jump = m_actions.FindAction("Jump");
        m_dash = m_actions.FindAction("Dash");
        m_move = m_actions.FindAction("Move");
        m_hook = m_actions.FindAction("Hook");
        m_shoot = m_actions.FindAction("Shoot");
        m_escMenu = m_actions.FindAction("ESCMenu");

        m_playerAudioSource = GetComponent<AudioSource>();
        m_body = GetComponent<Rigidbody>();
        m_hookLineRenderer = GetComponent<LineRenderer>();
        m_collider = GetComponent<CapsuleCollider>();
        m_hookStartPoint = transform.Find("HookStart");

        m_checkpointPos = transform.position;

        m_body.freezeRotation = true;

        m_currentHP = m_maxHP;

        StartCoroutine(LoadArrow());

        Cursor.lockState = CursorLockMode.Locked;

        m_gameHUD.UpdateHP(m_currentHP);
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        CalculateVectors();
        Look();
        m_currentState.Move();
        JumpHandling();
        Dash();
        Grapple();
        ReadyBow();
        GrappleExitHandling();
        EscMenuHandling();
        ShootArrow();
    }

    public void SetCheckpoint(Vector3 pos)
    {
        m_checkpointPos = pos;
        Debug.Log("checkpoint pos updated, " + m_checkpointPos);
    }

    #region DAMAGE

    public void Hurt(float damage)
    {
        if (!m_isInvuln)
        {
            //play sfx
            m_playerAudioSource.clip = m_hurtSFX;
            m_playerAudioSource.Play();

            //reduce hp and start invuln window
            m_currentHP -= damage;
            StartCoroutine(InvincibilityWindow());

            //update hud
            m_gameHUD.UpdateHP(m_currentHP);

            //die if 0 hp
            if (m_currentHP <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator InvincibilityWindow()
    {
        m_isInvuln = true;
        yield return new WaitForSeconds(m_invulnTime);
        m_isInvuln = false;
    }

    private void Die()
    {
        //placeholder, reload scene for now
        SceneManager.LoadScene("PrototypeLevel");
    }

    #endregion

    #region BOW

    private void ShootArrow()
    {
        if (m_shoot.WasReleasedThisFrame())
        {
            if (m_readyToFire)
            {
                //play sfx
                m_bowAudioSource.clip = m_shootBowSFX;
                m_bowAudioSource.Play();

                //find a transform ahead of where the player is looking
                Vector3 target = m_playerCam.transform.position + m_playerCam.transform.forward * m_bowTargetDistance;
                //Vector3 target = m_playerCam.transform.position + m_playerCam.transform.forward;

                //create a direction vector by subtracting arrow's position
                //Vector3 directionVector = target - m_currentArrow.transform.position;
                //directionVector.Normalize();

                //attach a rigidbody to the arrow and add the force to it
                Rigidbody arrowBody = m_currentArrow.AddComponent<Rigidbody>();

                m_currentArrow.transform.LookAt(target);
                m_currentArrow.transform.parent = null;
                m_currentArrow.transform.position = m_playerCam.transform.position;
                arrowBody.AddForce(m_playerCam.transform.forward * m_arrowForce * m_bowChargeNormalized, ForceMode.Impulse);

                //activate arrow hitbox
                BoxCollider arrowCollider = m_currentArrow.GetComponent<BoxCollider>();
                arrowCollider.enabled = true;

                //feed info to the projectile script
                Projectile arrowScript = m_currentArrow.GetComponent<Projectile>();
                arrowScript.SetActive();
                m_currentArrow.GetComponent<Projectile>().SetDamage(m_arrowDamage * m_bowChargeNormalized);

                //prepare next shot
                m_currentArrow = null;
                m_bowChargeTime = 0;
                m_bowAnimator.SetFloat("BowCharge", 0);
                m_bowAnimator.SetTrigger("Shot");
                m_readyToFire = false;
                StartCoroutine(LoadArrow());
            }
            else
            {
                //if released too early, reset charge
                m_bowChargeTime = 0;
                m_bowAnimator.SetFloat("BowCharge", 0);
            }
        }
    }

    private IEnumerator LoadArrow()
    {
        //wait for reload time, then instantiate new arrow
        yield return new WaitForSeconds(m_reloadTime);
        GameObject arrowHolder = GameObject.Find("ArrowHolder");
        m_currentArrow = Instantiate(m_arrowPrefab, arrowHolder.transform.position, arrowHolder.transform.rotation, arrowHolder.transform);
    }

    private void ReadyBow()
    {
        if (m_shoot.IsPressed() && m_currentArrow != null)
        {
            //at the start of the drawing animation, play the sfx
            if (m_bowChargeTime == 0)
            {
                m_bowAudioSource.clip = m_pullStringSFX;
                m_bowAudioSource.Play();
            }

            //every frame while held, add deltatime to charge time and update the normalized charge var
            m_bowChargeTime += Time.deltaTime;
            m_bowChargeNormalized = Mathf.InverseLerp(0, m_bowFullChargeTime, m_bowChargeTime);
            m_bowAnimator.SetFloat("BowCharge", m_bowChargeNormalized);

            //if charged a certain % of the way, player is ready to fire
            if (m_bowChargeTime >= m_bowFullChargeTime / m_readyToShootThreshold)
            {
                m_readyToFire = true;
            }
        }
    }
    #endregion

    #region GRAPPLE

    private void Grapple()
    {
        if (m_playerHasGrapple)
        {
            if (m_hook.WasPressedThisFrame())
            {
                //send a raycast to find a hit target (ADD LAYERMASK TO THE RAYCAST CALL LATER)
                RaycastHit hit;
                if (Physics.Raycast(m_playerCam.transform.position, m_playerCam.transform.forward, out hit, m_hookRange))
                {
                    m_playerAudioSource.clip = m_hookShootSFX;
                    m_playerAudioSource.Play();

                    //detect the target to determine whether it is a swing or zip action and store for later
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        m_grappleAction += Zip;
                    }
                    else
                    {
                        m_grappleAction += Swing;
                    }

                    //reset/update grappling variables
                    m_grappleInProgress = true;
                    m_drawRopeTimer = 0;
                    m_hookAnchor = hit.point;

                    //set line renderer parameters and vars to ready it for the drawrope function
                    m_hookLineRenderer.positionCount = 2;
                    m_currentHookPosition = m_hookStartPoint.position;

                    //instantiate the hook anchor model
                    m_hookHeadInstance = Instantiate(m_hookHead, m_currentHookPosition, Quaternion.identity).transform;
                    m_hookHeadInstance.LookAt(m_hookAnchor);

                    //start rope coroutine
                    m_ropeCoroutine = StartCoroutine(DrawRope());
                }
            }
        }
    }

    private IEnumerator DrawRope()
    {
        //while rope hasn't reached the anchor point and line renderer is still active, draw the rope gradually
        while (m_drawRopeTimer < m_hookDrawSpeed && m_hookLineRenderer.positionCount == 2)
        {
            m_drawRopeTimer += Time.deltaTime;

            //lerp for gradual creation of the line
            m_currentHookPosition = Vector3.Lerp(m_hookStartPoint.position, m_hookAnchor, m_drawRopeTimer / m_hookDrawSpeed);

            //set start and end point for the line renderer
            m_hookLineRenderer.SetPosition(0, m_hookStartPoint.position);
            m_hookLineRenderer.SetPosition(1, m_currentHookPosition);

            m_hookHeadInstance.position = m_currentHookPosition;
            yield return null;
        }

        //swing/zip
        m_grappleAction.Invoke();
        m_currentState = new PlayerStateGrappling(this);

        //play hook hit sfx
        m_playerAudioSource.clip = m_hookLandSFX;
        m_playerAudioSource.Play();

        //keep updating the start of the line renderer (player pos)
        while (m_hookLineRenderer.positionCount == 2)
        {
            m_hookLineRenderer.SetPosition(0, m_hookStartPoint.position);
            yield return null;
        }
    }

    private void GrappleExitHandling()
    {
        if (m_hook.WasReleasedThisFrame() && m_playerHasGrapple)
        {
            StopGrapple();
        }
    }

    private void StopGrapple()
    {
        //stop rope coroutine
        StopCoroutine(m_ropeCoroutine);

        //if currently hooking, clean up every grapple variable
        if (m_grappleInProgress)
        {
            m_hookLineRenderer.positionCount = 0;
            Destroy(m_joint);
            Destroy(m_hookHeadInstance.gameObject);
            m_collider.material = null;
            m_grappleAction = null;
            m_grappleInProgress = false;

            if (m_isGrounded)
            {
                m_currentState = new PlayerStateGrounded(this);
            }
            else
            {
                m_currentState = new PlayerStateAirborne(this);
            }
        }
    }

    private void Swing()
    {
        //set friction to null during hook swing
        m_collider.material = m_swingPhysicsMaterial;

        //store the point of impact, add a spring joint to the player
        m_joint = gameObject.AddComponent<SpringJoint>();
        m_joint.autoConfigureConnectedAnchor = false;
        m_joint.connectedAnchor = m_hookAnchor;

        //configure component params
        float distanceFromPoint = Vector3.Distance(transform.position, m_hookAnchor);
        m_joint.maxDistance = distanceFromPoint * m_jointMinDistance;
        m_joint.minDistance = distanceFromPoint * m_jointMaxDistance;

        m_joint.spring = m_jointSpring;
        m_joint.damper = m_jointDamper;
        m_joint.massScale = m_jointMassScale;
    }

    private void Zip()
    {
        //shoot player towards hook anchor and automatically end the grapple
        m_body.linearVelocity = Vector3.zero;
        m_body.AddForce((m_hookAnchor - transform.position) * m_zipSpeed + Vector3.up * m_zipHopHeight, ForceMode.Impulse);
        StopGrapple();
    }
    #endregion

    private void GroundCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 2f))
        {
            m_isGrounded = true;
        }
        else
        {
            m_isGrounded = false;
        }
    }

    private void EscMenuHandling()
    {
        if (m_escMenu.WasPressedThisFrame())
        {
            TogglePlayerInputs();
            m_gameHUD.ToggleEscMenu();
        }
    }

    public void TogglePlayerInputs()
    {
        if (m_isPaused)
        {
            m_actions.FindActionMap("Player").Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
        else
        {
            m_actions.FindActionMap("Player").Disable();
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
        }

        m_isPaused = !m_isPaused;
    }

    public void ChangeState(BasePlayerState state)
    {
        m_currentState = state;
    }

    private void CalculateVectors()
    {
        //get the transforms to gauge current direction
        Vector3 forwardDirection = transform.forward;
        Vector3 sideDirection = transform.right;

        //get the player's inputs
        m_inputVector = m_move.ReadValue<Vector2>();

        //create a movement vector by multiplying the inputs with their directions
        m_moveVector = (forwardDirection * m_inputVector.y + sideDirection * m_inputVector.x);
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
            if (m_isGrounded && m_jumpCount < 1)
            {
                Vector3 up = Vector3.up * m_jumpForce;
                Jump(up);
                m_isGrounded = false;
                m_currentState = new PlayerStateAirborne(this);
            }
            //if not on the ground, double jump force (forward force)
            else if (m_jumpCount < 2 && m_playerHasDoubleJump)
            {
                Vector3 upAndForward = Vector3.up * m_doubleJumpUpForce + transform.forward * m_doubleJumpForwardForce;
                Jump(upAndForward);
            }
        }
    }

    //function to reset velocity for the jump and dash functions
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
        //if velocity is negative, set it to 0
        if (m_body.linearVelocity.y < 0)
        {
            ResetVelocity('Y');
        }

        //play sfx
        m_playerAudioSource.clip = m_jumpSFX;
        m_playerAudioSource.Play();

        //add force
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
            m_currentState = new PlayerStateDashing(this);

            //play sfx
            m_playerAudioSource.clip = m_dashSFX;
            m_playerAudioSource.Play();

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

            //multiply by dash force
            dashDirection *= m_dashForce;

            //if player is on ground, add an upwards force
            if (m_isGrounded)
            {
                dashDirection += Vector3.up * m_dashGroundedUpForce;
            }

            //dot product tests, will need this later to affect velocities based on player direction. WIP
            if (Vector3.Dot(m_body.linearVelocity, dashDirection) < -0.8)
            {
                Debug.Log("opposite direction");
            }
            if (Vector3.Dot(m_body.linearVelocity, dashDirection) > 0.8)
            {
                Debug.Log("same direction");
            }

            //add the force and reset dash timer
            m_body.AddForce(dashDirection, ForceMode.Impulse);
            m_dashTimer = 0;
            m_dashReady = false;
        }
    }

    public void AcquireGrapple()
    {
        m_playerHasGrapple = true;
        m_gameHUD.ShowGrappleText();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_currentState = new PlayerStateGrounded(this);
            m_jumpCount = 0;
        }
    }
}
