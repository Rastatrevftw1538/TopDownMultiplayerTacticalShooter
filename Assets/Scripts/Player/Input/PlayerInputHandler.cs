using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// READS INPUT FROM PLAYERSTATE
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }
    public int NormInputLeft { get; private set; }
    public int NormInputRight { get; private set; }
    public int NormInputUp { get; private set; }
    public int NormInputDown { get; private set; }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();

        //MAKES IT SO THAT THE MOVEMENT INPUT EITHER RETURNS -1,0,1. ONLY REALLY NEEDED IF YOU DON'T WANT CONTROLLER PLAYERS MOVING AT DIFFERENT SPEEDS THAN KB-M.
        NormInputLeft = (int)(RawMovementInput * Vector2.left).x;
        NormInputRight = (int)(RawMovementInput * Vector2.right).x;
        NormInputUp = (int)(RawMovementInput * Vector2.up).y;
        NormInputDown = (int)(RawMovementInput * Vector2.down).y;
    }

    public void OnAbilityInput(InputAction.CallbackContext context)
    {
        
        //code

    }
}
