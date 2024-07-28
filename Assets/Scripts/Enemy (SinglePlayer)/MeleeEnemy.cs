using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour, IEnemy
{
    [Header("Enemy Stats")]
    public float maxHealth;
    [SerializeField] private float currentHealth;
    public float damage;
    public float attackSpd;
    public float respawnTime = 2f;
    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    [Header("Enemy Components")]
    [SerializeField] private Image healthbarExternal;

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

        if (target == null && player != null)
        {
            target = player.gameObject.transform;
        } else if (target == null && player == null){
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

        Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y); //FIND DIRECTION OF PLAYER
        transform.up = direction; //ROTATES THE ENEMY TO THE PLAYER 

        //attack
    }

    static PlayerHealthSinglePlayer player;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (player == null)
                player = other.gameObject.GetComponent<PlayerHealthSinglePlayer>();

            player.TakeDamage(damage);
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackSpd);
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
    public void TakeDamage(float amount)
    {
        //FIRST CHECK IF THE BASE'S HEALTH IS BELOW 0
        if (currentHealth > 0)
            currentHealth -= amount;
        
        CheckHealth();

        //THEN CHECK FOR ALL THE OTHER DAMAGE CONDITIONS
        _damageTaken += amount;

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

        //IDEALLY, we move this to the interface
        SPGameManager.Instance.enemiesKilled++;
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
