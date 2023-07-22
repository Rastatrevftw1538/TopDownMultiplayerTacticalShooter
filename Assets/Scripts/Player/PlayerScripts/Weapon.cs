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
    private Vector3 endPoint;

    public SpriteRenderer weaponLooks;
    public SpriteRenderer spreadCone;
    private float coneSpreadFactor = 0.1f;
    public LayerMask targetLayers;
    public GameObject bulletPrefab;
    private float fireRange = 100f;
    private int numOfBulletsPerShot;

    private int damage;
    private float spreadValue;

    public int mags = 3;
    private int magSize;

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
    private bool shootingGun;

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
            spreadValue = weaponSpecs.spreadIncreasePerSecond * 1000;
        }
        currentAmmo = magSize;
        totalMags = mags;
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
        spreadCone.transform.localScale = new Vector3(Mathf.Clamp(coneScale,0,35), spreadCone.transform.localScale.y, 1f);
        spreadCone.color = new Color(1,0,0,Mathf.Clamp((Mathf.Clamp(spread,0f,100f)-0)/(100-0),0.25f,0.75f));
        if(this.transform.GetComponent<PlayerScript>().PlayerDevice == PlayerScript.SetDeviceType.Mobile){
            shootingGun = shootingJoystick.isShooting ;
        }
        else if(this.transform.GetComponent<PlayerScript>().PlayerDevice == PlayerScript.SetDeviceType.PC){
            shootingGun = Input.GetMouseButton(0);
        }
        if (shootingGun && Time.time >= nextFireTime && !outOfAmmo)
        {
            nextFireTime = Time.time + fireRate;
            Vector2 direction = firePoint.transform.up;
            float spreadAngle = Mathf.Clamp(UnityEngine.Random.Range(0, spread) - spread / 2f,-45,45);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
            direction = spreadRotation * direction;
            print("Direction thing: "+direction);
            CmdFire(direction);
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
    void CmdFire(Vector2 direction)
    {
        var damageDone = 0;
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
                        //PLAYER HEALTH STUFF
                        PlayerHealth enemyHealth = objectOrigin.GetComponent<PlayerHealth>();
                        if (enemyHealth != null)
                        {
                            if (hit.collider.gameObject.name == "Bullseye!")
                            {
                                damageDone = 2 * damage;
                            }
                            else
                            {
                                damageDone = damage;
                            }
                            enemyHealth.TakeDamage(damageDone);
                        }

                        //DAMAGE BASE STUFF
                        Base baseHealth = objectOrigin.GetComponent<Base>();
                        if (baseHealth != null)
                        {
                            Debug.Log("<color=orange>did grab base </color>");
                            if (baseHealth.canHit && !baseHealth.CompareTag(this.GetComponent<PlayerScript>().playerTeam.ToString()))
                            {
                                damageDone = damage * (2 / (NetworkServer.connections.Count / 2));
                                print(NetworkServer.connections.Count);
                                print("<color=yellow> Damage to base: " + damageDone + "</color>");
                                baseHealth.TakeDamage(damageDone);
                                Debug.Log("<color=orange>did Hit base </color>");
                            }
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
        Instantiate(trailRender.effectPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        trailRender.SetTargetPosition(collisionPoint);
        Debug.Log("Bullet Fired Client " + collisionPoint+ " direction "+spreadDirection);
    }
}
