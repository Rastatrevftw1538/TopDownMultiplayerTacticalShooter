using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Cinemachine;
using Debug = UnityEngine.Debug;
using UnityEngine.InputSystem;

public class WeaponSinglePlayer : MonoBehaviour
{
    [Header("Components")]
    public WeaponDataSP weaponSpecs;
    public Transform firePoint;
    private Vector3 endPoint;
    public PlayerInputActions playerInputAction;
    private InputAction fire;

    private float coneSpreadFactor = 0.1f;
    public SpriteRenderer weaponLooks;
    public SpriteRenderer spreadCone;
    [SerializeField]
    private JoystickControllerRotationAndShooting shootingJoystick;
    public LayerMask targetLayers;
    public GameObject bulletPrefab;
    [HideInInspector] public float fireRange = 100f;
    [HideInInspector] public int numOfBulletsPerShot;
    private string gunName;

    [Header("Damage Multipliers & Ammo")]
    public float damageMultiplier;
    public float damageMultiplierBPM;
    private float damage;
    private float spreadValue;

    public int mags = 3;
    private int totalMags;
    [HideInInspector] public int magSize;
    [HideInInspector] public float fireRate = 0.2f; // added fire rate variable
    private bool semiAuto;
    private bool allowedToFire;
    private int currentAmmo;
    private float nextFireTime = 0f;
    private bool outOfAmmo = false;
    [HideInInspector] public float reloadTime = 2f; // added reload time variable

    private bool isReloading = false; // flag to check if reloading is in progress

    private float spread = 0f;

    private RaycastHit2D tempHitLocation;
    private bool shootingGun;

    private StatusEffectData _statusEffectData = null;
    [HideInInspector]public int bonusPointsPerShot;
    public GameObject BPMShotPrefab;
    public GameObject DMGShotPrefab;
    public GameObject ATKSPDShotPrefab;

    private TrailRenderer BPMShotTrail;
    private TrailRenderer DMGShotTrail;
    private TrailRenderer ATKSPDShotTrail;
    //AudioSource playerAudioSource;


    PlayerScriptSinglePlayer player;
    CinemachineImpulseSource impulseSource;
    private void Awake() {
        player = this.transform.GetComponent<PlayerScriptSinglePlayer>();
        impulseSource = this.GetComponentInParent<CinemachineImpulseSource>();
        playerInputAction = new PlayerInputActions();

        if (weaponSpecs != null){
            damage = weaponSpecs.damagePerBullet;
            fireRange = weaponSpecs.fireRange;
            numOfBulletsPerShot = weaponSpecs.numOfBulletsPerShot;
            fireRate = weaponSpecs.fireRate;
            semiAuto = weaponSpecs.semiAuto;
            weaponLooks.sprite = weaponSpecs.weaponSprite;
            magSize = weaponSpecs.ammo;
            reloadTime = weaponSpecs.reloadTime;
            spreadValue = weaponSpecs.spreadIncreasePerSecond * 1000;
            gunName = weaponSpecs.name;
        }
        currentAmmo = magSize;
        totalMags = mags;
        damageMultiplier = 1;
        bonusPointsPerShot = 0;

        spreadCone.enabled = true;

        EvtSystem.EventDispatcher.AddListener<ApplyStatusEffects>(ApplyStatusEffects);

        BPMShotPrefab.TryGetComponent<TrailRenderer>(out BPMShotTrail);
        DMGShotPrefab.TryGetComponent<TrailRenderer>(out DMGShotTrail);
        ATKSPDShotPrefab.TryGetComponent<TrailRenderer>(out ATKSPDShotTrail);
        if (!BPMShotTrail)
        {
            BPMShotTrail = BPMShotPrefab.GetComponent<TrailRenderer>();
            BPMShotTrail.widthMultiplier = 2f;
            BPMShotTrail.startColor = Color.magenta;
            BPMShotTrail.endColor = Color.blue;
        }

        if (!DMGShotTrail)
        {
            DMGShotTrail = DMGShotPrefab.GetComponent<TrailRenderer>();
            DMGShotTrail.widthMultiplier = 1.5f;
            DMGShotTrail.startColor = Color.blue;
            DMGShotTrail.endColor = Color.blue;
        }

        if (!ATKSPDShotTrail)
        {
            ATKSPDShotTrail = ATKSPDShotPrefab.GetComponent<TrailRenderer>();
            ATKSPDShotTrail.startColor = Color.yellow;
            ATKSPDShotTrail.endColor = Color.yellow;
        }
    }

