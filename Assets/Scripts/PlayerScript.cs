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

    [SyncVar(hook = "OnPositionUpdated")]
    private Vector2 position;

    [SyncVar(hook = "OnRotationUpdated")]
    private Quaternion rotate;

    [SyncVar(hook = "OnWalkingChanged")]
    private bool isWalking;
    [SyncVar(hook = "OnShootingChanged")]
    private bool isShooting;

        private void OnWalkingChanged(bool oldValue, bool newValue)
{
    isWalking = newValue;
}
private void OnShootingChanged(bool oldValue, bool newValue)
{
    isShooting = newValue;
}
private void Start() {
}

    [ClientCallback]
    private void Update()
{
    if (!isOwned)
    {
        Debug.Log("Not local Player");
        return;
    }
    JoystickControllerMovement joystickController = joystickMovementHandle.GetComponent<JoystickControllerMovement>();
    float horizontal = joystickController.GetHorizontalInput();
    float vertical = joystickController.GetVerticalInput();

    JoystickControllerRotationAndShooting rotationJoystick = joystickRotateHandle.GetComponent<JoystickControllerRotationAndShooting>();
    Quaternion rotationInput = rotationJoystick.GetRotationInput();
    isShooting = rotationJoystick.isShooting;
    isWalking = joystickController.isWalking;
    Vector2 movement = new Vector2(horizontal, vertical).normalized;

    // Apply movement to the rigidbody
    if(isWalking){
        rb.velocity = movement * walkSpeed;
        //Debug.Log("Walking");
    }
    else{
        rb.velocity = movement * runSpeed;
        //Debug.Log("Running");
    }
    


        this.transform.rotation = rotationInput;

    // Send movement command to the server
    CmdSendMovement(movement,this.transform.rotation,isWalking);
}

    [Command]
void CmdSendMovement(Vector2 movement, Quaternion rotate, bool isWalking)
{
    // Apply movement to the rigidbody on the server
    if (isWalking)
    {
        rb.velocity = movement * walkSpeed;
    }
    else
    {
        rb.velocity = movement * runSpeed;
    }

    // Set the player's position and rotation
    position = rb.position;
    this.transform.rotation = rotate;

    // Broadcast movement to all clients except the sender
    RpcUpdateMovement(position, this.transform.rotation);

    // Wait for a short time before updating the player's position and rotation
    StartCoroutine(DelayUpdate(position, this.transform.rotation));
}

    private IEnumerator DelayUpdate(Vector2 position, Quaternion rotation)
{
    // Wait for 0.05 seconds before updating the player's position and rotation
    yield return new WaitForSeconds(0.05f);

    // Update the player's position and rotation on the clients
    RpcUpdateMovement(position, rotation);
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
    private void OnPositionUpdated(Vector2 oldValue, Vector2 newPosition)
{
    if (!isOwned)
    {
        // The player's position should only be updated for the local player
        return;
    }

    // Calculate the distance between the old position and the new position
    float distance = Vector2.Distance(oldValue, newPosition);

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
        base.OnStartLocalPlayer();

        // Set the player's color to blue
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
    }
}
