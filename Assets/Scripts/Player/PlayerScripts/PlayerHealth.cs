using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using static PlayerScript;

public class PlayerHealth : NetworkBehaviour
{
    public const int maxHealth = 100;

    [SyncVar]public int currentHealth = maxHealth;

    private Slider healthbarInternal;
    [SerializeField] private Image healthbarExternal;

    private bool hasSentEvent = false;
    private bool isAlive = true;
    private bool isRespawning = false;
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
        healthbarInternal = GetComponentInChildren<Slider>();

    }

    public void TakeDamage(int amount)
    {
        //CHECK IF THE DAMAGE PASSED IN WAS NEGATIVE, IF IT WAS, THIS FUNCTION WILL ADD HEALTH INSTEAD
        amount = 
            amount > 0 ? amount : amount * -1;

        if(currentHealth > 0)
            currentHealth -= amount;

        checkHealth();
    }
    private void Update()
    {
        if (healthbarInternal != null)
        {
            healthbarInternal.value = currentHealth;
        }

        if (healthbarExternal != null)
        {
            //Debug.Log("Health: " + (float)currentHealth);
            healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
            //Debug.Log("Health Changed - Bar: " + healthbarExternal.fillAmount + " Health: " + currentHealth);
        }

        if (!hasSentEvent)
        {
            checkHealth();
            hasSentEvent = true;
        }
    }
    /*
    [ClientRpc]
    void OnChangedHealth(int oldHealth, int health)
    {
        if (healthbarInternal != null)
        {
            healthbarInternal.value = health;
        }

        if (healthbarExternal != null)
        {
            Debug.Log("Health: "+(float)health);
            healthbarExternal.fillAmount = (float)health/(float)maxHealth;
            Debug.Log("Health Changed - Bar: "+healthbarExternal.fillAmount+" Health: "+currentHealth);
        }
        //Debug.Log("Health Changed");
    }
    */
    [ClientRpc]
    void RpcDie()
    {
        // Stop player movement
        this.GetComponent<PlayerScript>().setCanMove(false);
        this.GetComponent<Weapon>().enabled = false;
        isAlive = false;
        // Teleport player back to spawn
        // Restore health after 3 seconds
        //StartCoroutine(RestoreHealth());

        //RAISE THE EVENT SO THE 'HeistGameManager.cs' CAN TRACK THIS MESSAGE
         PlayerDied playerDied = new PlayerDied();
         playerDied.playerThatDied = this.gameObject;


         if(!isRespawning)
            EvtSystem.EventDispatcher.Raise<PlayerDied>(playerDied);
    }
    private void RestoreHealth()
    {
        currentHealth = maxHealth;
        isAlive = true;
        isRespawning = false;
        GetComponent<Weapon>().enabled = true;
        GetComponent<PlayerScript>().setCanMove(true);

        //TO ENSURE THE SAME EVENTS DON'T GET RAISED MORE THAN ONCE
        hasSentEvent = false;

        Debug.LogError("<color=yellow>RESPAWNED SUCCESSFULLY.</color>");
    }

    [ClientRpc]
    public void Respawn(float respawnTime)
    {
        Debug.LogError(this.name+"DIED!");
        foreach (Transform spawnPoint in NetworkManager.startPositions) {
                Debug.LogError("got respawn point");
                if (spawnPoint.CompareTag(this.GetComponent<PlayerScript>().PlayerTeam.ToString())) {
                    transform.position = spawnPoint.position;
                }
            }

        isRespawning = true;
        //ACTUALLY RESTORE HEALTH TO THE PLAYER
        Invoke(nameof(RestoreHealth), respawnTime);
    }

    private void checkHealth()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RpcDie();
        }
    }
}
