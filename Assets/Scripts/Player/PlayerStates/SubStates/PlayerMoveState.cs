using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerAbleToMoveState
{
    public PlayerMoveState(PlayerScriptSinglePlayer player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //CHECKING IF THE PLAYER CAN FLIP DIRECTIONS. BASED ON PLAYER SCRIPTS FACING DIRECTIONS
        //player.CheckIfShouldFlip(input.x);

        player.SetVelocity(player.walkSpeed * input.x, player.walkSpeed * input.y);
        //Debug.Log(player.CurrentVelocity);

        if (input.x == 0 && input.y == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
