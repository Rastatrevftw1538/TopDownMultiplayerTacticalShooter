using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Base : NetworkBehaviour
{
    [Header("Base Stats")]
    public int maxHealth = 1000;
    public int maxDamageInRound;
    [SyncVar] public PlayerScript.Team team;
    [SyncVar] private int currentHealth;

    //BASE COMPONENTS
    [SerializeField] private Image healthbarExternal;

    [Header("Base Status")]
    [SyncVar] public bool canHit = false;
    private bool isAlive = true;

    private int _damageTaken = 0;

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
        maxHealth = GameObject.Find("GameModeManager").GetComponent<HeistGameManager>().baseHealth;
        maxDamageInRound = maxHealth/2;
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
        EvtSystem.EventDispatcher.AddListener<ChangeBaseState>(StartPhase);
        EvtSystem.EventDispatcher.AddListener<StartTeamRespawn>(EndPhase);
    }

    public void TakeDamage(int amount)
    {
        if (_damageTaken < maxDamageInRound && currentHealth > 0)
        {
            
            _damageTaken += amount;
            currentHealth -= amount;
            print("<color=red> OO Ouch I have taken: "+_damageTaken+" ouch! </color>");
        }
        else if (_damageTaken >= maxDamageInRound){
            //CHANGE BASE STATE, RESET _damageTaken
            GameObject.Find("GameModeManager").GetComponent<HeistGameManager>().ClearDead();
            IsVulnerable(false);
            print("<color=red> Okay thats enough! >:( </color>");
        }
        else if(currentHealth <= 0){
            currentHealth = 0;
        }
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
    }

    public void IsVulnerable(bool isVulnerable)
    {
        Debug.LogWarning("<color=purple>IS BASE VULNERABLE?: " + isVulnerable + "</color>");
        this.canHit = isVulnerable;

        //MAXIMUM DAMAGE THE BASE CAN TAKE IN ONE PHASE
        _damageTaken = 0;
    }

    public void StartPhase(ChangeBaseState evtData)
    {
        if(evtData.team == this.team)
            IsVulnerable(evtData.isBaseVulnerable);
    }

    public void EndPhase(StartTeamRespawn evtData)
    {
        Debug.LogWarning("<color=purple>ENDING BASE PHASE</color>");
        StartCoroutine(EndVulnerability(evtData.respawnTime));
    }
    IEnumerator EndVulnerability(float time)
    {
        yield return new WaitForSeconds(time);
        this.canHit = false;
        _damageTaken = 0;
    }
}
