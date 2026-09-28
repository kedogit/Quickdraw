using UnityEngine;

public class PlayerStateDashing : BasePlayerState
{
    private const float m_dashRotationSpeed = 2f;
    private bool m_velocityChangeRegistered;

    public PlayerStateDashing(Player script) : base(script)
    {
        Observer.GetInstance()?.TriggerEvent(EVENT.ON_PLAYER_DASH_START);
    }

    public override void Move()
    {
        //fetches the planar movement vector of the player
        Vector3 currentVectorPlanar = m_body.linearVelocity;
        currentVectorPlanar.y = 0;

        //fetches the planar input vector of the player
        Vector3 moveVectorPlanar = m_playerScript.MoveVector;
        moveVectorPlanar.y = 0;

        //rotates the player's current movement towards the input without changing the magnitude
        currentVectorPlanar = Vector3.RotateTowards(currentVectorPlanar, moveVectorPlanar, m_dashRotationSpeed * Time.deltaTime, 0f);

        //applies the rotation to the rigidbody
        currentVectorPlanar.y = m_body.linearVelocity.y;
        m_body.linearVelocity = currentVectorPlanar;

        if (currentVectorPlanar.magnitude > m_playerScript.MoveSpeed)
        {
            Vector3 currentVelocity = m_body.linearVelocity * m_playerScript.DashDecayRate;
            currentVelocity.y = m_body.linearVelocity.y;
            m_body.linearVelocity = currentVelocity;

            //this bool exists because this state might accidentally detect the player's walking speed on its first frame and immediately end itself otherwise.
            if (!m_velocityChangeRegistered)
            {
                m_velocityChangeRegistered = true;
            }
        }
        else if (m_velocityChangeRegistered)
        {
            m_playerScript.ChangeState(new PlayerStateAirborne(m_playerScript));
        }
    }
}
