using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingDummy : MonoBehaviour
{
    public float maxHealth;
    [SerializeField] private float currentHealth;
    public float respawnTime = 2f;
    [SerializeField] private Image healthbarExternal;

    private float _damageTaken = 0;
    public bool canHit = true;
    private bool isAlive = true;

    public GameObject hitDisplay;
    public float hitDisplaySeconds;

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

    private void Awake()
    {
        //healthbarInternal = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
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

        //DISPLAY DAMAGE TAKEN
        //DisplayUI displayUI = new DisplayUI();
        //displayUI.colorOfText = Color.white;
        //displayUI.textToDisplay = "-" + amount;

        //EvtSystem.EventDispatcher.Raise<DisplayUI>(displayUI);
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    public List<GameObject> hitDisplays = new List<GameObject>();
    private void DisplayHit(float amount)
    {
        //INSTANTIATE A HITDISPLAY
        GameObject currentDisplay = Instantiate(hitDisplay, gameObject.transform);
        hitDisplays.Add(currentDisplay);

       // currentDisplay.transform.position.y += 0.10f;

    }

    private void Update()
    {
        //healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;


    }

    private void EndVulnerability()
    {
        this.canHit = false;
        _damageTaken = 0;
    }


    private bool isRespawning;
    private void RestoreHealth()
    {
        currentHealth = maxHealth;
        isAlive = true;
        isRespawning = false;
        canHit = true;
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
        //Debug.LogError("<color=yellow>BASE: " + this.name + " RESPAWNED SUCCESSFULLY.</color>");
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
        Respawn(respawnTime);
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
