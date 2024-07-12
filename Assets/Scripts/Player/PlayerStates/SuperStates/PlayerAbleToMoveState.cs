using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbleToMoveState : PlayerState
{
    protected Vector2 input;

    public PlayerAbleToMoveState(PlayerScriptSinglePlayer player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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

        //GIVES ACCESS TO MOVE AND IDLE STATES
        input.x = player.InputHandler.RawMovementInput.x;
        input.y = player.InputHandler.RawMovementInput.y;

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
