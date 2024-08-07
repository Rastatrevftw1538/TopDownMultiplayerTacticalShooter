using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerScriptSinglePlayer;

[RequireComponent(typeof(PlayerScriptSinglePlayer))]
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

        //subtract health
        currentHealth -= amount;
        canHit = false;

        //did you die?
        CheckHealth();
        //animation
        StartCoroutine(nameof(GotHitAnim));
        //dmg flash
        StartCoroutine(nameof(DamageFlash));
    }


    private IEnumerator DamageFlash()
    {
        //iframes
        Invoke(nameof(SetCanHitTrue), iFrames);
        SetFlashColor(flashColor);
        float currentFlashAmt = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmt = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));

            yield return new WaitForSeconds(0.5f);

            SetFlashColor(Color.white);
        }
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
    }

    WeaponSinglePlayer weapon;
    PlayerScriptSinglePlayer player;

    void SetPlayerWep(bool set)
    {
        if (!player) player = this.GetComponent<PlayerScriptSinglePlayer>();
        player.setCanMove(set);
        if (!weapon) weapon = this.GetComponent<WeaponSinglePlayer>();
        weapon.enabled = weapon.enabled = false; ;
    }
    void RpcDie()
    {
        // Stop player movement
        SetPlayerWep(false);
        isAlive = false;

        if(UIManager.Instance) UIManager.Instance.ShowDefeat();
    }
    private void RestoreHealth()
    {
        currentHealth = maxHealth;
        isAlive = true;
        isRespawning = false;

        SetPlayerWep(true);
    }

    public void Respawn(float respawnTime)
    {
        isRespawning = true;
        //ACTUALLY RESTORE HEALTH TO THE PLAYER
        Invoke(nameof(RestoreHealth), respawnTime);
    }

    private void CheckHealth()
    {
        healthbarExternal.fillAmount = currentHealth / maxHealth;
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
