using UnityEngine;

public class PlayerStateDashing : BasePlayerState
{
    private float m_elapsed;
    private const float m_dashDuration = 0.5f;
    private const float m_dashRotationSpeed = 2f;

    public PlayerStateDashing(Player script) : base(script)
    {
    }

    public override void Move()
    {
        //fetches the planar movement vector of the player
        Vector3 currentVectorPlanar = m_body.linearVelocity;
        currentVectorPlanar.y = 0;

        //fetches the planar input vector of the player
        Vector3 moveVectorPlanar = m_playerScript.MoveVector;
        moveVectorPlanar.y = 0;

        //rotates the player's currnet movement towards the input without changing the magnitude
        currentVectorPlanar = Vector3.RotateTowards(currentVectorPlanar, moveVectorPlanar, m_dashRotationSpeed * Time.deltaTime, 0f);

        //applies the rotation to the rigidbody
        currentVectorPlanar.y = m_body.linearVelocity.y;
        m_body.linearVelocity = currentVectorPlanar;

        m_elapsed += Time.deltaTime;
        if (m_elapsed >= m_dashDuration)
        {
            m_playerScript.ChangeState(new PlayerStateAirborne(m_playerScript));
        }
    }
}
