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
            //if left the ground and currently not transitioning, start transition
            if (m_isTransitioning == false)
            {
                m_elapsed = 0;
                m_isTransitioning = true;
            }
        }
        else
        {
            //if on the ground, set transition to false
            m_isTransitioning = false;
        }

        //if transitioning, elapse timer and change to airborne after fully elapsed
        if (m_isTransitioning)
        {
            m_elapsed += Time.deltaTime;

            if (m_elapsed >= m_coyoteTime)
            {
                m_playerScript.ChangeState(new PlayerStateAirborne(m_playerScript));
            }
        }

        //apply planar movement by modifying linear velocity
        Vector3 planarMovement = m_playerScript.MoveVector * m_moveSpeed;
        planarMovement.y = m_body.linearVelocity.y;
        m_body.linearVelocity = planarMovement;
    }
}
