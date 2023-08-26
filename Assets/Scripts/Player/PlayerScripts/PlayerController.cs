using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    // Delegate and event for OnKill
    public delegate void KillEventHandler();
    public event KillEventHandler OnKill;

    // PlayerHealth variables
    public const int maxHealth = 100;
    [SyncVar(hook = "OnChangedHealth")]
    public int currentHealth = maxHealth;
    private Slider healthbarInternal;
    [SerializeField] private Image healthbarExternal;

    // PlayerScript variables
    [SerializeField]
    private GameObject joystickMovementHandle;
    [SerializeField]
    private GameObject joystickRotateHandle;
    public float runSpeed = 5f;
    public float walkSpeed = 0.5f;
    [SerializeField]
    private Rigidbody2D rb;
    public Vector2 position;
    public Quaternion rotate;
    public bool isRunning;
    private bool isShooting;
    private bool canMove = true;

    // PlayerUIScript variables
    Weapon gun;
    [SerializeField]
    TMP_Text ammoUI;
    public GameObject cameraHolderPrefab;
    private GameObject cameraHolder;
    public GameObject internalUI;

    // Weapon variables
    [SerializeField]
    public WeaponData weaponSpecs;
    public Transform firePoint;
    private SpriteRenderer weaponLooks;
    private SpriteRenderer spreadCone;
    private float coneSpreadFactor;
    public LayerMask targetLayers;
    public GameObject bulletPrefab;
    private float fireRange = 100f;
    private int numOfBulletsPerShot;
    private float zoomValue = 100f;
    private int damage;
    private float spreadValue;
    private int magSize;
    private int currentAmmo;
    private int totalMags;
    public float fireRate = 0.2f;
    [SerializeField]
    private JoystickControllerRotationAndShooting shootingJoystick;
    [SerializeField]
    private JoystickControllerMovement movementJoystick;
    private float nextFireTime = 0f;
    private bool outOfAmmo = false;
    private float reloadTime = 2f;
    private bool isReloading = false;
    private float spread = 0f;
    private RaycastHit2D tempHitLocation;

    private void Awake()
    {
        // Initialize variables and components
        //healthbarInternal = GetComponentInChildren<Slider>();
        ammoUI.text = currentAmmo.ToString() + " / " + totalMags.ToString();
        if (weaponSpecs != null)
        {
            damage = weaponSpecs.damagePerBullet;
            fireRange = weaponSpecs.fireRange;
            numOfBulletsPerShot = weaponSpecs.numOfBulletsPerShot;
            fireRate = weaponSpecs.fireRate;
            magSize = weaponSpecs.ammo;
            reloadTime = weaponSpecs.reloadTime;
            totalMags = 6;
        }
    }
    public override void OnStartAuthority()
    {
        // Instantiate the camera holder and activate it
        cameraHolder = Instantiate(cameraHolderPrefab, this.transform.parent);
        cameraHolder.SetActive(true);

        // Activate the internal UI
        internalUI.SetActive(true);

    }

    private void Start()
    {
        // Update health UI

        /*
        healthbarInternal.maxValue = maxHealth;
        healthbarInternal.value = currentHealth;
        */
        healthbarExternal.fillAmount = (float)currentHealth / maxHealth;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        // Player UI
        if (cameraHolder != null)
        {
            // Update the position of the camera holder to follow the player
            cameraHolder.transform.position = this.transform.GetChild(0).position;
        }
        // Player movement
        if (canMove)
        {
            float moveX = joystickMovementHandle.transform.localPosition.x;
            float moveY = joystickMovementHandle.transform.localPosition.y;
            position = new Vector2(moveX, moveY).normalized;
            isRunning = movementJoystick.isRunning;
            rb.velocity = position * (isRunning ? runSpeed : walkSpeed);
        }

        // Player shooting
        isShooting = shootingJoystick.isShooting;
        if (isShooting && !outOfAmmo && !isReloading && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }

        // Player reloading
        if (isReloading)
            return;
        if (currentAmmo <= 0 && totalMags > 0 && !isReloading && !outOfAmmo)
        {
            StartCoroutine(Reload());
            return;
        }
    }

    private void Shoot()
    {
        // Reduce ammo count
        currentAmmo--;
        ammoUI.text = currentAmmo.ToString() + " / " + totalMags.ToString();

        // Shoot bullets
        for (int i = 0; i < numOfBulletsPerShot; i++)
        {
            float coneRadius = Random.Range(0f, spread);
            float angle = Random.Range(0f, 360f);
            Vector2 conePosition = firePoint.position + Quaternion.Euler(0f, 0f, angle) * (Vector3.up * coneRadius);

            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, conePosition - (Vector2)firePoint.position, fireRange, targetLayers);

            if (hit.collider != null)
            {
                tempHitLocation = hit;
                // Apply damage to the enemy (if it has the EnemyHealth script)
                PlayerHealth enemyHealth = hit.collider.GetComponent<PlayerHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }

        // Display muzzle flash and play shooting sound

        // Check if out of ammo
        if (currentAmmo <= 0 && totalMags <= 0)
        {
            outOfAmmo = true;
            ammoUI.text = "OUT";
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        // Play reload animation

        yield return new WaitForSeconds(reloadTime);

        if (totalMags > 0)
        {
            int bulletsNeeded = magSize - currentAmmo;
            int bulletsToReload = Mathf.Min(bulletsNeeded, totalMags);
            currentAmmo += bulletsToReload;
            totalMags -= bulletsToReload;

            ammoUI.text = currentAmmo.ToString() + " / " + totalMags.ToString();

            outOfAmmo = false;
        }

        isReloading = false;
    }

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Trigger the OnKill event
            if (OnKill != null)
                OnKill.Invoke();
        }

        // Update health UI
        healthbarInternal.value = currentHealth;
        healthbarExternal.fillAmount = (float)currentHealth / maxHealth;
    }

    private void OnChangedHealth(int oldHealth, int newHealth)
    {
        // Update health UI
        healthbarInternal.value = newHealth;
        healthbarExternal.fillAmount = (float)newHealth / maxHealth;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
            rb.velocity = Vector2.zero;
    }
}
