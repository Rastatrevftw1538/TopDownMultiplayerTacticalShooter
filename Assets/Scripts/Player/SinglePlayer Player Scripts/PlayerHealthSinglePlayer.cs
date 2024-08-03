using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerScriptSinglePlayer;

public class PlayerHealthSinglePlayer : Singleton<PlayerHealthSinglePlayer> {

    public const float maxHealth = 100;
    public float iFrames;

    public float currentHealth = maxHealth;
    public float respawnTime;

    private Slider healthbarInternal;
    [SerializeField] private Image healthbarExternal;

    private bool hasSentEvent = false;
    private bool isAlive = true;
    private bool isRespawning = false;
    private bool canHit = true;
    private Transform[] spawnPointList;
    private Animator Anim; //move to statemachine later
    private SpriteRenderer[] sprites;
    public Color flashColor = Color.red;
    public float flashTime = 0.25f;

    public bool checkIfAlive
    {
        get { return isAlive; }
    }
    public float GetHealth()
    {
        return currentHealth;
    }

    private IEnumerator DamageFlash()
    {
        SetFlashColor(flashColor);
        float currentFlashAmt = 0f;
        float elapsedTime = 0f;

        while(elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmt = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));

            yield return new WaitForSeconds(0.5f);

            SetFlashColor(Color.white);
        }
    }

    private void SetFlashColor(Color color)
    {
        foreach(SpriteRenderer spriteRenderer in sprites)
        {
            if(spriteRenderer.gameObject.name != "Pointer")
            spriteRenderer.color = color;
        }
    }

    private void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        healthbarInternal = GetComponentInChildren<Slider>();
        Anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(float amount)
    {
        if (!canHit) return;
        //CHECK IF THE DAMAGE PASSED IN WAS NEGATIVE, IF IT WAS, THIS FUNCTION WILL ADD HEALTH INSTEAD
        amount = 
            amount > 0 ? amount : amount * -1;

        StartCoroutine(nameof(DamageFlash));

        if(currentHealth > 0)
            currentHealth -= amount;

        healthbarExternal.fillAmount = currentHealth / maxHealth;

        checkHealth();
        canHit = false;
        Invoke(nameof(SetCanHitTrue), iFrames);

        //animation
        StartCoroutine(nameof(GotHitAnim));
    }
    
    private void SetCanHitTrue()
    {
        canHit = true;
    }

    private bool CanHit()
    {
        return canHit;
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
            healthbarExternal.fillAmount = currentHealth / maxHealth;
            //Debug.Log("Health Changed - Bar: " + healthbarExternal.fillAmount + " Health: " + currentHealth);
        }

        if (!hasSentEvent)
        {
            checkHealth();
            hasSentEvent = true;
        }
    }
    /*
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
    void RpcDie()
    {
        // Stop player movement
        this.GetComponent<PlayerScriptSinglePlayer>().setCanMove(false);
        this.GetComponent<WeaponSinglePlayer>().enabled = false;
        isAlive = false;
        // Teleport player back to spawn
        // Restore health after 3 seconds
        //StartCoroutine(RestoreHealth());

        //RAISE THE EVENT SO THE GAMEMODE MANAGER CAN TRACK THIS MESSAGE
        PlayerDied playerDied = new PlayerDied();
        playerDied.playerThatDied = this.gameObject;

        ///FOR NOW RYAN DOOOO NOTT FORGETT
        //if (!isRespawning)
            //EvtSystem.EventDispatcher.Raise<PlayerDied>(playerDied);
            //Respawn(respawnTime);

        UIManager.Instance.ShowDefeat();
    }
    private void RestoreHealth()
    {
        currentHealth = maxHealth;
        isAlive = true;
        isRespawning = false;
        GetComponent<WeaponSinglePlayer>().enabled = true;
        GetComponent<PlayerScriptSinglePlayer>().setCanMove(true);

        //TO ENSURE THE SAME EVENTS DON'T GET RAISED MORE THAN ONCE
        hasSentEvent = false;

       // Debug.LogError("<color=yellow>RESPAWNED SUCCESSFULLY.</color>");
    }

    public void Respawn(float respawnTime)
    {
        //Debug.LogError(this.name+"DIED!");
        /*TODO: Replace with game starting points*/
        /*foreach (Transform spawnPoints in spawnPointList) {
                Debug.LogError("got respawn point");
                if (spawnPoints.CompareTag(this.GetComponent<PlayerScriptSinglePlayer>().PlayerTeam.ToString())) {
                    transform.position = spawnPoints.position;
                }
        }*/

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
    
    private IEnumerator GotHitAnim()
    {
        Anim.SetBool("GotHit", true);
        yield return new WaitForSeconds(iFrames);
        Anim.SetBool("GotHit", false);
        Debug.Log("stopped getting hit");
    }
}
