using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    // Start is called before the first frame update
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
         base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        if (XInput != 0 && player.facingDir != XInput)
            stateMachine.ChangeState(player.idleState);

        if(YInput < 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y * 0.9f);

        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);

        if (!player.IsWallDetected())
            stateMachine.ChangeState(player.airState);


    }
}
