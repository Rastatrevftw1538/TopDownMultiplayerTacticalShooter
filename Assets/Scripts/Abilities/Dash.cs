using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/Movement/Dash")]
public class Dash : MovementAbility
{
    [Header("Ability Stats")]
    public float dashVelocity;

    public override void Activate(GameObject parent){
        startDash(parent);
    }

    public override void BeginCooldown(GameObject parent){
        endDash(parent);
    }

    #region PLAYER-ABILITIES
    public void startDash(GameObject parent)
    {

        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE
        player.runSpeed = dashVelocity;
        player.walkSpeed = dashVelocity;

        TrailRenderer tr = parent.GetComponent<TrailRenderer>();
        tr.emitting = true;
    }

    public void endDash(GameObject parent)
    {
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE
        player.runSpeed = player.runSpeedNormal;
        player.walkSpeed = player.walkSpeedNormal;

        TrailRenderer tr = parent.GetComponent<TrailRenderer>();
        tr.emitting = false;
    }
    #endregion
}
