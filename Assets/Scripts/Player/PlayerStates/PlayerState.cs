using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// IS THE SCRIPT FOR DETERMINING WHETHER THE PLAYER IS MOVING, USING AN ABILITY, ETC.
/// </summary>
public class PlayerState
{
    //DECLARING VARIABLES
    protected PlayerScriptSinglePlayer player;
    //protected PlayerScript player;
    protected PlayerStateMachine stateMachine;
    protected PlayerAbleToMoveState playerAbleToMoveState;

    protected float startTime;

    private string animBoolName;

    public PlayerState(PlayerScriptSinglePlayer player, PlayerStateMachine stateMachine, string animBoolName)
    {
        //SPECIFYING 'THIS' BECAUSE THEY HAVE THE SAME VARIABLE NAMES AS DECLARED.
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    //CALLED WHEN WE ENTER A STATE
    public virtual void Enter()
    {
        DoChecks();

        //grabbing the player animator, setting the float of the current animation to 1.0f so it starts.
        //player.Anim.SetFloat("MoveHorizontal", playerData.movementVelocity * player.InputHandler.RawMovementInput.x);
        //player.Anim.SetFloat("MoveVertical", playerData.movementVelocity * player.InputHandler.RawMovementInput.y);
        //player.Anim.SetBool(animBoolName, true);

        player.Anim.SetFloat(animBoolName, 1f);

        startTime = Time.time;

    }

    //CALLED WHEN WE EXIT A STATE
    public virtual void Exit()
    {
        //grabbing the player animator, setting the float of the movement input to 0f so it stops. "Idle"
        //player.Anim.SetBool(animBoolName, false);
        player.Anim.SetFloat(animBoolName, 0f);

    }

    //REGULAR UPDATE FUNCTION
    public virtual void LogicUpdate()
    {
        //CONSTANTLY SETTING THE FLOAT VALUE TO THE VELOCITY SO WE RETURN THE CORRECT FLOAT VALUES FOR THE ANIMATION TO WORK AT ALL TIMES.
        player.Anim.SetFloat("MoveHorizontal", player.walkSpeed * player.InputHandler.RawMovementInput.x);
        player.Anim.SetFloat("MoveVertical", player.walkSpeed * player.InputHandler.RawMovementInput.y);
    }

    //FIXED UPDATE FUNCTION
    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    //WILL CALL FROM ENTER AND PHYSICS UPDATE. CHECK FOR WALLS, THINGS LIKE THAT
    public virtual void DoChecks()
    {

    }

}
