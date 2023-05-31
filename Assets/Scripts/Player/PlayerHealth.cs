using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    public const int maxHealth = 100;

    [SyncVar(hook = "OnChangedHealth")]
    public int currentHealth = maxHealth;

    private Slider healthbarInternal;
    [SerializeField] private Image healthbarExternal;

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
            currentHealth = 0;
            Debug.Log("DEAD");
        }
    }

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
}
