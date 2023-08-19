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


    PlayerScript player;
    private void Awake() {
        player = this.transform.GetComponent<PlayerScript>();

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
        spreadCone.transform.localScale = new Vector3(Mathf.Clamp(coneScale,0,35), spreadCone.transform.localScale.y, 1f); //HERE
        spreadCone.color = new Color(1,0,0,Mathf.Clamp((Mathf.Clamp(spread,0f,100f)-0)/(100-0),0.25f,0.75f));
        if(player.PlayerDevice == PlayerScript.SetDeviceType.Mobile){
            shootingGun = shootingJoystick.isShooting ;
        }
        else if(player.PlayerDevice == PlayerScript.SetDeviceType.PC){
            shootingGun = Input.GetMouseButton(0);
        }
        if (shootingGun && Time.time >= nextFireTime && !outOfAmmo)
        {
            nextFireTime = Time.time + fireRate;
            Vector2 direction = firePoint.transform.up;
            float spreadAngle = Mathf.Clamp(UnityEngine.Random.Range(0, spread) - spread / 2f,-45,45); //HERE
            Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
            direction = spreadRotation * direction;
            print("Direction thing: "+direction);
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

                        if (enemyHealth != null && !foundWhatHit && playerScript != null)
                        {
                            if (playerScript.playerTeam != player.playerTeam)
                            {
                                if (hit.collider.gameObject.name == "Bullseye!")
                                {
                                    damageDone = 2 * (damage * damageMultiplier);
                                }
                                else
                                {
                                    damageDone = (damage * damageMultiplier);
                                }
                                enemyHealth.TakeDamage(damageDone);
                            }

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

                                        /*ApplyStatusEffects applyStatusEffects = new ApplyStatusEffects();
                                        applyStatusEffects.team = GetComponent<PlayerScript>().playerTeam;
                                        applyStatusEffects.statusEffect = baseHealthEffects.statusEffect;

                                        EvtSystem.EventDispatcher.Raise<ApplyStatusEffects>(applyStatusEffects);*/
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
                                ChaseGameManager.instance.bluePoints += damageDone + bonusPointsPerShot;
                            else
                                ChaseGameManager.instance.redPoints += damageDone + bonusPointsPerShot;
                        }
                    }
                }
            }

            else
            {
                whatWasHit = "NOTHING";
                endPoint = Vector3.zero;
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
        
        var bulletInstance = Instantiate(bulletPrefab, firePoint.position, new Quaternion(0, 0, 0, 0));
        BulletScript trailRender = bulletInstance.GetComponent<BulletScript>();
        GameObject particleEffect = trailRender.effectPrefab;
        ParticleSystem particleSystem = particleEffect.GetComponent<ParticleSystem>();
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
            else{
                main.startColor = Color.clear;
            }
        Instantiate(trailRender.effectPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        trailRender.SetTargetPosition(collisionPoint);
        Debug.Log("Bullet Fired Client " + collisionPoint + " direction " + spreadDirection);
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
}
