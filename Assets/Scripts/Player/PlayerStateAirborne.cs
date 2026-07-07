using UnityEngine;

public class PlayerStateAirborne : BasePlayerState
{
    public PlayerStateAirborne(Player script) : base(script)
    {

    }

    public override void Move()
    {
        Debug.Log("airborne");
        m_playerScript.RigidBody.AddForce(m_playerScript.MoveVector * m_playerScript.AirAcceleration, ForceMode.Acceleration);

        if (m_playerScript.IsGrounded)
        {
           m_playerScript.ChangeState(new PlayerStateGrounded(m_playerScript));
        }
    }
}
