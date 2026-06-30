using UnityEngine;

public abstract class BasePlayerState
{
    protected Player m_playerScript;

    public BasePlayerState(Player script)
    {
        m_playerScript = script;
    }

    public abstract void Move();
}
