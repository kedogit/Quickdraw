using UnityEngine;

public class PlayerStateJumping : BasePlayerState
{
    private float m_airRotationSpeed;
    private float m_elapsed;
    private const float m_jumpDuration = 0.2f;

    public PlayerStateJumping(Player script) : base(script)
    {
        m_airRotationSpeed = m_playerScript.AirRotationSpeed;

        //if planar speed faster than base move speed, apply a dampening effect
        Vector3 currentVectorPlanar = m_body.linearVelocity;
        currentVectorPlanar.y = 0;

        //fetches the planar input vector of the player
        Vector3 moveVectorPlanar = m_playerScript.MoveVector;
        moveVectorPlanar.y = 0;

        //dampen the planar magnitude if it exceeds the base movespeed, higher dampening the higher the difference
        if (currentVectorPlanar.magnitude > m_playerScript.MoveSpeed)
        {
            currentVectorPlanar *= 1 - ((currentVectorPlanar.magnitude - m_playerScript.MoveSpeed) / 100);
            currentVectorPlanar.y = m_body.linearVelocity.y;
            m_body.linearVelocity = currentVectorPlanar;
        }
    }

    public override void Move()
    {
        //fetches the planar movement vector of the player
        Vector3 currentVectorPlanar = m_body.linearVelocity;
        currentVectorPlanar.y = 0;

        //fetches the planar input vector of the player
        Vector3 moveVectorPlanar = m_playerScript.MoveVector;
        moveVectorPlanar.y = 0;

        //if player is not moving, add a little bit of force in the input direction
        if (currentVectorPlanar.magnitude <= 0.1f)
        {
            m_body.AddForce(moveVectorPlanar * m_playerScript.JumpKickstart, ForceMode.Impulse);
        }

        //rotates the player's currnet movement towards the input without changing the magnitude
        currentVectorPlanar = Vector3.RotateTowards(currentVectorPlanar, moveVectorPlanar, m_airRotationSpeed * Time.deltaTime, 0f);

        //applies the rotation to the rigidbody
        currentVectorPlanar.y = m_body.linearVelocity.y;
        m_body.linearVelocity = currentVectorPlanar;

        m_elapsed += Time.deltaTime;
        if (m_elapsed >= m_jumpDuration)
        {
            //TODO: check if player is grounded (hitting ceilings) and handle
            m_playerScript.ChangeState(new PlayerStateAirborne(m_playerScript));
        }
    }
}
