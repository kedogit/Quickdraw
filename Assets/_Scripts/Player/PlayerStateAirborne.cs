using UnityEngine;

public class PlayerStateAirborne : BasePlayerState
{
    private float m_airRotationSpeed;
    private const float m_stationaryKickstartForce = 1.5f;
    private const float m_rayDistance = 2f;

    public PlayerStateAirborne(Player script) : base(script)
    {
        m_airRotationSpeed = m_playerScript.AirRotationSpeed;
    }

    public override void Move()
    {
        if (Physics.Raycast(m_playerScript.transform.position, Vector3.down, m_rayDistance, m_playerScript.GroundLayer))
        {
            m_playerScript.ChangeState(new PlayerStateGrounded(m_playerScript));
        }

        //fetches the planar movement vector of the player
        Vector3 currentVectorPlanar = m_body.linearVelocity;
        currentVectorPlanar.y = 0;

        //fetches the planar input vector of the player
        Vector3 moveVectorPlanar = m_playerScript.MoveVector;
        moveVectorPlanar.y = 0;

        //if player is not moving, add a little bit of force in the input direction
        if (currentVectorPlanar.magnitude <= 0.1f)
        {
            m_body.AddForce(moveVectorPlanar * m_stationaryKickstartForce, ForceMode.Impulse);
        }

        //rotates the player's currnet movement towards the input without changing the magnitude
        currentVectorPlanar = Vector3.RotateTowards(currentVectorPlanar, moveVectorPlanar, m_airRotationSpeed * Time.deltaTime, 0f);

        //applies the rotation to the rigidbody
        currentVectorPlanar.y = m_body.linearVelocity.y;
        m_body.linearVelocity = currentVectorPlanar;
    }
}
