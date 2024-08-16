using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Threading.Tasks;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEditor;

public class RangedEnemy : MonoBehaviour, IEnemy
{
    [Header("Enemy Stats")]
    [SerializeField] private float currentHealth;
    [field: SerializeField] public float maxHealth { get; set; }
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
    [field: SerializeField] public float dropChance { get; set; }
    [field: SerializeField] public List<GameObject> dropObjects { get; set; }
    [SerializeField] private LayerMask targetLayers;

    [Header("Debug")]
    [SerializeField] private float _damageTaken = 0;
    [SerializeField] private bool isAlive = true;
    public bool canHit = true;

    [Header("Hit Display")]
    //public GameObject hitDisplay;
    public float hitDisplaySeconds;
    [field: SerializeField] public AudioClip hitSound { get; set; }
    [field: SerializeField] public AudioClip movementSound { get; set; }
    [field: SerializeField] public AudioClip firingSound { get; set; }
    [field: SerializeField] public AudioClip defeatSound { get; set; }
    [Header("Flash Color")]
    public Color flashColor = Color.red;
    public float flashTime = 0.25f;
    public SpriteRenderer spriteRenderer;
    public GameObject defeatParticles;
    public GameObject onBeatDefeatParticles;
    static BPMManager bpmManager;
    private Animator anim;

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
        initColor = spriteRenderer.color;
        spriteRenderer.transform.TryGetComponent<Animator>(out anim);


        shotCooldown = startShotCooldown;

        if(!player)
            player = GameObject.FindWithTag("Player").GetComponent<PlayerHealthSinglePlayer>();
        if (!target)
        {
            target = GameObject.FindWithTag("Player").transform;
        }
        if (!bpmManager)
        {
            bpmManager = FindObjectOfType<BPMManager>();
        }
    }

    private void Update()
    {
        if (!player)
            player = GameObject.FindWithTag("Player").GetComponent<PlayerHealthSinglePlayer>();
        if (!target)
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        //healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
        agent.SetDestination(target.position);
        if(agent.velocity.magnitude > 0)
        {
            anim.SetBool("Idle", false);
            PlaySound(movementSound, 0.2f);
        }
        else
        {
            anim.SetBool("Idle", true);
        }

        //DISTANCE BEFORE STOPPING
        if(agent.remainingDistance <= stoppingDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

        if (shotCooldown <= 0 && (agent.remainingDistance*1.5f >= stoppingDistance || agent.remainingDistance <= stoppingDistance))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, stoppingDistance*1.5f, targetLayers);
            if (hit.collider)
                if (hit.collider.CompareTag("Player"))
                {
                    shotCooldown = 0f;
                    StartCoroutine(nameof(Attack));
                }
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
            player.TakeDamage(touchDamage);
        }
    }

    Color initColor;
    private IEnumerator DamageFlash()
    {
        SetFlashColor(flashColor);
        float currentFlashAmt = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmt = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));

            yield return new WaitForSeconds(0.5f);

            SetFlashColor(initColor);
        }
    }

    private void SetFlashColor(Color color)
    {
        spriteRenderer.color = color;
    }

    private IEnumerator Attack()
    {
        anim.SetBool("IsAttacking", true);
        //PlaySound(firingSound);
        Instantiate(projectile, transform.position, transform.rotation);
        shotCooldown = startShotCooldown;
        yield return new WaitForSeconds(shotCooldown);
        anim.SetBool("IsAttacking", false);
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
        PlaySound(hitSound, 0.15f);
        StartCoroutine(nameof(DamageFlash));
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

    private void PlaySound(AudioClip sound, float volume = 1f)
    {
        if (!SoundFXManager.Instance) return;
            SoundFXManager.Instance.PlaySoundFXClip(sound, transform, volume);
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

    public void DropOnDeath()
    {
        int randDropProb = Random.Range(0, 100);
        int randDropIdx = Random.Range(0, dropObjects.Count);
        if (randDropProb <= dropChance && dropObjects[randDropIdx])
            Instantiate(dropObjects[randDropIdx], transform.position, Quaternion.identity);
    }

    void RpcDie()
    {
        DropOnDeath();
        PlayParticleDefeat();
        isAlive = false;
        //this.transform.parent.gameObject.SetActive(false);
        //Respawn(respawnTime);
        PlaySound(defeatSound, 0.1f);
        Destroy(this.transform.parent.gameObject);
        UpdateEnemiesKilled();
    }

    void PlayParticleDefeat()
    {
        GameObject temp;
        if (CheckBPM())
        {
            temp = Instantiate(onBeatDefeatParticles, transform.position, Quaternion.identity);
        }
        else
        {
            temp = Instantiate(defeatParticles, transform.position, Quaternion.identity);
        }
        Destroy(temp, 0.4f);
    }

    bool CheckBPM()
    {
        if (!bpmManager) FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        if (bpmManager.CanClick())
        {
            return true;
        }
        return false;
    }

    void UpdateEnemiesKilled()
    {
        if (!WaveManager.Instance) return;
        WaveManager.Instance.enemiesKilled++;
        WaveManager.Instance.UpdateEnemiesLeft();
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