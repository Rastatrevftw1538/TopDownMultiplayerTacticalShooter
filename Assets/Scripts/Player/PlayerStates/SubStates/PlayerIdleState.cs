using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerAbleToMoveState
{
    public PlayerIdleState(PlayerScriptSinglePlayer player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0f, 0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        
        base.LogicUpdate();

        if (input.x != 0)
        {
            stateMachine.ChangeState(player.MoveStateX);
        }
        if (input.y != 0)
        {
            stateMachine.ChangeState(player.MoveStateY);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
