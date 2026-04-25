using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        player.anim.SetTrigger("Dead");
        rb = player.rb;
        triggerCalled = false;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, player.rb.velocity.y);
    }
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }
}
