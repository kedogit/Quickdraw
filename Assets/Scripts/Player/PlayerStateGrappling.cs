using UnityEngine;

public class PlayerStateGrappling : BasePlayerState
{
    private const float m_magnitudeSoftCap = 20f;
    private const float m_velocityTaper = 0.95f;

    public PlayerStateGrappling(Player script) : base(script)
    {
    }

    public override void Move()
    {
        //if player is going too fast, gradually decrease his velocity
        if (m_body.linearVelocity.magnitude >= m_magnitudeSoftCap)
        {
            m_body.linearVelocity *= m_velocityTaper;
        }

        m_body.AddForce(m_playerScript.MoveVector * m_playerScript.GrappleAcceleration, ForceMode.Acceleration);
    }
}
