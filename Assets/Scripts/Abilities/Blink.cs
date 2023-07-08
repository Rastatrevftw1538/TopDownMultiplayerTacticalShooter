using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/Movement/Pseudo Blink")]
public class Blink : MovementAbility
{
    [Header("Ability Stats")]
    private Vector2 destination;
    public float blinkDistance;

    public override void Activate(GameObject parent)
    {
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE

        //scuffed blink ability
        destination = new Vector2(parent.transform.position.x + blinkDistance, parent.transform.position.y);

        player.transform.position = destination;
    }

    public override void BeginCooldown(GameObject parent)
    {
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE
        
    }
}
