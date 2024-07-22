using System.Linq;
using System.Numerics;
using System.IO.Enumeration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHealthSinglePlayer))]
public class PlayerScriptSinglePlayer : MonoBehaviour, IEffectable
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveStateX { get; private set; }
    public PlayerMoveState MoveStateY { get; private set; }
    public Animator Anim { get; private set; }
    public PlayerAbleToMoveState playerAbleToMove { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    #endregion

    #region Device
    private Camera playerCamera;

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
    #endregion

    #region Player Stats & Components
    [Header("Player Stats")]
    public float runSpeed = 5f;
    public float walkSpeed = 0.5f;
    [HideInInspector]
    public float runSpeedNormal;
    [HideInInspector]
    public float walkSpeedNormal;

    [Header("Player Components")]
    public UnityEngine.Vector2 movement;
    public UnityEngine.Quaternion rotation;
    public Rigidbody2D rb;
    private GameObject playerBodyArms;
    private GameObject playerBodyBody;

    private SpriteRenderer playerBodyArmsSkelSprite;
    private SpriteRenderer playerBodyBodySkelSprite;
    private StatusEffectData _statusEffectData;

    [Header("Player Status")]
    public bool isRunning;
    private bool isShooting;
    private bool canMove = true;
    #endregion

    #region Player Team
    public enum Team
    {
        None,
        Red,
        Blue
    }
    public Team playerTeam;
    private string playerId;
    #endregion

    private void Awake()
    {
        //STATUS EFFECT SETTERS
        EvtSystem.EventDispatcher.AddListener<PlayerDied>(ClearStatusEffects);
        EvtSystem.EventDispatcher.AddListener<ApplyStatusEffects>(SetStatusEffectData);

        //STATE MACHINE
        StateMachine = new PlayerStateMachine();

        IdleState  = new PlayerIdleState(this, StateMachine, "Idle");
        MoveStateX = new PlayerMoveState(this, StateMachine, "MoveHorizontal");
        MoveStateY = new PlayerMoveState(this, StateMachine, "MoveVertical");

        playerBodyArms = GameObject.Find("PlayerBody - Arms").gameObject;
        playerBodyArmsSkelSprite = playerBodyArms.transform.GetChild(0).GetComponent<SpriteRenderer>();
        playerBodyBody = GameObject.Find("PlayerBody - Body").gameObject;
        playerBodyBodySkelSprite = playerBodyBody.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Start() {
        //CACHE THE ORIGINAL VALUES SO WE CAN REVERT IT IF NECESSARY
        runSpeedNormal  = runSpeed;
        walkSpeedNormal = walkSpeed;

        //this.GetComponent<WeaponSinglePlayer>().spreadCone.enabled = true;
        Anim = gameObject.transform.GetChild(0).GetComponent<Animator>();

        //AUTOMATICALLY SET THE PLAYER'S ANIMATION STATE TO IDLE
        StateMachine.Initialize(IdleState);
    }
    public SetDeviceType PlayerDevice
    {
        get { return deviceType; }
    }
    public void setCanMove(bool newCanMove){
    this.canMove = newCanMove;
    }

    public void Update()
    {
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

            movement = new UnityEngine.Vector2
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };
            movement = new UnityEngine.Vector2(movement.x, movement.y).normalized * (isRunning ? runSpeed : walkSpeed) * 0.25f;

            UnityEngine.Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -playerCamera.transform.position.z;
            UnityEngine.Vector3 mouseWorldPosition = playerCamera.ScreenToWorldPoint(mousePosition);
            UnityEngine.Vector3 aimDirection = mouseWorldPosition - transform.position;
            rotation = UnityEngine.Quaternion.LookRotation(UnityEngine.Vector3.forward, aimDirection);
            //print(rotation);

            if(0f >= playerBodyArms.transform.localEulerAngles.z || playerBodyArms.transform.localEulerAngles.z <= 180f){
                playerBodyArmsSkelSprite.flipY = true;
                playerBodyBodySkelSprite.flipX = true;
                Debug.Log("Z Rotation is: "+playerBodyArms.transform.localEulerAngles.z);
            }
            else{
                playerBodyArmsSkelSprite.flipY = false;
                playerBodyBodySkelSprite.flipX = false;
                Debug.Log("Z Rotation is: "+playerBodyArms.transform.localEulerAngles.z);
            }
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
            movement = new UnityEngine.Vector2(horizontal, vertical).normalized * (isRunning ? runSpeed : walkSpeed);
        }
        #endregion

        #region GENERAL MOVEMENT LOGIC (APPLIES TO ALL DEVICES)
        if (canMove)
        {
            CmdMove(movement);
            CmdRotate(rotation);
        }
        #endregion

        //StateMachine.CurrentState.PhysicsUpdate();
    }

    public void SetVelocity(float x, float y)
    {
        movement.Set(x, y);
        rb.velocity = movement;
        //CurrentVelocity = movement 
    }

    private void CmdMove(UnityEngine.Vector2 movement)
    {
        //RpcMove(movement);
        transform.Translate(movement * Time.fixedDeltaTime);    
    }
    private void CmdRotate(UnityEngine.Quaternion rotation)
    {
        //RpcRotation(rotation);
        transform.GetChild(0).transform.rotation = rotation;
    }

    private void RpcMove(UnityEngine.Vector2 movement)
    {
        transform.Translate(movement * Time.fixedDeltaTime);
        //transform.Translate(movement * Time.deltaTime);
    }

    private void RpcRotation(UnityEngine.Quaternion rotation)
    {
        transform.GetChild(0).transform.rotation = rotation;
    }

    public void setLayerForEnemies(GameObject[] players)
    {
        foreach(GameObject player in players)
        {
            if(player != this.gameObject)
                player.layer = LayerMask.NameToLayer("Enemy");
        }
            
    }

    #region 'IEffectable' INTERFACE FUNCTIONS
    private ParticleSystem _effectParticles;

    public void CmdApplyEffect(StatusEffectData data)
    {
        ApplyEffect(data);
    }

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
    private void CmdHandleEffect()
    {
        Debug.LogError("Calls effect handling");
        if(_statusEffectData != null)
            HandleEffect();
    }

    //CALLS IT ONCE
    private void SetStatusEffectData(ApplyStatusEffects evtData)
    {
        //EvtSystem.EventDispatcher.RemoveListener<ApplyStatusEffects>(deleteListener);
        Debug.LogError("Object recieved from event: " + evtData.player);
        RpcSetStatusEffectData(evtData);
    }

    private void RpcSetStatusEffectData(ApplyStatusEffects evtData)
    {
        //if (evtData.team == playerTeam)
            _statusEffectData = evtData.statusEffect;
    }

    private void deleteListener(ApplyStatusEffects evtData)
    {

    }

    PlayerHealth playerHealth;
    Weapon weapon;
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

    public void ApplyDOT(PlayerHealth playerHealthScript)
    {
        //CHECK IF YOU WERE TRYING TO HEAL, OR TO DAMAGE
        int posOrNeg = _statusEffectData.isDOT ? -1 : 1;
        //Debug.LogError("Applying DOT To player" + gameObject.name);
        playerHealthScript.TakeDamage((int)_statusEffectData.valueOverTime * posOrNeg);
    }

    bool hasApplied;
    public void ApplyDamageBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.damageMultiplier = _statusEffectData.damageBuff;
    }
    public void ReloadBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.reloadTime *= (1 - _statusEffectData.reloadTime);
    }
    public void FireRateBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.fireRate /= _statusEffectData.fireRate;
    }
    public void FireRangeBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.fireRange *= _statusEffectData.fireRange;
    }
    public void BulletCountBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.magSize += _statusEffectData.magSize;
    }
    public void NumOfShotsBuff(Weapon weaponScript)
    {
        hasApplied = true;
        weaponScript.numOfBulletsPerShot = _statusEffectData.numOfBulletsPerShot;
    }
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
