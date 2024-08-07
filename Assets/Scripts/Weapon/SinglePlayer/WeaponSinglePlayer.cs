using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class WeaponSinglePlayer : MonoBehaviour
{
    [Header("Components")]
    public WeaponDataSP weaponSpecs;
    public Transform firePoint;
    private Vector3 endPoint;

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
    //AudioSource playerAudioSource;


    PlayerScriptSinglePlayer player;
    private void Awake() {
        player = this.transform.GetComponent<PlayerScriptSinglePlayer>();
        //playerAudioSource = GetComponent<AudioSource>();

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
        if (weaponSpecs.name != gunName)
            SetDefaultValues();
    }

    bool onBeat;
    private void FixedUpdate()
    {
        if (isReloading)
            return;
        float coneScale = 1f + (spread * coneSpreadFactor);
        spreadCone.transform.localScale = new Vector3(Mathf.Clamp(coneScale, 0, 35), spreadCone.transform.localScale.y, 1f); //HERE
        spreadCone.color = new Color(1, 0, 0, Mathf.Clamp((Mathf.Clamp(spread, 0f, 100f) - 0) / (100 - 0), 0.25f, 0.75f));

        if (player.PlayerDevice == PlayerScriptSinglePlayer.SetDeviceType.Mobile){ 
            shootingGun = shootingJoystick.isShooting;
        }
        else if(player.PlayerDevice == PlayerScriptSinglePlayer.SetDeviceType.PC){
            shootingGun = Input.GetMouseButton(0);
        }

        if (shootingGun && Time.time >= nextFireTime && !outOfAmmo)
        {
            nextFireTime = Time.time + fireRate;
            Vector2 direction = firePoint.transform.up;
            CmdFire(direction);
        }

        if(!shootingGun){
            spread = 0f;
        }
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
    public void CmdFire(Vector2 direction)
    {
        RpcFire(direction);
    }

    private bool CheckBPM()
    {
        if (BPMManager.instance.CanClick())
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
        RaycastHit2D hit = new RaycastHit2D();
        hit.point = firePoint.position;

        Vector3 spreadDirection = direction;

        string whatWasHit = "";
        bool hitWall = false;
        //IF THE BULLET DOESN'T HIT ANYTHING...
        if (hits.Length <= 0)
        {
            Debug.LogError("hit nothing...");
            endPoint = firePoint.position + (spreadDirection * fireRange);
            RpcOnFire(hit, spreadDirection, endPoint, whatWasHit, onBeat);
            return;
        }

        for (int i = 0; i < hits.Length && !hitWall; i++)
        {
            hit = hits[i];
            switch (hit.collider.tag)
            {
                case "Enemy":
                    Transform objectOrigin = hit.collider.transform;
                    whatWasHit = "Enemy";
                    if (objectOrigin != null)
                    {
                        IEnemy enemy = objectOrigin.GetComponent<IEnemy>();
                        float dmg = damage;
                        float points = enemy.pointsPerHit;
                        if (CheckBPM() && enemy != null)
                        {
                            dmg *= damageMultiplierBPM;
                            points *= damageMultiplierBPM;
                        }
                        enemy.TakeDamage(dmg);
                        UIManager.Instance.AddPoints(points);
                    }
                    break;

                case "Wall":
                    hitWall = true;
                    break;
                default:

                    break;
            }
        }
        endPoint = hit.point;
        RpcOnFire(hit, spreadDirection, endPoint, whatWasHit, onBeat);
    }

    //[ClientRpc]
    BulletScript trailRender;
    GameObject particleEffect;
    ParticleSystem particleSystemIns;
    GameObject tempParticle;
    TrailRenderer trailRenderer;
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
        
        var bulletInstance = Instantiate(bulletPrefab, firePoint.position, new Quaternion(0, 0, 0, 0));

        if(!trailRender) trailRender = bulletInstance.GetComponent<BulletScript>();
        if(!particleEffect) particleEffect = trailRender.effectPrefab;
        if(!particleSystemIns) particleSystemIns = particleEffect.GetComponent<ParticleSystem>();

        tempParticle = Instantiate(particleEffect, collisionPoint, new Quaternion(0, 0, 0, 0));
        Destroy(tempParticle, 0.5f);

        //IF ON BEAT, MAKE THE TRAIL RENDER DIFFERENT COLOR
        if (CheckBPM())
        {
            if (!trailRenderer) trailRenderer = trailRender.GetComponent<TrailRenderer>();

            trailRenderer.widthMultiplier = 2f;
            trailRenderer.startColor = Color.magenta;
            trailRenderer.endColor = Color.blue;

            //some camera shake stuff (unoptimized)
            //camera shake
            StartCoroutine(ClientCamera.Instance.cameraShake.CustomCameraShake(0.1f, 0.2f));
        }

        trailRender.SetTargetPosition(collisionPoint);
        //if (SoundFXManager.Instance) SoundFXManager.Instance.PlaySoundFXClip(weaponSpecs.shootSound, transform, 1f);
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