using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/Movement/Blink")]
public class Blink_test : MovementAbility
{
    public float distance = 5.0f;

    public override void Activate(GameObject parent){ 
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE
        Transform playerTransform = player.transform;

        RaycastHit hit;
        Vector2 destination = playerTransform.position + playerTransform.up * distance;

        //OBSTACLE FOUND IN THE WAY
        if (Physics.Linecast(playerTransform.position, destination, out hit))
        {
            destination = playerTransform.position + playerTransform.up * (hit.distance - 1f); //DESTINATION OF THE BLINK IS RIGHT BEFORE THE OBSTACLE
        }

        //NO OBSTACLE FOUND IN THE WAY
        if (Physics.Raycast(destination, -Vector2.up, out hit))
        {
            destination = hit.point; //DESTINATION OF THE BLINK EQUALS THE PLAYER'S CURRENT POSITION PLUS 'distance'
            playerTransform.position = destination; //SET THE PLAYER'S POSITION TO THE BLINK DESTINATION
        }
    }

    public override void BeginCooldown(GameObject parent){
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE


    }
}
