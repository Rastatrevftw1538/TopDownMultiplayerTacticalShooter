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
    [SerializeField] public int currentHealth;
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
        maxHealth = GameObject.Find("GameModeManager").GetComponent<ChaseGameManager>().baseHealth;
        maxDamageInRound = maxHealth / 2;
    }

    private void Start()
    {
        /*
        if(this.tag == "Red")
        {
            team = PlayerScript.Team.Red;
        }
        else
        {
            team = PlayerScript.Team.Blue;
        }
        */
        //IF THIS RECIEVES A MESSAGE THAT A TEAM HAS BEEN RESPAWNED, END THE VULNERABILITY PHASE
        //EvtSystem.EventDispatcher.AddListener<ChangeBaseState>(StartPhase);
        //EvtSystem.EventDispatcher.AddListener<ApplyStatusEffects>(ApplyStatusEffect);
        currentHealth = 100;
    }

    public void TakeDamage(int amount)
    {
        //FIRST CHECK IF THE BASE'S HEALTH IS BELOW 0
        if (currentHealth <= 0)
        {
            RpcDie();
        }

        //THEN CHECK FOR ALL THE OTHER DAMAGE CONDITIONS
        _damageTaken += amount;
        currentHealth -= amount;
        print("<color=red> OO Ouch I have taken: " + _damageTaken + " ouch! </color>");
 
        /*else if (_damageTaken >= maxDamageInRound)
        {
            //CHANGE BASE STATE, RESET _damageTaken
            IsVulnerable(false);
            Debug.LogError("<color=red> Okay thats enough! >:( </color>");
            Debug.LogError("<color=red> Base Health: " + currentHealth + "</color>");

            //REPLACE "BASE IS VULNERABLE MESSAGE"
            ReplaceUI replaceUI = new ReplaceUI();
            replaceUI.replacementMessage = "MAXIMUM DAMAGE!!";
            EvtSystem.EventDispatcher.Raise<ReplaceUI>(replaceUI);
        }*/
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

        if (currentHealth <= 0)
            RpcDie();
    }

    /*public void IsVulnerable(bool isVulnerable)
    {
        Debug.LogWarning("<color=purple>IS BASE VULNERABLE?: " + isVulnerable + "</color>");
        this.canHit = isVulnerable;

        //MAXIMUM DAMAGE THE BASE CAN TAKE IN ONE PHASE
        _damageTaken = 0;
    }

    public void StartPhase(ChangeBaseState evtData)
    {
        //if (evtData.team == this.team)
        IsVulnerable(evtData.isBaseVulnerable);

        lastBaseEvent = evtData;
    }*/

    public void ApplyStatusEffect(ApplyStatusEffects evtData)
    {
        //Debug.LogWarning("<color=purple>ENDING BASE PHASE</color>");
    }

    private void EndVulnerability()
    {
        this.canHit = false;
        _damageTaken = 0;
    }


    private bool isRespawning;
    //private bool hasSentEvent;
    private void RestoreHealth()
    {
        currentHealth = maxHealth;
        isAlive = true;
        isRespawning = false;
        canHit = true;

        //TO ENSURE THE SAME EVENTS DON'T GET RAISED MORE THAN ONCE
        //hasSentEvent = false;

        Debug.LogError("<color=yellow>BASE: " + this.name + " RESPAWNED SUCCESSFULLY.</color>");
    }

    [ClientRpc]
    public void Respawn(float respawnTime)
    {
        Debug.LogError(this.name + "DIED!");
        isRespawning = true;
        canHit = false;
        //ACTUALLY RESTORE HEALTH TO THE PLAYER
        Invoke(nameof(RestoreHealth), respawnTime);
    }

    [ClientRpc]
    void RpcDie()
    {
        isAlive = false;

        //RAISE BASE DESTROYED EVENT, TO BE RECIEVED BY 'ChaseGameManager.cs'
        ChangeBaseState baseDestroyed = new ChangeBaseState();
        baseDestroyed.thisBaseEffects = this;
        EvtSystem.EventDispatcher.Raise<ChangeBaseState>(baseDestroyed);
        //sentBaseDestroyedEvent = true;
        Debug.LogError("RPC DIED BASEFFECTS");
        //STATUS EFFECT EVENTS ARE HANDLED THRU 'Weapon.cs'
        Respawn(respawnTime);
    }
}

[CustomEditor(typeof(BaseEffects))]
public class BaseEffectsEditor : Editor
{

}
