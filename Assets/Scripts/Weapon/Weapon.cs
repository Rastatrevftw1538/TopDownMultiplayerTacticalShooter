﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Weapon : NetworkBehaviour
{
    [SerializeField]
    public WeaponData weaponSpecs;
    [SerializeField]
    public Transform firePoint;
    private Vector3 endPoint;

    public SpriteRenderer weaponLooks;
    public SpriteRenderer spreadCone;
    private float coneSpreadFactor = 0.1f;
    public LayerMask targetLayers;
    public GameObject bulletPrefab;
    [HideInInspector] public float fireRange = 100f;
    [HideInInspector] public int numOfBulletsPerShot;

    private float damage;
    public float damageMultiplier;
    private float spreadValue;

    public int mags = 3;
    [HideInInspector] public int magSize;

    private int currentAmmo;

    private int totalMags;
    [HideInInspector] public float fireRate = 0.2f; // added fire rate variable

    [SerializeField]
    private JoystickControllerRotationAndShooting shootingJoystick;

    private float nextFireTime = 0f;

    private bool outOfAmmo = false;
    [HideInInspector] public float reloadTime = 2f; // added reload time variable

    private bool isReloading = false; // flag to check if reloading is in progress

    private float spread = 0f;

    private RaycastHit2D tempHitLocation;
    private bool shootingGun;

    private StatusEffectData _statusEffectData = null;
    [HideInInspector]public int bonusPointsPerShot;
    AudioSource weaponAudioSource;

    [Header("Gun Sounds")]
    public List<AudioClip> ARSounds      = new List<AudioClip>();
    public List<AudioClip> SMGSounds     = new List<AudioClip>();
    public List<AudioClip> ShotgunSounds = new List<AudioClip>();
    public List<AudioClip> FamasSounds   = new List<AudioClip>();
    public List<AudioClip> SniperSounds  = new List<AudioClip>();

    public AudioClip ARSoundsReload;
    public AudioClip SMGSoundsReload;
    public AudioClip ShotgunSoundsReload;
    public AudioClip FamasSoundsReload;
    public AudioClip SniperSoundsReload;

    public AudioClip headShotSound;

    private AudioClip currentGunSound = null;

    PlayerScript player;
    private void Awake() {
        player = this.transform.GetComponent<PlayerScript>();

        try { weaponAudioSource = GetComponent<AudioSource>();  }
        catch { 
            weaponAudioSource = gameObject.AddComponent<AudioSource>();
            weaponAudioSource.volume = 0.3f;
        }

        if (weaponSpecs != null){
            damage = weaponSpecs.damagePerBullet;
            fireRange = weaponSpecs.fireRange;
            numOfBulletsPerShot = weaponSpecs.numOfBulletsPerShot;
            fireRate = weaponSpecs.fireRate;
            weaponLooks.sprite = weaponSpecs.weaponSprite;
            magSize = weaponSpecs.ammo;
            reloadTime = weaponSpecs.reloadTime;
            spreadValue = weaponSpecs.spreadIncreasePerSecond * 1000;
        }
        currentAmmo = magSize;
        totalMags = mags;
        damageMultiplier = 1;
        bonusPointsPerShot = 0;

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

    private void Update()
    {
        if (!isOwned)
            return;

        if (isReloading)
            return;

        float coneScale = 1f + (spread * coneSpreadFactor);
        spreadCone.transform.localScale = new Vector3(Mathf.Clamp(coneScale, 0, 35), spreadCone.transform.localScale.y, 1f);
        spreadCone.color = new Color(1, 0, 0, Mathf.Clamp((Mathf.Clamp(spread, 0f, 100f) - 0) / (100 - 0), 0.25f, 0.75f));

        if (player.PlayerDevice == PlayerScript.SetDeviceType.Mobile){
            shootingGun = shootingJoystick.isShooting ;
        }
        else if(player.PlayerDevice == PlayerScript.SetDeviceType.PC){
            shootingGun = Input.GetMouseButton(0);
        }
        if (shootingGun && Time.time >= nextFireTime && !outOfAmmo)
        {
            //TIME BETWEEN SHOTS
            nextFireTime = Time.time + fireRate;

            //WEAPON SPREAD
            Vector2 direction = firePoint.transform.up;
            float spreadAngle = Mathf.Clamp(UnityEngine.Random.Range(0, spread) - spread / 2f,-45,45);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
            direction = spreadRotation * direction;

            print("Direction: " + direction);

            CmdFire(direction);

            if(player.isRunning){
                spread += Time.deltaTime * (spreadValue*2);
            }
            else{
                spread += Time.deltaTime * spreadValue;
            }
            currentAmmo -= 1;
        }

        if(currentAmmo <= 0){
            StartCoroutine(Reload());
            //SOUND
            weaponAudioSource.PlayOneShot(getAudioForGunReload());
            return;
        }

        if(totalMags < 0){
            outOfAmmo = true;
            weaponLooks.color = Color.red;
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
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magSize;
        if(totalMags !>= 99){
            totalMags -= 1;
        }
        isReloading = false;
    }

    [Command]
    public void CmdFire(Vector2 direction)
    {
        RpcFire(direction);
    }

    [ClientRpc]
    public void RpcFire(Vector2 direction)
    {
        float damageDone = 0;
        for (int i = 0; i < numOfBulletsPerShot; i++)
        {
            //CinemachineShake.Instance.ShakeCamera(5f, .1f);
            Vector3 spreadDirection = direction;
            print("Direction thing SERVER: " + spreadDirection);
            if (numOfBulletsPerShot > 1)
            {
                float spreadAngle = Mathf.Clamp(i * (spread / numOfBulletsPerShot) + UnityEngine.Random.Range(0, spreadValue) - spreadValue / 2f, -5, 5);
                Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
                spreadDirection = spreadRotation * direction;
            }

            var hit = Physics2D.Raycast(firePoint.position, spreadDirection, fireRange, targetLayers);
            String whatWasHit = "";
            if (hit.collider != null)
            {
                whatWasHit = hit.collider.tag;
                if (hit.collider.name.Equals("HitBox") || hit.collider.name.Equals("Bullseye!"))
                {
                    Transform objectOrigin = hit.collider.transform.parent.parent;
                    if (objectOrigin != null)
                    {
                        bool foundWhatHit = false;
                        //PLAYER HEALTH STUFF
                        PlayerHealth enemyHealth = objectOrigin.GetComponent<PlayerHealth>();
                        PlayerScript playerScript = objectOrigin.GetComponent<PlayerScript>();
                        float numPoints = 0;

                        if (enemyHealth != null && !foundWhatHit && playerScript != null)
                        {
                            if (playerScript.playerTeam != player.playerTeam)
                            {
                                if (hit.collider.gameObject.name == "Bullseye!")
                                {
                                    damageDone = 2 * (damage * damageMultiplier);
                                    currentGunSound = headShotSound;
                                }
                                else
                                {
                                    damageDone = (damage * damageMultiplier);

                                    //SOUND
                                    currentGunSound = getAudioForGun();
                                }
                                enemyHealth.TakeDamage(damageDone);
                            }

                            if (enemyHealth.checkIfAlive)
                                numPoints = damageDone + bonusPointsPerShot;
                            else
                                numPoints = 0f;

                            foundWhatHit = true;
                        }

                        //DAMAGE BASE STUFF
                        if (!foundWhatHit)
                        {
                            Base baseHealth = objectOrigin.GetComponent<Base>();
                            if (baseHealth != null && !foundWhatHit)
                            {
                                Debug.Log("<color=orange>did grab base </color>");
                                if (baseHealth.canHit && !baseHealth.CompareTag(player.playerTeam.ToString()))
                                {
                                    damageDone = (damage * damageMultiplier) * (2 / (NetworkServer.connections.Count / 2));
                                    print(NetworkServer.connections.Count);
                                    print("<color=yellow> Damage to base: " + damageDone + "</color>");
                                    baseHealth.TakeDamage(damageDone);
                                    Debug.Log("<color=orange>did Hit base </color>");
                                }

                                foundWhatHit = true;
                            }
                        }

                        if (!foundWhatHit)
                        {
                            BaseEffects baseHealthEffects = objectOrigin.GetComponent<BaseEffects>();
                            if (baseHealthEffects != null)
                            {

                                Debug.Log("<color=orange>Grabbed base: " + baseHealthEffects.gameObject.name + "</color>");
                                if (baseHealthEffects.canHit && baseHealthEffects.GetHealth() >= 0)
                                {
                                    damageDone = (damage * damageMultiplier);// * (2 / (NetworkServer.connections.Count / 2));
                                    print(NetworkServer.connections.Count);
                                    baseHealthEffects.TakeDamage(damageDone);

                                    if (baseHealthEffects.GetHealth() <= 0)
                                    {
                                        baseHealthEffects.TakeDamage(damageDone);

                                        WhoBrokeBase playerWhoBrokeBase = new WhoBrokeBase();
                                        playerWhoBrokeBase.playerTeam = player.playerTeam;
                                        playerWhoBrokeBase.whatBase = baseHealthEffects.gameObject;
                                        EvtSystem.EventDispatcher.Raise<WhoBrokeBase>(playerWhoBrokeBase);
                                    }
                                }
                                else
                                {
                                    Debug.LogError("Base cannot be hit");
                                }

                                foundWhatHit = true;
                                Debug.LogWarning("<color=yellow> Damage to base: " + damageDone + "</color>");
                                Debug.LogWarning("<color=yellow> Base health: " + baseHealthEffects.GetHealth() + "</color>");
                            }
                        }

                        //APPLY POINTS
                        if (ChaseGameManager.instance != null)
                        {
                            if (player.playerTeam == PlayerScript.Team.Blue)
                                ChaseGameManager.instance.bluePoints += numPoints;
                            else
                                ChaseGameManager.instance.redPoints += numPoints;
                        }
                    }
                }
            }

            else
            {
                whatWasHit = "NOTHING";
                endPoint = Vector3.zero;
                currentGunSound = getAudioForGun();
            }

            Debug.Log("HUh? Server: " + spreadDirection);

            if (endPoint == Vector3.zero)
            {
                //BOOOOOOOOOOOOOOOOOOM
                Debug.DrawLine(firePoint.position, firePoint.position + (spreadDirection * fireRange), Color.red);
                endPoint = new Vector3(firePoint.position.x, firePoint.position.y, firePoint.position.z) + (spreadDirection * fireRange);
            }
            else
            {
                endPoint = hit.point;
            }
            RpcOnFire(hit, spreadDirection, endPoint, whatWasHit);
        }
    }


    //BulletScript trailRender  = null;
    //GameObject particleEffect = null;
    //ParticleSystem particleSystem = null;
    [ClientRpc]
    void RpcOnFire(RaycastHit2D hit, Vector3 spreadDirection, Vector3 collisionPoint, String whatWasHit)
    {
        Debug.Log("Collision Point: " + collisionPoint);
        Debug.Log("Hit: " + whatWasHit);
        Debug.Log("HUh? Client: " + spreadDirection);
        
        if (whatWasHit != "NOTHING")
        {
            Debug.Log("Hit " + collisionPoint);
        }
        else
        {
            //collisionPoint = spreadDirection;
            Debug.Log("Hit Nothing:");
        }
        
        var bulletInstance = Instantiate(bulletPrefab, firePoint.position, new Quaternion(0, 0, 0, 0)); //INSTANTIATE ACTUAL BULLET

        BulletScript trailRender = bulletInstance.GetComponent<BulletScript>();
        GameObject particleEffect = trailRender.effectPrefab;
        ParticleSystem particleSystem = particleEffect.GetComponent<ParticleSystem>();

        //DETERMINE PARTICLE COLORS FOR THE HIT OBJECT
        var main = particleSystem.main;
            if(whatWasHit == "Base"){
                main.startColor = Color.cyan;
            }
            else if(whatWasHit == "Player"){
                main.startColor = Color.red;
            }
            else if(whatWasHit == "Wall"){
                main.startColor = Color.gray;
            }

        //SOUND
        weaponAudioSource.PlayOneShot(currentGunSound);

        Instantiate(trailRender.effectPrefab, collisionPoint, new Quaternion(0, 0, 0, 0)); // INSTANTIATE TRAIL RENDER

        trailRender.SetTargetPosition(collisionPoint); // ENSURE THE TRAIL VISUALLY LINES UP WITH THE ACTUAL BULLET
        Debug.Log("Bullet Fired from Client: " + collisionPoint + ", in direction: " + spreadDirection);
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

        totalMags = mags;
        damageMultiplier = 1;
    }

    private void ApplyStatusEffects(ApplyStatusEffects evtData)
    {
        
    }

    private AudioClip getAudioForGun()
    {
        List<AudioClip> whichGunSounds = new List<AudioClip>();

        if (weaponSpecs.name == "AR") whichGunSounds = ARSounds;
        if (weaponSpecs.name == "SMG") whichGunSounds = SMGSounds;
        if (weaponSpecs.name == "Famas") whichGunSounds = FamasSounds;
        if (weaponSpecs.name == "Shotgun") whichGunSounds = ShotgunSounds;
        if (weaponSpecs.name == "Sniper") whichGunSounds = SniperSounds;

        if (whichGunSounds.Count == 0) return null;

        int rand = UnityEngine.Random.RandomRange(0, whichGunSounds.Count - 1);

        return whichGunSounds[rand];
    }

    private AudioClip getAudioForGunReload()
    {
        AudioClip whichGunSound = null;

        if (weaponSpecs.name == "AR") whichGunSound = ARSoundsReload;
        if (weaponSpecs.name == "SMG") whichGunSound = SMGSoundsReload;
        if (weaponSpecs.name == "Famas") whichGunSound = FamasSoundsReload;
        if (weaponSpecs.name == "Shotgun") whichGunSound = ShotgunSoundsReload;
        if (weaponSpecs.name == "Sniper") whichGunSound = SniperSoundsReload;

        return whichGunSound;
    }
}
