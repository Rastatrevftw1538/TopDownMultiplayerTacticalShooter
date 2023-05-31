using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    private GameObject joystickMovementHandle;
    [SerializeField]
    private GameObject joystickRotateHandle;
    
    public float runSpeed = 5f;
    public float walkSpeed = 0.5f;

    [SerializeField]
    private Rigidbody2D rb;

    public Vector2 position;

    public Quaternion rotate;

    public bool isWalking;
    private bool isShooting;

private void Start() {
    if (isLocalPlayer)
    {
        Debug.Log("local Player" + netId);
        return;
    }
}
[ClientCallback]
private void Update()
{
    if (!isLocalPlayer)
    {
        return;
    }

    JoystickControllerMovement joystickController = joystickMovementHandle.GetComponent<JoystickControllerMovement>();

    float horizontal = joystickController.GetHorizontalInput();
    float vertical = joystickController.GetVerticalInput();

    JoystickControllerRotationAndShooting rotationJoystick = joystickRotateHandle.GetComponent<JoystickControllerRotationAndShooting>();
    Quaternion rotationInput = rotationJoystick.GetRotationInput();
    bool isShooting = rotationJoystick.isShooting;
    bool isWalking = joystickController.isWalking;
    Vector2 movement = new Vector2(horizontal, vertical).normalized;

    // Apply movement to the rigidbody
    if (isWalking)
    {
        this.transform.Translate( movement * walkSpeed);
        //Debug.Log("Walking");
    }
    else
    {
        this.transform.Translate(movement * runSpeed);
        //Debug.Log("Running");
    }

    this.transform.rotation = rotationInput;

    // Update the position and rotation variables
    position = this.transform.position;
    rotate = this.transform.rotation;

    // Update the position and rotation on all clients
    OnPositionUpdated(position, rotate);

    // Send movement command to the server
    CmdSendMovement(movement, rotate, isWalking);
}

[Command]
private void CmdSendMovement(Vector2 movement, Quaternion rotate, bool isWalking)
{
    rb.velocity = movement * (isWalking ? walkSpeed : runSpeed);

    // Update the position and rotation variables
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

    public override void OnStartLocalPlayer()
    {

        // Set the player's outline to Yellow
        this.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1,1,0,1);

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
