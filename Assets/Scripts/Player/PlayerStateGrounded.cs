using UnityEngine;

public class PlayerStateGrounded : BasePlayerState
{
    public PlayerStateGrounded(Player script) : base(script)
    {
    }

    public override void Move()
    {
        Debug.Log("grounded movement");
        Vector3 xzMovement = m_playerScript.MoveVector * m_playerScript.MoveSpeed;
        xzMovement.y = m_playerScript.RigidBody.linearVelocity.y;
        m_playerScript.RigidBody.linearVelocity = xzMovement;

        if (!m_playerScript.IsGrounded)
        {
            m_playerScript.ChangeState(new PlayerStateAirborne(m_playerScript));
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
