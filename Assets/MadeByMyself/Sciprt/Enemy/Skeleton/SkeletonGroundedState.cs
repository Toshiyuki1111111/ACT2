using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundedState : EnemyState
{
    protected Enemy_Skeleton enemy;

    protected Transform player;
    public SkeletonGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        // 确保 PlayerManager 和 player 都存在
        if (PlayerManager.instance == null || PlayerManager.instance.player == null)
        {
            Debug.LogWarning("PlayerManager or Player is not ready! Retrying...");
            // 可以延迟再试一次（例如用协程）
            enemy.StartCoroutine(WaitForPlayer());
            return;
        }

        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDelected() || Vector2.Distance(enemy.transform.position, player.position) < 2 * enemy.attackDistance)
            stateMachine.ChangeState(enemy.battleState);
    }

    private IEnumerator WaitForPlayer()
    {
        while (PlayerManager.instance == null || PlayerManager.instance.player == null)
        {
            yield return null; // 等待一帧
        }
        player = PlayerManager.instance.player.transform;
    }
}
