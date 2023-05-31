using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHealth : NetworkBehaviour 
{

    public const int maxHealth = 100;
    [SyncVar(hook ="OnChangedHealth")]public int currentHealth = maxHealth;
    private Slider[] sliderArray;

    public int GetHealth()
    {
        return this.currentHealth;
    }

    private void Awake()
    {
        sliderArray = GetComponentsInChildren<Slider>();
    }

    public void TakeDamage(int amount){
        if(!isServer){
            return;
        }
        currentHealth -= amount;
        if(currentHealth <= 0){
            currentHealth = 0;
            Debug.Log("DEAD");
        }
        
    }
    void OnChangedHealth(int oldHealth, int health){
        foreach (var slider in sliderArray)
        {
            slider.value = currentHealth;
        }
    }
}

