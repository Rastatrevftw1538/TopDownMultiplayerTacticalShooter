using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    public enum Team
    {
        None,
        Red,
        Blue
    }
    [SyncVar]
    private Team playerTeam;

    [Header("Controller")]
    [SerializeField]
    private GameObject joystickMovementUI;
    [SerializeField]
    private GameObject joystickRotateUI;
    
    [Header("Player Stats")]
    public float runSpeed = 5f;
    public float walkSpeed = 0.5f;
    [HideInInspector]
    public float runSpeedNormal = 5f;
    [HideInInspector]
    public float walkSpeedNormal = 0.5f;

    [Header("Player Components")]
    [SerializeField]
    private Rigidbody2D rb;
    public Vector2 movement;
    public Vector2 position;
    public Quaternion rotate;

    [Header("Player Status")]
    public bool isRunning;
    private bool isShooting;
    private bool canMove = true;

    private void Start() {
        runSpeedNormal = 5f;
        walkSpeedNormal = 0.5f;

        if (isLocalPlayer)
        {
            Debug.Log("local Player" + netId);
            return;
        }

    }

    public Team PlayerTeam
    {
        get { return playerTeam; }
        set { playerTeam = value; }
    }
    public void setCanMove(bool newCanMove){
    this.canMove = newCanMove;
    }

[ClientCallback]
public void FixedUpdate()
{
    if (!isLocalPlayer)
    {
        return;
    }

    JoystickControllerMovement joystickController = joystickMovementUI.GetComponent<JoystickControllerMovement>();

    float horizontal = joystickController.GetHorizontalInput();
    float vertical = joystickController.GetVerticalInput();

    JoystickControllerRotationAndShooting rotationJoystick = joystickRotateUI.GetComponent<JoystickControllerRotationAndShooting>();
    Quaternion rotationInput = rotationJoystick.GetRotationInput();
    isShooting = rotationJoystick.isShooting;
    isRunning = joystickController.isRunning;
        movement = new Vector2(horizontal, vertical).normalized * (isRunning ? runSpeed : walkSpeed);
        //Debug.Log(canMove);
        //Debug.Log("horizontal" + horizontal);
        //Debug.Log("vertical" + vertical);
        //Debug.Log("running: " + runSpeed);
        //Debug.Log("walking: " + walkSpeed);
        // Apply movement to the rigidbody
        if (canMove){
        CmdMove(movement);
        //this.transform.rotation = rotationInput;
        // Update the position and rotation variables
        this.transform.GetChild(0).transform.SetPositionAndRotation(this.transform.GetChild(0).transform.position,rotationInput); //POSSIBLY MAKE THIS FIND THE 'PlayerBody' RATHER THAN JUST FINDING THE FIRST CHILD OF THE GAMEOBJECT
        //position = this.transform.position;
        //rotate = this.transform.rotation;
        
        /*
        // Update the position and rotation on all clients
        OnPositionUpdated(new Vector2(horizontal, vertical).normalized, rotationInput);

        // Send movement command to the server
        CmdSendMovement(movement, rotationInput, isRunning);
        */
    }
}
    #region ryan messing stuff up
    #endregion

    [Command]
    private void CmdMove(Vector2 movement)
    {
        RpcMove(movement);
        transform.Translate(movement * Time.deltaTime);    
    }
    // FIGURE OUT HOW THE SERVER RECIEVES THIS INFO

[ClientRpc]
    private void RpcMove(Vector2 movement)
    {
        transform.Translate(movement * Time.deltaTime);
    }

//COMMENTED OUT CODE WAS FOR SERVER SIDE IMPLEMENTATION


/*
[Command]
private void CmdSendMovement(Vector2 movement, Quaternion rotate, bool isWalking)
{
    //rb.velocity = movement * (isWalking ? walkSpeed : runSpeed);

    // Update the position and rotation variables
    this.transform.Translate(movement * (isWalking ? walkSpeed : runSpeed));
    position = rb.position;
    this.transform.rotation = rotate;

    // Update the position and rotation on all clients
    RpcUpdateMovement(position, rotate);
}

    [ClientRpc]
    private void RpcUpdateMovement(Vector2 position,Quaternion rotation)
    {
        // Update the player's position on all clients except the owner
        if (!isLocalPlayer)
        {
            rb.position = position;
            this.transform.rotation = rotation;
        }
    }
    
private void OnPositionUpdated(Vector2 newPosition, Quaternion newRotation)
{
    if (!isLocalPlayer)
    {
        // The player's position should only be updated for the local player
        return;
    }

    // Calculate the distance between the old position and the new position
    float distance = Vector2.Distance(rb.position, newPosition);

    // If the distance is too great, teleport the player to the new position
    if (distance > 2f)
    {
        transform.position = newPosition;
    }
    // Otherwise, move the player smoothly towards the new position
    else
    {
        transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime * 10f);
    }

    // Update the player's rotation
    transform.rotation = newRotation;
}
    private void OnRotationUpdated(Quaternion oldRotation,Quaternion newRotation)
    {
        if (isLocalPlayer)
        {
            return;
        }

        // Update the player's rotation on all clients except the owner
        this.transform.rotation = newRotation;
    }
        */
    public override void OnStartLocalPlayer()
    {

        // Set the player's outline to Yellow
        this.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1,1,0,1);
        this.GetComponent<Weapon>().spreadCone.enabled = true;

        // Request authority from the server
        CmdRequestAuthority();
    }
[Command]
private void CmdRequestAuthority()
{
    if (!isLocalPlayer)
        {
            // Assign authority to the player object
            NetworkIdentity networkIdentity = this.gameObject.GetComponent<NetworkIdentity>();
            networkIdentity.AssignClientAuthority(connectionToClient);
            Debug.Log(networkIdentity.assetId);
            /*
            foreach (var conn in NetworkServer.connections.Values)
            {
                foreach (var id in conn.owned)
                {
                    if(networkIdentity.assetId == id.assetId){
                        NetworkServer.UnSpawn(this.)
                    }
                }
            }
            */
            Debug.Log(networkIdentity.assetId+" taking Authority for "+ this.gameObject.name);
        }
}
public override void OnStopClient()
    {
        // Clear authority when the client stops
        if (isLocalPlayer)
        {
            CmdClearAuthority();
        }
    }

    [Command]
    private void CmdClearAuthority()
    {
        // Clear authority if this client had it
        if (isLocalPlayer)
        {
            NetworkIdentity networkIdentity = this.gameObject.GetComponent<NetworkIdentity>();
            networkIdentity.RemoveClientAuthority();
        }
    }
}
