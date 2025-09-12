using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter = 0;
    private float lastTimeAttacked;
    private float comboWindow = 0.5f;
    


    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 0.1f;

        #region Choose attack direction 
        float attackDir = player.facingDir;
        if (XInput != 0)
            attackDir = Input.GetAxisRaw("Horizontal");

        #endregion

        if (comboCounter > 2 || Time.time>= lastTimeAttacked + comboWindow) 
            comboCounter = 0;

        player.anim.SetInteger("ComboCounter",comboCounter);

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.2f);//บ๓าก

        comboCounter++;
        lastTimeAttacked = Time.time;

    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.ZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }


}
