using UnityEngine;

public abstract class BasePlayerState
{
    protected Player m_playerScript;
    protected Rigidbody m_body;

    public BasePlayerState(Player script)
    {
        m_playerScript = script;
        m_body = script.GetComponent<Rigidbody>();
    }

    public abstract void Move();
}
