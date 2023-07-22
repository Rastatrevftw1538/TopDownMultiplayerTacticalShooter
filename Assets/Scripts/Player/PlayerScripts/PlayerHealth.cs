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
        if(currentHealth> 0)
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

         EvtSystem.EventDispatcher.Raise<PlayerDied>(playerDied);
    }
    [ClientRpc]
    public void Respawn(float respawnTime)
    {
        Debug.Log(this.name+"DIED!");
        foreach (Transform spawnPoint in NetworkManager.startPositions) {
                if (spawnPoint.CompareTag(this.GetComponent<PlayerScript>().PlayerTeam.ToString())) {
                    transform.position = spawnPoint.position;
                }
            }
        //TO ENSURE THE SAME EVENTS DON'T GET RAISED MORE THAN ONCE
        IEnumerator setBool(float time) {
            yield return new WaitForSeconds(time);
            hasSentEvent = false;
        }
        StartCoroutine(setBool(respawnTime));
        
        //ACTUALLY RESTORE HEALTH TO THE PLAYER
        StartCoroutine(RestoreHealth(respawnTime));
    }
    
    IEnumerator RestoreHealth(float time)
    {
        yield return new WaitForSeconds(time);
        currentHealth = maxHealth;
        isAlive = true;
        GetComponent<Weapon>().enabled = true;
        GetComponent<PlayerScript>().setCanMove(true);

        Debug.Log("<color=yellow>RESPAWNED SUCCESSFULLY</color>");
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
