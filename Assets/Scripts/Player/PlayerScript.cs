using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    private Camera playerCamera;
    public enum Team
    {
        None,
        Red,
        Blue
    }
    [SyncVar]
    public Team playerTeam;

    public enum SetDeviceType
    {
        Auto,
        PC,
        Mobile
    }
    [SerializeField]
    private SetDeviceType deviceType;

    [Header("Controller")]
    [SerializeField]
    private GameObject joystickMovementUI;
    [SerializeField]
    private GameObject joystickRotateUI;
    
    [Header("Player Stats")]
    public float runSpeed = 5f;
    public float walkSpeed = 0.5f;
    [HideInInspector]
    public float runSpeedNormal;
    [HideInInspector]
    public float walkSpeedNormal;

    [Header("Player Components")]
    public Vector2 movement;
    public Quaternion rotation;
    public Rigidbody2D rb;
    private Vector3 tempAimDir;

    [Header("Player Status")]
    public bool isRunning;
    private bool isShooting;
    private bool canMove = true;

    private void Start() {
        runSpeedNormal = runSpeed;
        walkSpeedNormal = walkSpeed;

        if (isLocalPlayer)
        {
            Debug.Log("local Player" + netId);
            return;
        }

        //rb = this.gameObject.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            Debug.LogWarning("got rb");
        }
    }

    public Team PlayerTeam
    {
        get { return playerTeam; }
        set { playerTeam = value; }
    }
    public SetDeviceType PlayerDevice
    {
        get { return deviceType; }
    }
    public void setCanMove(bool newCanMove){
    this.canMove = newCanMove;
    }

    [ClientCallback]
    public void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //CHANGE TO SET WHEN GAME STARTS, OR WHEN PLAYER JOINS
        setColorsOfPlayers();

        #region PC MOVEMENT
        if (deviceType == SetDeviceType.PC){
        if(!playerCamera){
            playerCamera = GameObject.Find("ClientCamera").GetComponent<Camera>();
            print("Camera Set");
        }
        if(Input.GetKey(KeyCode.LeftShift)){
            isRunning = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift)){
            isRunning = false;
        }

            movement = new Vector2
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };
            movement = new Vector2(movement.x, movement.y).normalized * (isRunning ? runSpeed : walkSpeed) * 0.25f;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -playerCamera.transform.position.z;
        Vector3 mouseWorldPosition = playerCamera.ScreenToWorldPoint(mousePosition);
        Vector3 aimDirection = mouseWorldPosition - transform.position;
        tempAimDir = aimDirection;
        rotation = Quaternion.LookRotation(Vector3.forward, aimDirection);
        //print(rotation);
        }
        #endregion

        #region MOBILE MOVEMENT
        if (deviceType == SetDeviceType.Mobile){
    
            JoystickControllerMovement joystickController = joystickMovementUI.GetComponent<JoystickControllerMovement>();

            float horizontal = joystickController.GetHorizontalInput();
            float vertical = joystickController.GetVerticalInput();

            JoystickControllerRotationAndShooting rotationJoystick = joystickRotateUI.GetComponent<JoystickControllerRotationAndShooting>();
            rotation = rotationJoystick.GetRotationInput();
            isShooting = rotationJoystick.isShooting;
            isRunning = joystickController.isRunning;
            movement = new Vector2(horizontal, vertical).normalized * (isRunning ? runSpeed : walkSpeed);
        }
        #endregion

        #region GENERAL MOVEMENT LOGIC (APPLIES TO ALL DEVICES)
        if (canMove){
            CmdMove(movement);
            CmdRotate(rotation);
        }
        #endregion
    }

    [Command]
    private void CmdMove(Vector2 movement)
    {
        RpcMove(movement);
        transform.Translate(movement * Time.fixedDeltaTime);    
    }
    // FIGURE OUT HOW THE SERVER RECIEVES THIS INFO
    [Command]
    private void CmdRotate(Quaternion rotation)
    {
        RpcRotation(rotation);
        transform.GetChild(0).transform.rotation = rotation;
    }
    [ClientRpc]
    private void RpcMove(Vector2 movement)
    {
        transform.Translate(movement * Time.fixedDeltaTime);
    }
    [ClientRpc]
    private void RpcRotation(Quaternion rotation)
    {
        transform.GetChild(0).transform.rotation = rotation;
    }

    public override void OnStartLocalPlayer()
    {

        // Set the player's outline to Yellow
        
        /*
        foreach (NetworkConnectionToClient players in NetworkServer.connections.Values) {
            if (!isLocalPlayer)
            {
                if (players.identity.GetComponent<PlayerScript>().PlayerTeam != this.playerTeam || players.identity.GetComponent<PlayerScript>().PlayerTeam == Team.None)
                {
                    players.identity.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                }
                else
                {
                    players.identity.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
        }
        */
        this.GetComponent<Weapon>().spreadCone.enabled = true;

        // Request authority from the server
        CmdRequestAuthority();
    }
    
    [Client]
    private void setColorsOfPlayers(){
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players){
                if (player != this.gameObject)
                {
                    if (player.GetComponent<PlayerScript>().playerTeam == Team.Red || player.GetComponent<PlayerScript>().playerTeam == Team.None)
                    {
                        //Debug.LogError("SET PLAYER COLOR TO RED");
                        player.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else if (player.GetComponent<PlayerScript>().playerTeam == Team.Blue)
                    {
                        //Debug.LogError("SET PLAYER COLOR TO BLUE");
                        player.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                }
                //IF YOU WANT PLAYERS SELF COLOR TO BE YELLOW, USE THIS, IF NOT THEN JUST COMMENT IT OUT
                else if (player == this.gameObject)
                {
                    //Debug.LogWarning("SET PLAYER COLOR TO YELLOW");
                    player.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            }
            

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
