using UnityEngine;

public class PlayerStateDashing : BasePlayerState
{
    private float m_elapsed = 0;
    private const float m_dashRotationSpeed = 2f;
    private bool m_velocityChangeRegistered;

    public PlayerStateDashing(Player script) : base(script)
    {
        Observer.GetInstance()?.TriggerEvent(EVENT.ON_PLAYER_DASH_START);
    }

    public override void Move()
    {
        m_elapsed += Time.deltaTime;

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

        if (m_elapsed < m_playerScript.DashDuration)
        {
            Vector3 currentVelocity = m_body.linearVelocity * m_playerScript.DashDecayRate;
            currentVelocity.y = m_body.linearVelocity.y;
            m_body.linearVelocity = currentVelocity;
        }
        else
        {
            ExitState();
        }
    }

    private void ExitState()
    {
        m_playerScript.ChangeState(new PlayerStateAirborne(m_playerScript));
    }
}
