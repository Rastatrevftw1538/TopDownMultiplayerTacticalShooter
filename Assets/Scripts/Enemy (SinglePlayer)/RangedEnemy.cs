using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour, IEnemy
{
    [Header("Enemy Stats")]
    public float maxHealth;
    [SerializeField] private float currentHealth;
    public float respawnTime = 2f;
    [SerializeField] private Transform target;
    private NavMeshAgent agent; 
    public float stoppingDistance;
    public float startShotCooldown;
    public float touchDamage;
    [field: SerializeField] public float pointsPerHit { get; set; }
    private float shotCooldown;

    [Header("Enemy Components")]
    [SerializeField] private Image healthbarExternal;
    [SerializeField] private GameObject projectile;

    [Header("Debug")]
    [SerializeField] private float _damageTaken = 0;
    [SerializeField] private bool isAlive = true;
    public bool canHit = true;

    [Header("Hit Display")]
    //public GameObject hitDisplay;
    public float hitDisplaySeconds;

    private void Awake()
    {
        //healthbarInternal = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;

        //agent = GetComponent<NavMashAgent>();
        agent = GetComponentInParent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        shotCooldown = startShotCooldown;
        
                if (target == null && player != null)
                {
                    target = player.gameObject.transform;
                }
                else if (target == null && player == null)
                {
                    target = GameObject.Find("Player - SinglePlayer").transform;
                    player = target.GetComponent<PlayerHealthSinglePlayer>();
                }
        //player = PlayerHealthSinglePlayer.Instance;
        //target = PlayerHealthSinglePlayer.Instance.gameObject.transform;
    }

    private void Update()
    {
        //healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
        agent.SetDestination(target.position);

        //DISTANCE BEFORE STOPPING
        if(agent.remainingDistance <= stoppingDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

        if (shotCooldown <= 0)
        {
            Attack();
        }
        else
        {
            shotCooldown -= Time.deltaTime;
        }

        Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y); //FIND DIRECTION OF PLAYER
        transform.up = direction; //ROTATES THE ENEMY TO THE PLAYER 

        //attack
    }

    static PlayerHealthSinglePlayer player;
    public void OnTriggerEnter2D(Collider2D other)
    {
        //attack
        if (other.gameObject.tag == "Player")
        {
            if (!player)
                player = other.gameObject.GetComponent<PlayerHealthSinglePlayer>();

            player.TakeDamage(touchDamage);
        }
    }

    private void Attack()
    {
        GameObject currentProjectile = Instantiate(projectile, transform.position, transform.rotation);
        shotCooldown = startShotCooldown;
    }

    public bool checkIfAlive
    {
        get { return isAlive; }
    }

    public float GetHealth()
    {
        return currentHealth;
    }
    public void setHealth(int healthValue)
    {
        this.currentHealth = healthValue;
    }

    private float timeSinceLastShot;
    public void TakeDamage(float amount)
    {
        //FIRST CHECK IF THE BASE'S HEALTH IS BELOW 0
        if (currentHealth > 0)
            currentHealth -= amount;
        
        CheckHealth();

        //THEN CHECK FOR ALL THE OTHER DAMAGE CONDITIONS
        _damageTaken += amount;

        timeSinceLastShot = Time.deltaTime;

        DisplayHit(amount);
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;

        //SLOW ENEMY

    }

    public List<GameObject> hitDisplays = new List<GameObject>();
    private void DisplayHit(float amount)
    {
        //INSTANTIATE A HITDISPLAY
        //GameObject currentDisplay = Instantiate(hitDisplay, gameObject.transform);
        //hitDisplays.Add(currentDisplay);

       // currentDisplay.transform.position.y += 0.10f;
    }

    private void EndVulnerability()
    {
        this.canHit = false;
        _damageTaken = 0;
    }

    private bool isRespawning;
    private void RestoreHealth()
    {
        this.transform.parent.gameObject.SetActive(true);
        currentHealth = maxHealth;
        isAlive = true;
        isRespawning = false;
        canHit = true;
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
        _damageTaken = 0f;
    }

    public void Respawn(float respawnTime)
    {
        isRespawning = true;
        canHit = false;

        //ACTUALLY RESTORE HEALTH TO THE BASE
        Invoke(nameof(RestoreHealth), respawnTime);
    }

    void RpcDie()
    {
        isAlive = false;
        //this.transform.parent.gameObject.SetActive(false);
        //Respawn(respawnTime);
        Destroy(this.transform.parent.gameObject);

        WaveManager.Instance.enemiesKilled++;
    }

    private void CheckHealth()
    {
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RpcDie();
        }
    }
}
/*
bool foundWhatHit = false;
//training dummy
if (!foundWhatHit)
{
    TrainingDummy dummyHealth = objectOrigin.GetComponent<TrainingDummy>();
    if (dummyHealth != null && !foundWhatHit)
    {
        if (dummyHealth.canHit)
        {
            //CLICKED ON BEAT?
            if (BPMManager.instance.canClick == Color.green)
            {
                Debug.LogError("ON BEAT :)!! HIT DUMMY FOR " + damage * damageMultiplierBPM);
                damageDone = (damage * damageMultiplierBPM);
            }
            else
            {
                Debug.LogError("NOT ON BEAT :(!! HIT DUMMY FOR " + damage);
                damageDone = (damage);
            }
            dummyHealth.TakeDamage(damageDone);
        }

        foundWhatHit = true;
    }
}

//enemy

if (!foundWhatHit)
{
    Debug.LogError("made it here1");
    RangedEnemy dummyHealth = objectOrigin.GetComponent<RangedEnemy>();
    if (dummyHealth != null && !foundWhatHit)
    {
        Debug.LogError("made it here1a");
        if (dummyHealth.canHit)
        {
            Debug.LogError("made it here1b");
            //CLICKED ON BEAT?
            if (BPMManager.instance.canClick == Color.green)
            {
                Debug.LogError("ON BEAT :)!! HIT ENEMY FOR " + damage * damageMultiplierBPM);
                damageDone = (damage * damageMultiplierBPM);
            }
            else
            {
                Debug.LogError("NOT ON BEAT :(!! HIT ENEMY FOR " + damage);
                damageDone = (damage);
            }
            dummyHealth.TakeDamage(damageDone);
        }

        foundWhatHit = true;
    }
}

if (!foundWhatHit)
{
    Debug.LogError("made it here2");
    MeleeEnemy dummyHealth = objectOrigin.GetComponent<MeleeEnemy>();
    if (dummyHealth != null && !foundWhatHit)
    {
        Debug.LogError("made it here2a");
        if (dummyHealth.canHit)
        {
            Debug.LogError("made it here2b");
            //CLICKED ON BEAT?
            if (BPMManager.instance.canClick == Color.green)
            {
                Debug.LogError("ON BEAT :)!! HIT ENEMY FOR " + damage * damageMultiplierBPM);
                damageDone = (damage * damageMultiplierBPM);
            }
            else
            {
                Debug.LogError("NOT ON BEAT :(!! HIT ENEMY FOR " + damage);
                damageDone = (damage);
            }
            dummyHealth.TakeDamage(damageDone);
        }

        foundWhatHit = true;
    }
}*/