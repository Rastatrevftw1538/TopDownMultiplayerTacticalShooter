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

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            RpcDie();

            PlayerDied();
        }
    }
    private void Update()
    {
        if (healthbarInternal != null)
        {
            healthbarInternal.value = currentHealth;
        }

        if (healthbarExternal != null)
        {
           // Debug.Log("Health: " + (float)currentHealth);
            healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
            //Debug.Log("Health Changed - Bar: " + healthbarExternal.fillAmount + " Health: " + currentHealth);
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
            GetComponent<PlayerScript>().setCanMove(false);
            GetComponent<Weapon>().enabled = false;

            // Teleport player back to spawn
            foreach (Transform spawnPoint in NetworkManager.startPositions) {
                if (spawnPoint.tag.Equals(this.GetComponent<PlayerScript>().PlayerTeam)) {
                    transform.position = spawnPoint.position;
                }
            }
            
            // Restore health after 3 seconds
            //StartCoroutine(RestoreHealth());
        
    }

    [Command (requiresAuthority = false)]
    private void PlayerDied()
    {
        PlayerDied evtPlayerDied = new PlayerDied();
        evtPlayerDied.playerThatDied = this.gameObject;

        EvtSystem.EventDispatcher.Raise<PlayerDied>(evtPlayerDied);
    }
    
    public void RestoreHealth()
    {
        //yield return new WaitForSeconds(5f);
        currentHealth = maxHealth;
        GetComponent<Weapon>().enabled = true;
        GetComponent<PlayerScript>().setCanMove(true);
    }

    public void Respawn(float respawnTime)
    {
            Invoke("RestoreHealth", respawnTime);
    }
}
