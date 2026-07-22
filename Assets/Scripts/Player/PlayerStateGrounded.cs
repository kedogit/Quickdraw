using UnityEngine;

public class PlayerStateGrounded : BasePlayerState
{
    private float m_moveSpeed;
    private const float m_rayDistance = 2f;
    private float m_elapsed;
    private const float m_coyoteTime = 0.3f;
    private bool m_isTransitioning;

    public PlayerStateGrounded(Player script) : base(script)
    {
        m_moveSpeed = m_playerScript.MoveSpeed;
        m_playerScript.ResetJumpCount();
    }

    public override void Move()
    {
        if (!Physics.Raycast(m_playerScript.transform.position, Vector3.down, m_rayDistance, m_playerScript.GroundLayer))
        {
            if (m_isTransitioning == false)
            {
                m_elapsed = 0;
                m_isTransitioning = true;
            }
        }
        else
        {
            m_isTransitioning = false;
        }

        if (m_isTransitioning)
        {
            m_elapsed += Time.deltaTime;

            if (m_elapsed >= m_coyoteTime)
            {
                m_playerScript.ChangeState(new PlayerStateAirborne(m_playerScript));
            }
        }
        else
        {
            //m_playerScript.ResetJumpCount();
        }

        Vector3 planarMovement = m_playerScript.MoveVector * m_moveSpeed;
        planarMovement.y = m_body.linearVelocity.y;
        m_body.linearVelocity = planarMovement;

        Debug.Log("grounded");
    }
}
