using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MeleeEnemy : MonoBehaviour, IEnemy
{
    [Header("Enemy Stats")]
    [SerializeField] private float currentHealth;
    [field: SerializeField] public float maxHealth { get; set; }
    public float damage;
    public float attackSpd;
    //public float attackRange;
    public float respawnTime = 2f;
    //private
    [field: SerializeField] public float pointsPerHit { get; set; }
    [SerializeField] static Transform target;
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Enemy Components")]
    [SerializeField] private Image healthbarExternal;
    [field: SerializeField] public float dropChance { get; set; }
    [field: SerializeField] public List<GameObject> dropObjects { get; set; }

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
    private SpriteRenderer sprite;
    public GameObject healthBar;
    public GameObject defeatParticles;
    public GameObject onBeatDefeatParticles;
    static BPMManager bpmManager;


    private void Awake()
    {
        //healthbarInternal = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;

        //agent = GetComponent<NavMashAgent>();
        anim = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>();
        sprite = GetComponent<SpriteRenderer>();
        initColor = sprite.color;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        StartCoroutine(nameof(SpawnedAnim));

        /*if (target == null && player != null)
        {
            target = player.gameObject.transform;
        } else if (target == null && player == null){
            target = GameObject.Find("Player - SinglePlayer").transform;
            player = target.GetComponent<PlayerHealthSinglePlayer>();
        }*/

        if (!player)
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
        if (spawnedAnimDone)
        {
            agent.SetDestination(target.position);

            //move to state machine
            //Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y); //FIND DIRECTION OF PLAYER
            //transform.up = direction; //ROTATES THE ENEMY TO THE PLAYER 

            //attack
            if (agent.velocity.magnitude > 0)
            {
                PlaySound(movementSound, 0.2f);
                anim.SetFloat("IsMoving", 1);
                CheckFlip(agent.velocity.x);
            }
        }
    }

    bool spawnedAnimDone;
    private IEnumerator SpawnedAnim()
    {
        spawnedAnimDone = false;
        anim.SetBool("Spawned", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("Spawned", false);
        spawnedAnimDone = true;
    }

    void CheckFlip(float velocity)
    {
        if (velocity > 0)
            sprite.flipX = false;
        else
            sprite.flipX = true;
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
        sprite.color = color;
    }

    static PlayerHealthSinglePlayer player;
    bool isAttacking;
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !isAttacking)
        {
            //player.TakeDamage(damage);
            StartCoroutine(nameof(Attack));
        }
    }

    private void SetAttackingTrue()
    {

    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetBool("IsAttacking", true);
        player.TakeDamage(damage);
        yield return new WaitForSeconds(attackSpd);
        anim.SetBool("IsAttacking", false);
        isAttacking = false;
    }

    [HideInInspector] public bool dropOnDeath { get; set; }
    public void DropOnDeath(OnDeathDrop evtData)
    {
        Instantiate(evtData.drop, transform.position, Quaternion.identity);
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

    public IEnumerator GotHit()
    {
        StartCoroutine(nameof(DamageFlash));
        PlaySound(hitSound, 0.15f);
        anim.SetBool("GotHit", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("GotHit", false);
        //Debug.Log("Set to false");
    }

    private void PlaySound(AudioClip sound, float volume = 1f)
    {
        if (!SoundFXManager.Instance) return;
        SoundFXManager.Instance.PlaySoundFXClip(sound, transform, volume);
    }

    public void TakeDamage(float amount)
    {
        StartCoroutine(nameof(GotHit));
        //FIRST CHECK IF THE BASE'S HEALTH IS BELOW 0
        if (currentHealth > 0)
            currentHealth -= amount;
        
        CheckHealth();

        //THEN CHECK FOR ALL THE OTHER DAMAGE CONDITIONS
        _damageTaken += amount;

        DisplayHit(amount);
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;

        //SLOW ENEMY

        //FLASH COLOR ENEMY
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
        PlaySound(defeatSound, 0.1f);
        isAlive = false;
        //this.transform.parent.gameObject.SetActive(false);
        //Respawn(respawnTime);
        Destroy(this.transform.parent.gameObject);

        //IDEALLY, we move this to the interface
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
