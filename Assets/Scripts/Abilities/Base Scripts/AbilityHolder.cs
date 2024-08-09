using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityHolder : NetworkBehaviour
{
    //WANT THERE TO BE A MULTITUDE OF ABILITIES THAT ARE ABLE TO BE ADDED
    [SerializeField] private StatusEffectData statusEffectData;
    public List<Ability> abilities = new List<Ability>();
    float cooldownTime;
    float activeTime;
    bool hasCorrectTag = false;

    private PlayerScript playerScript;

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }

    //DETECT KEYCODE PRESSED
    private Event e;
    void OnGUI()
    {
        e = Event.current;
        if (e.isKey)
        {
            //Debug.Log("Detected key code: " + e.keyCode);
        }
    }

    //INSTANTLY SET ALL ABILITIES TO READY
    AbilityState state = AbilityState.ready;

    //WILL BE REPLACED WITH IF THE BUTTON ON THE UI IS CLICKED OR NOT
    public List<KeyCode> debugKeys = new List<KeyCode>();

    private void Start()
    {
        if (!isLocalPlayer) return;

        playerScript = gameObject.transform.parent.gameObject.GetComponent<PlayerScript>();
    }

    int indx;

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer) return;
        //Debug.LogError("PLAYER TEAM: " + playerScript.PlayerTeam);

        //DIFFERENT ABILITY STATES
        switch (state){
            //READY
            case AbilityState.ready:
                //IF THE DEBUG KEY IS PRESSED, ACTIVATE THE ABILITY
                //if(Input.GetKeyDown(debugKeys[0]) || Input.GetKeyDown(debugKeys[1]))//USE THIS IF EVENTS ARE BREAKING THE GAME
                if (debugKeys.Contains(e.keyCode)) //COMMENT THIS OUT IF EVENTS ARE BREAKING THE GAME
                {
                    indx = debugKeys.FindIndex(x => x == e.keyCode); //COMMENT THIS OUT IF EVENTS ARE BREAKING THE GAME
                    statusEffectData = abilities[indx].statusEffectData;
                    //USE THIS IF EVENTS ARE BREAKING THE GAME
                    /*if (Input.GetKeyDown(debugKeys[0]))
                    {
                        indx = 0;
                    }
                    else
                    {
                        indx = 1;
                    }*/

                    //IF THE ABILITY HAS A DELAY, INVOKE THE ABILITY WITH THE DELAY TIMER

                    if (abilities[indx].hasDelay)
                    {
                        Invoke(nameof(startAbility), abilities[indx].delayTime);
                    }
                    else
                    {
                        startAbility();
                    }
                    
                    Debug.LogWarning("<color=orange>Ability: " + abilities[indx].abilityName + " has been activated. </color>");
                }
            break;
            
            //ACTIVE
            case AbilityState.active:
                if(activeTime > 0){ //WHILE THE ABILITY IS ACTIVE
                    activeTime -= Time.deltaTime; //SUBTRACT TIME UNTIL IT HITS 0
                }
                else{
                    abilities[indx].whichAbility.BeginCooldown(playerScript.gameObject);
                    state = AbilityState.cooldown; //AND THEN PUT THE ABILITY ON COOLDOWN
                    cooldownTime = abilities[indx].cooldownTime; //SET THE COOLDOWN TIME TO THE ABILITY'S COOLDOWN TIME AND THEN START THE COOLDOWN TIMER
                }
            break;

            //COOLDOWN
            case AbilityState.cooldown:
                if(cooldownTime > 0){ //WHILE THE ABILITY IS ON COOLDOWN
                    cooldownTime -= Time.deltaTime; //SUBTRACT TIME FROM THE COOLDOWN TIMER UNTIL THE ABILITY IS READY AGAIN

                    Debug.LogWarning("<color=orange>Ability: " + abilities[indx].abilityName + " is on cooldown. </color>");
                }
                else{
                    state = AbilityState.ready; //AND THEN SET THE ABILITY TO READY

                    Debug.LogWarning("<color=orange>Ability: " + abilities[indx].abilityName + " is now ready to use. </color>");
                }
            break;  

        }
    }

    #region Search Functions
    int search(KeyCode[] arr, int N, KeyCode x)
    {
        for (int i = 0; i < N; i++)
            if (arr[i] == x)
                return i;

        return -1;
    }
    #endregion

    #region Remote Call Functions
    void startAbility()
    {
        abilities[indx].whichAbility.Activate(playerScript.gameObject); //ACTIVATE THE ABILITY OF INDEX

        state = AbilityState.active; //SET THE ABILITY TO READY
        activeTime = abilities[indx].activeTime; //SET THE ACTIVE TIME TO THE ABILITY'S ACTIVE TIME AND START THE TIMER
    }
    #endregion
    #region Collision Functions
    private void OnTriggerEnter2D(Collider2D other)
    {
        //IEffectable effectable = other.GetComponent<IEffectable>();
        //PlayerScript effectable = other.gameObject.GetComponent<PlayerScript>();
        /*if(effectable != null)
        {
            PlayerScript target = other.GetComponent<PlayerScript>();
            if (target != null && target != playerScript)
            {
                Debug.LogError(target.playerTeam);
                if (abilities[indx].playerEffects == PlayerEffects.ENEMY)
                {
                    if (playerScript.playerTeam != target.playerTeam)
                    {
                        Debug.LogError("did a ENEMY THING");
                        hasCorrectTag = true;
                    }
                }
                else if (abilities[indx].playerEffects == PlayerEffects.TEAM)
                {
                    if (playerScript.playerTeam == target.playerTeam)
                    {
                        Debug.LogError("did a TEAM THING");
                        hasCorrectTag = true;
                    }
                }
            }
        }*/
        //playerScript = this.gameObject.gameObject.GetComponent<PlayerScript>();

        // if (effectable != null)
        //{
        if (other.gameObject.GetComponent<IEffectable>() == null)
            return;

        IEffectable target = other.gameObject.GetComponent<IEffectable>();

        if (CheckCorrectTag(abilities[indx].playerEffects, other))
            target.ApplyEffect(abilities[indx].statusEffectData);

        //Debug.LogError("Applied from PLAYER TEAM: " + playerScript.PlayerTeam);
        //}
    }

    private bool CheckCorrectTag(PlayerEffects playerEffects, Collider2D other)
    {
        PlayerScript target = other.gameObject.GetComponent<PlayerScript>();

        switch (playerEffects)
        {
            case PlayerEffects.ENEMY: //IF ENEMY IS THE TARGET
            if (playerScript.playerTeam != target.playerTeam)
            {
                Debug.LogError("Used ENEMY");
                return true;
            }
            break;

            case PlayerEffects.TEAM: //IF TEAM IS THE TARGET
            if (playerScript.playerTeam == target.playerTeam)
            {
                Debug.LogError("Used TEAM");
                return true;
            }
            break;

            case PlayerEffects.PLAYER:
            if (playerScript.Equals(other.GetComponent<PlayerScript>()))
            {
                return true;
            }
            break;

            default:
                // hasCorrect = this.gameObject.GetComponent<Collider2D>() == other;
                Debug.LogError("Used SELF?");
            return false;
        }

        /*if(playerEffects == PlayerEffects.ENEMY)
        {
            if (playerScript.playerTeam != target.playerTeam)
            {
                Debug.LogError("did a ENEMY THING");
                   return true;
            }
            else
                return false;
        }
        else if (playerEffects == PlayerEffects.TEAM)
        {
            if (playerScript.playerTeam == target.playerTeam)
            {
                Debug.LogError("did a TEAM THING");
                return true;
            }
            else
                return false;
        }*/

        Debug.LogError("got to the end??");
        return false;
    }
    #endregion
}
