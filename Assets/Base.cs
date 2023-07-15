using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Base : NetworkBehaviour
{
    [Header("Base Stats")]
    public const int maxHealth = 100;
    public const int maxDamageInRound = 50;
    [SyncVar] public int currentHealth = maxHealth;

    //BASE COMPONENTS
    [SerializeField] private Slider healthbarInternal;
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

    private void Awake()
    {
        //healthbarInternal = GetComponentInChildren<Slider>();
    }

    public void TakeDamage(int amount)
    {
        _damageTaken += amount;
        currentHealth -= amount;
        if (maxDamageInRound <= _damageTaken)
        {
            //CHANGE BASE STATE, RESET _damageTaken
            StartPhase(false);
        }
    }
    private void Update()
    {
        if (healthbarInternal != null)
        {
            healthbarInternal.value = currentHealth;
            healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
            //Debug.Log("found healthbar");
        }

        /*if (healthbarExternal != null)
        {
            Debug.Log("Health: " + (float)currentHealth);
            healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
            Debug.Log("Health Changed - Bar: " + healthbarExternal.fillAmount + " Health: " + currentHealth);
        }*/
        //Debug.Log("<color=red>Base Health: " + currentHealth + "</color>");
    }

    [ClientRpc]
    void RpcRespawnPlayers()
    {

       // StartCoroutine(RestoreHealth());

    }

    void StartPhase(bool isVulnerable)
    {
        //RAISE 'ChangeBaseState' TO BE RECIEVED BY 'HeistGameManager.cs' SO THEY CAN REACT ACCORDINGLY
        this.canHit = isVulnerable;
        ChangeBaseState changeBaseState = new ChangeBaseState();
        changeBaseState.isBaseVulnerable = isVulnerable;

        EvtSystem.EventDispatcher.Raise<ChangeBaseState>(changeBaseState);
        _damageTaken = 0;
    }
}
