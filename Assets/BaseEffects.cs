using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Mirror;

public class BaseEffects : NetworkBehaviour
{
    [Header("Base Stats")]
    public int maxHealth = 1000;
    public int maxDamageInRound;
    [SyncVar] public PlayerScript.Team team;
    [SyncVar, SerializeField] private int currentHealth;
    public StatusEffectData statusEffect;
    public float respawnTime = 2f;

    //BASE COMPONENTS
    [SerializeField] private Image healthbarExternal;

    [Header("Base Status")]
    public bool canHit = true;
    private bool isAlive = true;

    private int _damageTaken = 0;
    private bool sentBaseDestroyedEvent = false;
    private ChangeBaseState lastBaseEvent = null;

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
    }

    private void Start()
    {
        EvtSystem.EventDispatcher.AddListener<WhoBrokeBase>(ApplyStatusEffect);

        maxHealth = ChaseGameManager.instance.baseHealth;
        maxDamageInRound = maxHealth / 2;
    }

    public void TakeDamage(int amount)
    {
        //FIRST CHECK IF THE BASE'S HEALTH IS BELOW 0
        if (currentHealth > 0)
            currentHealth -= amount;

        CheckHealth();

        //THEN CHECK FOR ALL THE OTHER DAMAGE CONDITIONS
        _damageTaken += amount;

        print("<color=red> OO Ouch I have taken: " + _damageTaken + " ouch! </color>");
    }
    private void Update()
    {
        healthbarExternal.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    private PlayerScript.Team teamThatBrokeBaseLast;
    public void ApplyStatusEffect(WhoBrokeBase evtData)
    {
        if (statusEffect != null)
        {
            teamThatBrokeBaseLast = evtData.playerTeam;

            ApplyStatusEffects applyStatusEffects = new ApplyStatusEffects();
            applyStatusEffects.team = evtData.playerTeam;
            applyStatusEffects.statusEffect = statusEffect;

            EvtSystem.EventDispatcher.Raise<ApplyStatusEffects>(applyStatusEffects);
        }
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

        //Debug.LogError("<color=yellow>BASE: " + this.name + " RESPAWNED SUCCESSFULLY.</color>");
    }

    [ClientRpc]
    public void Respawn(float respawnTime)
    {
        Debug.LogError(this.name + " has been destroyed!");

        DisplayUI displayUI     = new DisplayUI();
        displayUI.colorOfText   = Color.yellow;
        displayUI.textToDisplay = 
            "A base has been destroyed! " + statusEffect.Name + " has been applied to the " + teamThatBrokeBaseLast + " team.";

        EvtSystem.EventDispatcher.Raise<DisplayUI>(displayUI);


        isRespawning = true;
        canHit = false;

        //ACTUALLY RESTORE HEALTH TO THE BASE
        Invoke(nameof(RestoreHealth), respawnTime);
    }

    [ClientRpc]
    void RpcDie()
    {
        isAlive = false;
        Respawn(respawnTime);
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RpcDie();
        }
    }
}

[CustomEditor(typeof(BaseEffects))]
public class BaseEffectsEditor : Editor
{

}
