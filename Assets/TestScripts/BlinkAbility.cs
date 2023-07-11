using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Apple;

public class BlinkAbility : MonoBehaviour
{
    [SerializeField]
    public GameObject character;
    public GameObject characterChild;

    public int amtOfBlinks;
    public float distance;
    public float BlinkMultiplier;
    public float speed;
    public float coolDown;

    Vector2 destination;
    float coolDownTimer = 0.0f;

   


    // Start is called before the first frame update
    void Start()
    {
        character = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (coolDownTimer > 0.0f)
        {
            coolDownTimer -= Time.deltaTime;
        }
        if (amtOfBlinks < 1)
        {
            coolDownTimer = coolDown;
        }*/
        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveCharacter(character.transform.position, character.transform.GetChild(0).gameObject.transform.up);
        }
        
    }

    void MoveCharacter(Vector2 playerposition, Vector2 playerforward)
    {
        //PlayerScript player = character.GetComponent<PlayerScript>();
        destination = CalculateDistance(playerposition, playerforward);
        var dist = Vector2.Distance(playerposition, destination);
        if (dist > 0.5f)
        {
            Vector2 newposition = Vector2.MoveTowards(playerposition, destination, Time.deltaTime * speed);
            character.transform.position = newposition;
        }
        //return Vector2.zero;

        /*if (amtOfBlinks > 0)
        {
            amtOfBlinks -= 1;
        }*/
    }

    Vector2 CalculateDistance(Vector2 playerposition, Vector2 playerfoward)
    {
        RaycastHit hit;
        if(Physics.Raycast(playerposition, playerfoward, out hit, distance))
        {
            Debug.DrawLine(playerposition, hit.point, Color.yellow, 2);
            return(hit.point * BlinkMultiplier);
        }
        else
        {
            Debug.DrawRay(playerposition, (playerfoward.normalized * distance) * BlinkMultiplier, Color.green, 2);
            return ((playerposition + playerfoward.normalized * distance) * BlinkMultiplier);
        }
    }

}
