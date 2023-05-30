using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Weapon : NetworkBehaviour
{
    [SerializeField]
    public WeaponData weaponSpecs;
    public Transform firePoint;

    public SpriteRenderer weaponLooks;
    public LayerMask targetLayers;
    public GameObject bulletPrefab;
    public float fireRange = 100f;
    public int numOfBulletsPerShot;

    public float zoomValue = 100f;

    private int damage;
    private float spreadValue;

    public int magSize;

    private int currentAmmo;

    private int totalMags;
    public float fireRate = 0.2f; // added fire rate variable

    [SerializeField]
    private JoystickControllerRotationAndShooting shootingJoystick;

    private float nextFireTime = 0f;

    private bool outOfAmmo = false;
    public float reloadTime = 2f; // added reload time variable

    private bool isReloading = false; // flag to check if reloading is in progress

    private float spread = 0f;

    private RaycastHit2D tempHitLocation;
    private Vector3 tempSpreadDirection;
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
            spreadValue = weaponSpecs.spreadIncreasePerSecond;
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
        if (!isLocalPlayer){
            return;
        }
        if (isReloading)
        return;

        if (shootingJoystick.isShooting && Time.time >= nextFireTime && !outOfAmmo)
        {
            nextFireTime = Time.time + fireRate; // update the next fire time
            Vector2 direction = firePoint.transform.up; // use the up direction instead of right for topdown 2D
            float spreadAngle = Mathf.Clamp(Random.Range(0, spread) - spread / 2f,-45,45);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
            direction = spreadRotation * direction;
            CmdFire(direction); // Call the Command method to fire the bullet on the server
            spread += Time.deltaTime * spreadValue;
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
    IEnumerator Reload()
    {
        isReloading = true;
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
        tempSpreadDirection = spreadDirection;
        if (numOfBulletsPerShot > 1)
        {
            float spreadAngle = i * (spread / numOfBulletsPerShot) + Random.Range(0, spread) - spread / 2f;;
            Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
            spreadDirection = spreadRotation * direction;
        }

        var hit = Physics2D.Raycast(firePoint.position, spreadDirection, fireRange, targetLayers);
        tempHitLocation = hit;

        if (hit.collider != null)
        {
            Transform objectOrigin = hit.collider.gameObject.transform.parent;
            //Transform canvas = objectOrigin.GetChild(2).transform;
            //Slider slider = canvas.transform.GetChild(0).GetComponent<Slider>();
            if(hit.collider.gameObject.name == "Bullseye!"){
                damageDone = 2*damage;
                
                }
            else{
                damageDone = damage;
            }

            //slider.value -= damageDone;
            Debug.Log(hit.collider.gameObject.name+" "+damageDone);
            //HitText hitText = hit.collider.gameObject.GetComponentInChildren<HitText>();
            
            // Update the text and position it on the object that was hit
            //hitText.ShowHitText(hit.point,this);
            //DestroyImmediate(hit.collider.gameObject);
        }
        else
        {
            Debug.Log("Hit Nothing");
        }
    }
    
    RpcOnFire(tempHitLocation,tempSpreadDirection,damageDone);
}

[ClientRpc]
void RpcOnFire(RaycastHit2D hit, Vector3 spreadDirection,int damage)
{
    var bulletInstance = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
    BulletScript trailRender = bulletInstance.GetComponent<BulletScript>();
    if (hit.collider != null)
        {
            trailRender.SetTargetPosition(hit.point);
            Transform objectOrigin = hit.collider.gameObject.transform.parent;
            Slider slider = objectOrigin.GetChild(2).transform.GetChild(0).GetComponent<Slider>();
            slider.value -= damage;
            Debug.Log(hit.collider.gameObject.name+" "+slider.value);
        }
        else
        {
            var endPoint = firePoint.position + spreadDirection * fireRange;
            trailRender.SetTargetPosition(endPoint);
        }
}

}
