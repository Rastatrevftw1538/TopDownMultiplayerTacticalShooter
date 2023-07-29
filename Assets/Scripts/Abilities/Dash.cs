using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "Player Abilities/Movement/Dash")]
public class Dash : MovementAbility
{
    public enum ApplicationType
    {
        SET_SPEED,
        SPEED_MULTIPLIER
    }

    [Header("Movement Application Type")]
    public ApplicationType applicationType;
    [Header("Ability Stats")]
    public float dashVelocity;
    public float speedMultiplier;

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

        //DETERMINE WHICH APPLICATION TYPE TO USE
        if(applicationType == ApplicationType.SET_SPEED) //SET SPEED
        {
            player.runSpeed  = dashVelocity;
            player.walkSpeed = dashVelocity;
        }
        else //SPEED MULITPLIER
        {
            player.runSpeed  *= speedMultiplier;
            player.walkSpeed *= speedMultiplier;
        }

        DoPretty(parent);
        
    }
    [ClientRpc]
    void DoPretty(GameObject player)
    {
        TrailRenderer tr = player.GetComponent<TrailRenderer>();
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

    #region HIT-SCAN ABILITY TYPE
    private void hitScanDash(GameObject parent, GameObject target)
    {
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE
        Weapon weaponScript = parent.GetComponent<Weapon>(); //REFERENCE TO THE PLAYER'S WEAPON SCRIPT ACTIVE IN THE SCENE

        Vector2 direction = weaponScript.firePoint.transform.up;
        weaponScript.CmdFireAbility(direction, this);
        
    }
    #endregion
}
