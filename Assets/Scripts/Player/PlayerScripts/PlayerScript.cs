using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerScript : NetworkBehaviour, IEffectable
{
    private Camera playerCamera;
    private StatusEffectData _statusEffectData;
    public enum Team
    {
        None,
        Red,
        Blue
    }

    [SyncVar] public Team playerTeam;

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

    [Header("Player Status")]
    public bool isRunning;
    private bool isShooting;
    private bool canMove = true;

    [SyncVar] private string playerId;

    private void Awake()
    {
        EvtSystem.EventDispatcher.AddListener<PlayerDied>(ClearStatusEffects);
        EvtSystem.EventDispatcher.AddListener<ApplyStatusEffects>(SetStatusEffectData);
    }

    private void Start() {
        runSpeedNormal = runSpeed;
        walkSpeedNormal = walkSpeed;

        if (isLocalPlayer)
        {
            Debug.Log("local Player: " + netId);
            //cmdSetPlayerId();
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


    public bool hasSetColors;
    [ClientCallback]
    public void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (hasSetColors)
        {
            setColorOfSelf();
        }

        //setColorsOfPlayers(ChaseGameManager.instance.players);

        if(_statusEffectData != null)
        {
            CmdHandleEffect();
        }
    }

    public void FixedUpdate()
    {
        if (deviceType == SetDeviceType.Auto)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                deviceType = SetDeviceType.Mobile;
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                deviceType = SetDeviceType.PC;
            }
        }

        #region PC MOVEMENT
        if (deviceType == SetDeviceType.PC)
        {
            if (!playerCamera)
            {
                playerCamera = GameObject.Find("ClientCamera").GetComponent<Camera>();
                print("Camera Set");
                //playerCamera.cullingMask += LayerMask.GetMask("Objects");
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
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
            rotation = Quaternion.LookRotation(Vector3.forward, aimDirection);
            //print(rotation);
        }
        #endregion

        #region MOBILE MOVEMENT
        if (deviceType == SetDeviceType.Mobile)
        {

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
        if (canMove)
        {
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
        //transform.Translate(movement * Time.deltaTime);
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

    [Client]
    public void setColorOfSelf()
    {
        this.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
        hasSetColors = false;
    }

    [Client]
    public void setLayerForEnemies(GameObject[] players)
    {
        if (!isLocalPlayer)
        {
            foreach(GameObject player in players)
            {
                if(player != this.gameObject)
                    player.layer = LayerMask.NameToLayer("Enemy");
            }
        }
            
    }

    [Client]
    public void setColorsOfPlayers(GameObject[] players)
    {
        foreach (GameObject player in players)
        {
            if (player != this.gameObject)
            {
                if (player.GetComponent<PlayerScript>().playerTeam == PlayerScript.Team.Red)
                {
                    //Debug.LogError("SET PLAYER COLOR TO RED");
                    player.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                }
                else if (player.GetComponent<PlayerScript>().playerTeam == PlayerScript.Team.Blue)
                {
                    //Debug.LogError("SET PLAYER COLOR TO BLUE");
                    player.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
                }
                else
                {
                    player.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = Color.magenta;
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

    #region 'IEffectable' INTERFACE FUNCTIONS
    private ParticleSystem _effectParticles;

    [Command]
    public void cmdSetPlayerId()
    {
        setPlayerId();
    }

    [ClientRpc]
    public void setPlayerId()
    {
        for (int i = 0; i < 4; i++)
            playerId += Random.Range(0, 9).ToString();
        Debug.LogError("player id: " + playerId);
    }

    [Command]
    public void CmdApplyEffect(StatusEffectData data)
    {
        ApplyEffect(data);
    }

    //POTENTIALLY WHY WE HAVE TO DO SO MANY NULL CHECKS
    [ClientRpc]
    public void ApplyEffect(StatusEffectData data)
    {
        RemoveEffect(); // REMOVE THIS LINE IF YOU WANT TO BE ABLE TO STACK STATUS EFFECTS, AND LOOP THROUGH ABILITIY STATUS EFFECTS USING 'HandleEffect'
        _statusEffectData = data;

        //_effectParticles = Instantiate(_statusEffectData.EffectParticles, transform);

        //HandleEffect();
        //Debug.LogError("Applied effect");
    }



    private float _currentEffectTime = 0f;
    private float _nextTickTime = 0f;
    [Command]
    public void CmdRemoveEffect()
    {
        RemoveEffect();
    }

    [ClientRpc]
    public void RemoveEffect()
    {
        _statusEffectData = null;
        _currentEffectTime = 0f;
        _nextTickTime = 0f;

        //if(_effectParticles != null)
        //{
        //   Destroy(_effectParticles);
        //}

        if (weapon != null)
            weapon.SetDefaultValues();

        hasApplied = false;
    }

    //CALLS IT MULTIPLE TIMES
    [Command]
    private void CmdHandleEffect()
    {
        Debug.LogError("Calls effect handling");
        if(_statusEffectData != null)
            HandleEffect();
    }

    //CALLS IT ONCE
    [Command]
    private void SetStatusEffectData(ApplyStatusEffects evtData)
    {
        //EvtSystem.EventDispatcher.RemoveListener<ApplyStatusEffects>(deleteListener);
        Debug.LogError("Object recieved from event: " + evtData.player);
        RpcSetStatusEffectData(evtData);
    }

    [ClientRpc]
    private void RpcSetStatusEffectData(ApplyStatusEffects evtData)
    {
        if (evtData.team == playerTeam)
            _statusEffectData = evtData.statusEffect;
    }

    private void deleteListener(ApplyStatusEffects evtData)
    {

    }

    PlayerHealth playerHealth;
    Weapon weapon;
    [ClientRpc]
    public void HandleEffect()
    {
        _currentEffectTime += Time.deltaTime;
        //CODE DIFFERENT CASES FOR WHAT THE STATUS EFFECT IS; STATUS EFFECTS CAN BE APPLIED THROUGH ABILITIES OR THROUGH THE DESTRUCTION OF BASES
        if (_statusEffectData != null)
        {
            if (_currentEffectTime >= _statusEffectData.activeTime) 
            { 
                RemoveEffect(); 
                Debug.LogError("status effect expired"); 
                ClearStatusEffects(); 
            }

            if (_statusEffectData != null)
            {
                if (_statusEffectData.valueOverTime != 0 && _currentEffectTime > _nextTickTime)
                {
                    _nextTickTime += _statusEffectData.tickSpeed;

                    //CHECK FOR DIFFERENT TYPES HERE

                    if (_statusEffectData.statusEffectType == StatusEffectTypes.DOT)
                    {
                        Debug.LogError("DOT Effect");

                        if (playerHealth == null)
                            playerHealth = GetComponent<PlayerHealth>();

                        ApplyDOT(playerHealth);
                    }
                    else if(_statusEffectData.statusEffectType == StatusEffectTypes.STRENGTH_BUFF)
                    {
                        Debug.LogError("Weapon Buff");

                        if(weapon == null)
                            weapon = GetComponent<Weapon>();


                        //SUPER HARD CODED FOR NOWWW, IT'S OKAY
                        Debug.LogError("THE NAME OF THE EFFECT IS: " + _statusEffectData.Name);

                        if (!hasApplied)
                        {
                            switch (_statusEffectData.Name)
                            {
                                case "Damage Buff":
                                    ApplyDamageBuff(weapon);
                                    Debug.LogError("dmg");
                                    break;
                                case "Bonus Points":
                                    BonusPointsBuff(weapon);
                                    Debug.LogError("bonus");
                                    break;
                                case "Bullet Count":
                                    BulletCountBuff(weapon);
                                    Debug.LogError("BC");
                                    break;
                                case "Fire Range":
                                    FireRangeBuff(weapon);
                                    Debug.LogError("FRange");
                                    break;
                                case "Fire Rate":
                                    FireRateBuff(weapon);
                                    Debug.LogError("Frate");
                                    break;
                                case "Num of Shots":
                                    NumOfShotsBuff(weapon);
                                    Debug.LogError("numOf");
                                    break;
                                case "Reload":
                                    ReloadBuff(weapon);
                                    Debug.LogError("reload");
                                    break;
                                default:
                                    ApplyDamageBuff(weapon);
                                    Debug.LogError("DEFAULT");
                                    break;
                            }
                        }
                    }
                    else if(_statusEffectData.statusEffectType == StatusEffectTypes.MOVEMENT)
                    {
                        Debug.LogError("Movement Buff");
                    }
                    else
                        Debug.LogError("nothing?");
                }

                //EvtSystem.EventDispatcher.AddListener<ApplyStatusEffects>(SetStatusEffectData);
            }
        }
    }

    [ClientRpc]
    public void ApplyDOT(PlayerHealth playerHealthScript)
    {
        //CHECK IF YOU WERE TRYING TO HEAL, OR TO DAMAGE
        int posOrNeg = _statusEffectData.isDOT ? -1 : 1;
        //Debug.LogError("Applying DOT To player" + gameObject.name);
        playerHealthScript.TakeDamage((int)_statusEffectData.valueOverTime * posOrNeg);
    }

    bool hasApplied;
    [ClientRpc]
    public void ApplyDamageBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.damageMultiplier = _statusEffectData.damageBuff;
    }
    [ClientRpc]
    public void ReloadBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.reloadTime *= (1 - _statusEffectData.reloadTime);
    }
    [ClientRpc]
    public void FireRateBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.fireRate /= _statusEffectData.fireRate;
    }
    [ClientRpc]
    public void FireRangeBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.fireRange *= _statusEffectData.fireRange;
    }
    [ClientRpc]
    public void BulletCountBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.magSize += _statusEffectData.magSize;
    }
    [ClientRpc]
    public void NumOfShotsBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.numOfBulletsPerShot = _statusEffectData.numOfBulletsPerShot;
    }
    [ClientRpc]
    public void BonusPointsBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.bonusPointsPerShot = _statusEffectData.bonusPointsPerShot;
    }

    public void ApplyMoveSE()
    {
        //CHECK IF YOU WERE TRYING TO SLOW, OR SPEED UP
        int posOrNeg = _statusEffectData.isDOT ? -1 : 1;

        if (_statusEffectData.isSlow) {
            runSpeed  = runSpeed  - (Mathf.Pow(_statusEffectData.movementPenalty, -2));
            walkSpeed = walkSpeed - (Mathf.Pow(_statusEffectData.movementPenalty, -2));
        }
        else
        {

        }
    }


    private void ClearStatusEffects(PlayerDied evtData)
    {
        if(evtData.playerThatDied == this.gameObject)
            RemoveEffect();

        if (weapon != null)
            weapon.damageMultiplier = 1;

        Debug.LogError("Removed all ongoing status effects on Player: " + gameObject.name);
    }

    [ClientRpc]
    private void ClearStatusEffects()
    {
        RemoveEffect();

        if (weapon != null)
        {
            weapon.damageMultiplier = 1;
            Debug.LogError("SET WEAPON DAMAGE BACK TO 1X");
        }

        Debug.LogError("Removed all ongoing status effects on Player: " + gameObject.name);
    }
    #endregion
}