    private void OnEnable()
    {
        fire = playerInputAction.Player.Fire;
        fire.Enable();
        fire.performed += Fire;
    }

    private void OnDisable()
    {
        fire.performed -= Fire;
        fire.Disable();
    }
    
    bool sentEvent;
    private void Fire(InputAction.CallbackContext context)
    {
        if (Time.time >= nextFireTime && !outOfAmmo)
        {
            nextFireTime = Time.time + fireRate;
            Vector2 direction = firePoint.transform.up;
            RpcFire(direction);
        }

        if (!sentEvent)
        {
            if(BPMManager.instance != null) BPMManager.instance.hasMoved = true;
            sentEvent = true;
        }
    }

    public float getDamage(){
        return this.damage;
    }
    public bool isOutOfAmmo(){
        return this.outOfAmmo;
    }
    public bool isGunReloading(){
        return this.isReloading;
    }
    public int getCurrentAmmo(){
        return this.currentAmmo;
    }
    public int getTotalMags(){
        if(totalMags !>= 99){
            return this.totalMags;
        }
        else{
            return 9999;
        }
    }
    public float getSpread(){
        return this.spread;
    }

    public void Update()
    {
        //if (weaponSpecs.name != gunName)
        //    SetDefaultValues();

        //for showcase, making this compatible with controller too
        if (player.PlayerDevice == PlayerScriptSinglePlayer.SetDeviceType.PC)
        {
            //Debug.LogError(Input.GetAxisRaw("Shoot Gun"));
            //if (Input.GetAxisRaw("Shoot Gun") == 1)
            //    shootingGun = true;
            //else
            //    shootingGun = false;
        }
        //else(player.PlayerDevice == PlayerScriptSinglePlayer.SetDeviceType.Mobile)
        else
        {
            shootingGun = shootingJoystick.isShooting;
        }

        /*if (shootingGun && Time.time >= nextFireTime && !outOfAmmo)
        {
            nextFireTime = Time.time + fireRate;
            Vector2 direction = firePoint.transform.up;
            RpcFire(direction);
        }*/

        if (!shootingGun)
        {
            spread = 0f;
        }
    }

    bool onBeat;
    private void FixedUpdate()
    {
        //if (isReloading)
        //    return;
        //float coneScale = 1f + (spread * coneSpreadFactor);
        //spreadCone.transform.localScale = new Vector3(Mathf.Clamp(coneScale, 0, 35), spreadCone.transform.localScale.y, 1f); //HERE
        //spreadCone.color = new Color(1, 0, 0, Mathf.Clamp((Mathf.Clamp(spread, 0f, 100f) - 0) / (100 - 0), 0.25f, 0.75f));
    }

    IEnumerator Reload()
    {
        isReloading = true;
        spread = 0;
        spreadCone.color = new Color(1,1,1,0);

        ReloadSound shootSound = new ReloadSound();
        shootSound.GunName = weaponSpecs.name;
        shootSound.position = this.transform.position;
        EvtSystem.EventDispatcher.Raise<ReloadSound>(shootSound);

        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magSize;
        if(totalMags !>= 99){
            totalMags -= 1;
        }
        isReloading = false;
    }
    IEnumerator Cooldown()
    {
        allowedToFire = false;
        yield return new WaitForSeconds(fireRate);
        allowedToFire = true;
    }
    //[Command]
    /*public void CmdFire(Vector2 direction)
    {
        RpcFire(direction);
    }*/

