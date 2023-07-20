using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Base : NetworkBehaviour
{
    [Header("Base Stats")]
    public int maxHealth = 1000;
    public const int maxDamageInRound = 50;
    [SyncVar] private int currentHealth;

    //BASE COMPONENTS
    [SerializeField] private Image healthbarExternal;

    [Header("Base Status")]
    private bool isAlive = true;
    [SyncVar] public bool canHit = false;

    private int _damageTaken = 0;

    public bool checkIfAlive
    {
        get { return isAlive; }
    }

    public int GetHealth()
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
        maxHealth = GameObject.Find("GameModeManager").GetComponent<HeistGameManager>().baseHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_damageTaken < maxDamageInRound)
        {
        _damageTaken += amount;
        currentHealth -= amount;
        }
        else{
            //CHANGE BASE STATE, RESET _damageTaken
            StartPhase(false);
        }
    }
    private void Update()
    {
            healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
            //Debug.Log("found healthbar");

        /*if (healthbarExternal != null)
        {
            Debug.Log("Health: " + (float)currentHealth);
            healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
            Debug.Log("Health Changed - Bar: " + healthbarExternal.fillAmount + " Health: " + currentHealth);
        }*/
        //Debug.Log("<color=red>Base Health: " + currentHealth + "</color>");
    }

    public void StartPhase(bool isVulnerable)
    {
        //RAISE 'ChangeBaseState' TO BE RECIEVED BY 'HeistGameManager.cs' SO THEY CAN REACT ACCORDINGLY
        this.canHit = isVulnerable;
        ChangeBaseState changeBaseState = new ChangeBaseState();
        changeBaseState.isBaseVulnerable = isVulnerable;

        EvtSystem.EventDispatcher.Raise<ChangeBaseState>(changeBaseState);
        _damageTaken = 0;
    }
}
