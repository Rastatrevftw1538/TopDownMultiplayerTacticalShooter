using System;
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
    private Transform firePoint;
    private Vector2 endPoint;

    public SpriteRenderer weaponLooks;
    public SpriteRenderer spreadCone;
    private float coneSpreadFactor = 0.1f;
    public LayerMask targetLayers;
    public GameObject bulletPrefab;
    private float fireRange = 100f;
    private int numOfBulletsPerShot;

    private float zoomValue = 100f;

    private int damage;
    private float spreadValue;

    public int magSize;

    private int currentAmmo;

    private int totalMags;
    private float fireRate = 0.2f; // added fire rate variable

    [SerializeField]
    private JoystickControllerRotationAndShooting shootingJoystick;

    private float nextFireTime = 0f;

    private bool outOfAmmo = false;
    private float reloadTime = 2f; // added reload time variable

    private bool isReloading = false; // flag to check if reloading is in progress

    private float spread = 0f;

    private RaycastHit2D tempHitLocation;

    //private Vector3 tempSpreadDirection;
    private void Awake() {
        if(weaponSpecs != null){
            damage = weaponSpecs.damagePerBullet;
            fireRange = weaponSpecs.fireRange;
            numOfBulletsPerShot = weaponSpecs.numOfBulletsPerShot;
            fireRate = weaponSpecs.fireRate;
            weaponLooks.sprite = weaponSpecs.weaponSprite;
            magSize = weaponSpecs.ammo;
            reloadTime = weaponSpecs.reloadTime;
            zoomValue = weaponSpecs.zoomOutValue;
            spreadValue = weaponSpecs.spreadIncreasePerSecond * 1000;
        }
        currentAmmo = magSize;
        totalMags = 3;
    }
    public int getDamage(){
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
        return this.totalMags;
    }
    public float getSpread(){
        return this.spread;
    }

    private void Update()
    {
        if (isLocalPlayer){

        if (isReloading)
        return;
        float coneScale = 1f + (spread * coneSpreadFactor);

        spreadCone.transform.localScale = new Vector3(Mathf.Clamp(coneScale,0,35), spreadCone.transform.localScale.y, 1f);
        //Debug.Log(Mathf.Clamp((Mathf.Clamp(spread,0f,100f)-0)/(100-0),0.25f,0.75f));
        spreadCone.color = new Color(1,0,0,Mathf.Clamp((Mathf.Clamp(spread,0f,100f)-0)/(100-0),0.25f,0.75f));
        if (shootingJoystick.isShooting && Time.time >= nextFireTime && !outOfAmmo)
        {
            nextFireTime = Time.time + fireRate; // update the next fire time
            Vector2 direction = firePoint.transform.up; // use the up direction instead of right for topdown 2D
            float spreadAngle = Mathf.Clamp(UnityEngine.Random.Range(0, spread) - spread / 2f,-45,45);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
            direction = spreadRotation * direction;
            CmdFire(direction); // Call the Command method to fire the bullet on the server
            if(this.GetComponent<PlayerScript>().isRunning){
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
        if(totalMags<0){
            outOfAmmo = true;
            weaponLooks.color = Color.red;
        }
        if(!shootingJoystick.isShooting){
            spread = 0f;
        }
    }
    }
    IEnumerator Reload()
    {
        isReloading = true;
        spread = 0;
        spreadCone.color = new Color(1,1,1,0);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magSize;
        totalMags -= 1;
        isReloading = false;
    }
[Command]
void CmdFire(Vector2 direction)
{
    var damageDone = 0;
    for (int i = 0; i < numOfBulletsPerShot; i++)
    {
        Vector3 spreadDirection = direction;
        if (numOfBulletsPerShot > 1)
        {
            float spreadAngle = Mathf.Clamp(i * (spread / numOfBulletsPerShot) + UnityEngine.Random.Range(0, spreadValue) - spreadValue / 2f,-5,5);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
            spreadDirection = spreadRotation * direction;
            //Debug.Log(spread);
        }

            var hit = Physics2D.Raycast(firePoint.position, spreadDirection, fireRange, targetLayers);
            bool hitSomething = false;
        if (hit.collider != null)
        {
                hitSomething = true;
            if (hit.collider.name.Equals("HitBox") || hit.collider.name.Equals("Bullseye!")){
            Transform objectOrigin = hit.collider.transform.parent.parent;
            if (objectOrigin != null)
            {
                //Debug.Log(objectOrigin.name);
                PlayerHealth enemyHealth = objectOrigin.GetComponent<PlayerHealth>();
                if(enemyHealth != null){
                    if(hit.collider.gameObject.name == "Bullseye!"){
                        damageDone = 2*damage;
                        
                        }
                    else{
                        damageDone = damage;
                    }
                    enemyHealth.TakeDamage(damageDone);
                }
            }
            }
        }
            endPoint = hit.point;
            //colliderHit = hit.collider;
        Debug.Log("Bullet Fired Server " + hit.point+" direction "+spreadDirection);
        RpcOnFire(hit, spreadDirection,endPoint, hitSomething);
    }
}

[ClientRpc]
void RpcOnFire(RaycastHit2D hit, Vector3 spreadDirection,Vector3 collisionPoint,bool hitSomething)
{
        Debug.Log("Collision Point: " + collisionPoint);
        Debug.Log("Hit: " + hit);
        if (hitSomething)
    {
            //endPoint = hit.point;
            Debug.Log("Hit " + collisionPoint);
            
    }
    else
    {
            collisionPoint = (collisionPoint + spreadDirection * fireRange);
            Debug.Log("Hit Nothing:");

        }
        
    var bulletInstance = Instantiate(bulletPrefab, firePoint.position, new Quaternion(0,0,0,0));
    BulletScript trailRender = bulletInstance.GetComponent<BulletScript>();
        //Debug.Log("DIs is blood " + trailRender.bloodPrefab.name);
        /*
        if (hit.collider.tag == "Player")
        {
            trailRender.SetColor(Color.red);
        }
        else if (hit.collider.tag == "Wall") {
            trailRender.SetColor(Color.grey);
        }
        */
        Instantiate(trailRender.effectPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        trailRender.SetTargetPosition(collisionPoint);
        
        
        Debug.Log("Bullet Fired Client " + hit.point+ " direction "+spreadDirection);
    }
}