    BPMManager bpmManager;
    private bool CheckBPM()
    {
        if(!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        /*if (BPMManager.Instance.CanClick())
        {
            onBeat = true;
            return true;
        }*/

        if (bpmManager.CanClick())
        {
            onBeat = true;
            return true;
        }
        onBeat = false;
        return false;
    }

    //[ClientRpc]
    public void RpcFire(Vector2 direction)
    {
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(firePoint.position, direction, fireRange, targetLayers);

        Vector3 spreadDirection = direction.normalized;
        RaycastHit2D hit = new RaycastHit2D();

        string whatWasHit = "";
        bool hitWall = false;
        //IF THE BULLET DOESN'T HIT ANYTHING...
        if (hits.Length <= 0)
        {
            //Debug.LogError("hit nothing...");
            endPoint = firePoint.position + (spreadDirection * fireRange);
            RpcOnFire(hit, spreadDirection, endPoint, whatWasHit, onBeat);
           // Debug.DrawRay(firePoint.position, spreadDirection * fireRange, Color.white);
            return;
        }

        for (int i = 0; i < hits.Length && !hitWall; i++)
        {
            hit = hits[i];
            switch (hit.collider.tag)
            {
                case "Enemy":
                    //Debug.LogError("Hit: " + hit.collider.gameObject.name);
                    Transform objectOrigin = hit.collider.transform.root.GetChild(0);
                    //Debug.LogError("Trying to access: " + objectOrigin.name);
                    whatWasHit = "Enemy";
                    if (objectOrigin != null)
                    {
                        IEnemy enemy = objectOrigin.GetComponent<IEnemy>();
                        if (enemy == null) return;
                        float dmg = damage * damageMultiplier * BPMManager.instance.currentMultiplier;
                        float points = enemy.pointsPerHit + bonusPointsPerShot * BPMManager.instance.currentMultiplier;
                        if (CheckBPM() && enemy != null)
                        {
                            dmg *= damageMultiplierBPM;
                            points *= damageMultiplierBPM;
                        }
                        enemy.TakeDamage(dmg);
                        UIManager.Instance.DisplayHit(dmg, objectOrigin);
                        UIManager.Instance.AddPoints(points);
                    }
                    break;

                case "Wall":
                    hitWall = true;
                    break;
                default:
                    endPoint = firePoint.position + (spreadDirection * fireRange);
                    break;
            }  
        }
        endPoint = hit.point;
        RpcOnFire(hit, spreadDirection, endPoint, whatWasHit, onBeat);
        //Debug.DrawRay(firePoint.position, spreadDirection * fireRange, Color.blue);
    }

    //[ClientRpc]
    GameObject particleEffect;
    ParticleSystem particleSystemIns;
    GameObject tempParticle;
    //TrailRenderer trailRenderer;
    CameraShake cameraShake;
    [HideInInspector] public TrailRenderer trailRendererToUse;
    void RpcOnFire(RaycastHit2D hit, Vector3 spreadDirection, Vector3 collisionPoint, String whatWasHit, bool onBeat)
    {
        //Debug.Log("Collision Point: " + collisionPoint);
        //Debug.Log("Hit: " + whatWasHit);
        //Debug.Log("HUh? Client: " + spreadDirection);

        /*if (whatWasHit != "NOTHING")
        {
            Debug.Log("Hit " + collisionPoint);
        }
        else
        {
            //collisionPoint = spreadDirection;
            Debug.Log("Hit Nothing:"); 
        }*/


        //var bulletInstance = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        GameObject bulletInstance = ObjectPool.instance.GetPooledObject();
        bulletInstance.transform.position = firePoint.position;
        bulletInstance.SetActive(true);

        BulletScriptSP trailRender = bulletInstance.GetComponent<BulletScriptSP>();
        StartCoroutine(trailRender.SetSelfInactive(0.5f));
        trailRender.SetTargetPosition(collisionPoint);

        TrailRenderer trailRenderer = trailRender.GetComponent<TrailRenderer>();

        if (!particleEffect) particleEffect = trailRender.effectPrefab;
        if(!particleSystemIns) particleSystemIns = particleEffect.GetComponent<ParticleSystem>();
        if(!cameraShake) cameraShake = CameraShake.Instance;

        //IF ON BEAT, MAKE THE TRAIL RENDER DIFFERENT COLOR
        if (CheckBPM())
        {
            trailRendererToUse = BPMShotTrail;

            trailRenderer.widthMultiplier = trailRendererToUse.widthMultiplier;

            if (player.CurrentStatusEffect() == "Damage Buff")
            {
                trailRendererToUse = DMGShotTrail;
                trailRenderer.widthMultiplier *= DMGShotTrail.widthMultiplier;
            }
            else if (player.CurrentStatusEffect() == "Fire Rate")
            {
                trailRendererToUse = ATKSPDShotTrail;
            }
            else
            {
                trailRenderer.colorGradient = trailRendererToUse.colorGradient;
/*                trailRenderer.startColor = trailRendererToUse.startColor;
                trailRenderer.endColor = trailRendererToUse.endColor;*/
            }

            cameraShake.CustomCameraShake(impulseSource);
            if (SoundFXManager.Instance) SoundFXManager.Instance.PlaySoundFXClip(weaponSpecs.shootOnBeatSound, transform, 0.2f);

            //ALSO MAKE THE PARTICLES DIFFERENT;
            tempParticle = Instantiate(trailRender.onBeatEffectPrefab, collisionPoint, Quaternion.identity);
        }
        else
        {
            tempParticle = Instantiate(particleEffect, collisionPoint, Quaternion.identity);
            if (SoundFXManager.Instance) SoundFXManager.Instance.PlaySoundFXClip(weaponSpecs.shootSound, transform, 0.1f);
        }

        Destroy(tempParticle, 0.5f);

        if (player)
        {
            if(!trailRenderer) trailRenderer = trailRender.GetComponent<TrailRenderer>();
            if (player.CurrentStatusEffect() == "Damage Buff")
            {
                /*trailRenderer.startColor = DMGShotTrail.startColor;
                trailRenderer.endColor = DMGShotTrail.endColor;*/
                trailRenderer.colorGradient = DMGShotTrail.colorGradient;
                trailRenderer.widthMultiplier = DMGShotTrail.widthMultiplier;
            }
            else if (player.CurrentStatusEffect() == "Fire Rate")
            {
                /* trailRenderer.startColor = ATKSPDShotTrail.startColor;
                 trailRenderer.endColor = ATKSPDShotTrail.endColor;*/
                trailRenderer.colorGradient = ATKSPDShotTrail.colorGradient;
                trailRenderer.widthMultiplier = ATKSPDShotTrail.widthMultiplier;
            }
        }
        //trailRender.SetTargetPosition(collisionPoint);
        //Debug.Log("Bullet Fired Client " + collisionPoint + " direction " + spreadDirection);
    }

    AudioSource audioSource;
    private void PlaySound(AudioClip sound)
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(weaponSpecs.shootSound);
    }

    public void SetDefaultValues()
    {
        damage = weaponSpecs.damagePerBullet;
        fireRange = weaponSpecs.fireRange;
        numOfBulletsPerShot = weaponSpecs.numOfBulletsPerShot;
        fireRate = weaponSpecs.fireRate;
        weaponLooks.sprite = weaponSpecs.weaponSprite;
        magSize = weaponSpecs.ammo;
        reloadTime = weaponSpecs.reloadTime;
        spreadValue = weaponSpecs.spreadIncreasePerSecond * 1000;
        gunName = weaponSpecs.name;

        currentAmmo = magSize;
        damageMultiplier = 1;
        bonusPointsPerShot = 0;

        totalMags = mags;
        damageMultiplier = 1;
    }

    private void ApplyStatusEffects(ApplyStatusEffects evtData)
    {
        
    }
}