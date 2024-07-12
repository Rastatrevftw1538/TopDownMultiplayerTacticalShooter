using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HOLDS A VARIABLE TO PLAYER'S CURRENT STATE,
/// FUNCTION TO INITIALIZE PLAYER'S CURRENT STATE,
/// FUNCTION TO CHANGE PLAYER'S CURRENT STATE.
/// </summary>
public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
