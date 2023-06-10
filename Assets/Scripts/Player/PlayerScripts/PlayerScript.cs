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
    [SerializeField]
    private GameObject joystickMovementUI;
    [SerializeField]
    private GameObject joystickRotateUI;

    [HideInInspector] private float runSpeed = 5f;
    [HideInInspector] private float walkSpeed = 0.5f;

    [HideInInspector]
    private Vector2 playerPosition;
    [HideInInspector]
    private Quaternion playerRotation;

    [HideInInspector] public bool isRunning;
    private bool isShooting;
    private bool canMove = true;

    private void Start()
    {
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

    public void setCanMove(bool newCanMove)
    {
        this.canMove = newCanMove;
    }

    [ClientCallback]
private void Update()
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

    Vector2 movement = new Vector2(horizontal, vertical).normalized;

    // Apply movement to the character's position
    if (canMove)
    {
        if (!isRunning)
        {
            // Walk speed
            this.transform.Translate(movement * walkSpeed);
        }
        else
        {
            // Run speed
            this.transform.Translate(movement * runSpeed);
        }

        // Update the position and rotation variables
        playerPosition = this.transform.position;
        playerRotation = this.transform.rotation;

        // Update the position and rotation on all clients
        OnPositionUpdated(playerPosition, playerRotation);

        // Send movement command to the server
        CmdSendMovement(playerPosition, playerRotation, isRunning);
    }
}
[Command]
private void CmdSendMovement(Vector2 position, Quaternion rotate, bool isWalking)
{
    
    playerPosition = position;
    playerRotation = rotate;

    // Update the position and rotation on all clients
    RpcUpdateMovement(playerPosition, playerRotation);
}

[ClientRpc]
private void RpcUpdateMovement(Vector2 position, Quaternion rotation)
{
    // Update the player's position and rotation on all clients except the owner
    if (!isLocalPlayer)
    {
        // Update the position
        transform.position = position;

        // Update the rotation
        transform.rotation = rotation;
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
        float distance = Vector2.Distance(transform.position, newPosition);

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

    public override void OnStartLocalPlayer()
    {
        // Set the player's outline to Yellow
        this.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
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
            Debug.Log(networkIdentity.assetId + " taking Authority for " + this.gameObject.name);
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
