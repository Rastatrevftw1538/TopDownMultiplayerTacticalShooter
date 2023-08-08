using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Player Abilities/Movement/Blink")]
public class Blink : MovementAbility
{
    [Header("Ability Stats")]
    private Vector2 destination;
    //public int amtOfBlinks;
    public float blinkDistance;
    public float BlinkMultiplier;

    public override void Activate(GameObject parent)
    {
        PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE

        Vector2 movePlayer = MoveCharacter(player.transform.position, player.transform.GetChild(0).gameObject.transform.up);
        player.transform.Translate(movePlayer);
    }

    public override void BeginCooldown(GameObject parent)
    {
       //PlayerScript player = parent.GetComponent<PlayerScript>(); //REFERENCE TO THE PLAYER SCRIPT ACTIVE IN THE SCENE

    }

    Vector2 MoveCharacter(Vector2 playerposition, Vector2 playerforward)
    {
        //PlayerScript player = character.GetComponent<PlayerScript>();
        destination = CalculateDistance(playerposition, playerforward);
        var dist = Vector2.Distance(playerposition, destination);
        if (dist > 0.5f)
        {
            return destination;
        }
        else
        {
            return Vector2.zero;
        }
        
    }

    Vector2 CalculateDistance(Vector2 playerposition, Vector2 playerfoward)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerposition, playerfoward, out hit, blinkDistance))
        {
            Debug.DrawLine(playerposition, hit.point, Color.yellow, 2);
            return (hit.point * BlinkMultiplier);
        }
        else
        {
            Debug.DrawRay(playerposition, (playerfoward.normalized * blinkDistance) * BlinkMultiplier, Color.green, 2);
            return ((playerposition + playerfoward.normalized * blinkDistance) * BlinkMultiplier);
        }
    }
}
