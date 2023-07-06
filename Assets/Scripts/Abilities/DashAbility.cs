using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/GAME DESIGN/Dash")]
public class DashAbility : Ability
{
    public float dashVelocity;

    public override void Activate(GameObject parent){
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER GAMEOBJECT

        Rigidbody2D rb = parent.GetComponent<Rigidbody2D>();

        rb.velocity = player.movement.normalized * dashVelocity;
    }
}
