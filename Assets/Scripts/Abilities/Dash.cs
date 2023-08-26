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

    private CircleCollider2D aura;

    public override void Activate(GameObject parent){
        PlayerScript player = parent.GetComponent<PlayerScript>();
        StartDash(parent);
        //SpawnAura(player, true);
    }

    public override void BeginCooldown(GameObject parent){
        PlayerScript player = parent.GetComponent<PlayerScript>();
        //SpawnAura(player, false);
        EndDash(parent);
    }

    #region PLAYER-ABILITIES
    public void StartDash(GameObject parent)
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
    //[ClientRpc]
    void DoPretty(GameObject player)
    {
        TrailRenderer tr = player.GetComponent<TrailRenderer>();
        tr.emitting = true;
    }

    public void EndDash(GameObject parent)
    {
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE
        player.runSpeed = player.runSpeedNormal;
        player.walkSpeed = player.walkSpeedNormal;

        TrailRenderer tr = parent.GetComponent<TrailRenderer>();
        tr.emitting = false;
    }
    #endregion

    #region PLAYER EFFECTS
    private void EnemyDash(GameObject parent)
    {
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE
    }
    #endregion

    #region ACTIVATION METHODS
    private void SpawnAura(PlayerScript player, bool toSpawn)
    {
        if (toSpawn)
        {
            Debug.LogError("Calls Aura");
            aura = player.gameObject.AddComponent<CircleCollider2D>();
            aura.radius = 5f;
            aura.isTrigger = true;
        }
        else
        {
            Debug.LogError("Destroy Aura");
            GameObject.Destroy(aura);
        }
        //IF YOU WANT THE CIRCLE TO GET BIGGER OVER TIME, USE INVOKE REPEATING
    }

    /*private bool GetCorrectTag(PlayerEffects effect)
    {
        bool hasCorrectTag;
        switch (effect)
        {
            case PlayerEffects.ENEMY: //IF ENEMY IS THE TARGET
                hasCorrectTag = player.playerTeam != currentCollider.playerTeam;
                if (player.playerTeam == PlayerScript.Team.Red)
                {
                    correctTag = "Red";
                }
                break;
            case PlayerEffects.TEAM: //IF TEAM IS THE TARGET
                hasCorrectTag = player.playerTeam == currentCollider.playerTeam;
                break;
            default:
                hasCorrectTag = player == currentCollider;
                break;
        }
    }*/
    #endregion
}
