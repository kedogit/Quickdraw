using UnityEngine;

public class PlayerStateGrappling : BasePlayerState
{
    public PlayerStateGrappling(Player script) : base(script)
    {
    }

    public override void Move()
    {
        Debug.Log("grappling");
        m_playerScript.RigidBody.AddForce(m_playerScript.MoveVector * m_playerScript.GrappleAcceleration, ForceMode.Force);
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
