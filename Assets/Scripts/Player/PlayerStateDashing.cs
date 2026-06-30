using UnityEngine;

public class PlayerStateDashing : BasePlayerState
{
    public PlayerStateDashing(Player script) : base(script)
    {
    }

    public override void Move()
    {
        Debug.Log("dashing");
    }
}
