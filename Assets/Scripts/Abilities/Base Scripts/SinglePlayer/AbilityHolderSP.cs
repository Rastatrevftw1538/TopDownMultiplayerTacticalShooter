using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityHolderSP : MonoBehaviour
{
    //WANT THERE TO BE A MULTITUDE OF ABILITIES THAT ARE ABLE TO BE ADDED
    [SerializeField] private StatusEffectData statusEffectData;
    public List<Ability> abilities = new List<Ability>();
    public GameObject abilityUIHolder;
    private Image abilityUIIcon;
    private Image abilityCooldownUI;
    float cooldownTime;
    float activeTime;
    bool hasCorrectTag = false;

    private PlayerScriptSinglePlayer playerScript;

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
        playerScript = gameObject.transform.parent.gameObject.GetComponent<PlayerScriptSinglePlayer>();
        abilityUIIcon = abilityUIHolder.transform.GetChild(0).GetComponent<Image>();
        abilityCooldownUI = abilityUIHolder.transform.GetChild(1).GetComponent<Image>();

        if (abilities[indx].abilityIcon != null)
            abilityUIIcon.sprite = abilities[indx].whichAbility.abilityIcon;

    }

    int indx;
    private void Update()
    {
        //Debug.LogError("PLAYER TEAM: " + playerScript.PlayerTeam);

        //DIFFERENT ABILITY STATES
        switch (state){
            //READY
            case AbilityState.ready:
                //IF THE DEBUG KEY IS PRESSED, ACTIVATE THE ABILITY
                //if(Input.GetKeyDown(debugKeys[0]) || Input.GetKeyDown(debugKeys[1]))//USE THIS IF EVENTS ARE BREAKING THE GAME
                if (e != null && debugKeys.Contains(e.keyCode)) //COMMENT THIS OUT IF EVENTS ARE BREAKING THE GAME
                {
                    indx = debugKeys.FindIndex(x => x == e.keyCode); //COMMENT THIS OUT IF EVENTS ARE BREAKING THE GAME
                    //USE THIS IF EVENTS ARE BREAKING THE GAME
                    /*if (Input.GetKeyDown(debugKeys[0]))
                    {
                        indx = 0;
                    }
                    else
                    {
                        indx = 1;
                    }*/
                    statusEffectData = abilities[indx].statusEffectData;

                    //IF THE ABILITY HAS A DELAY, INVOKE THE ABILITY WITH THE DELAY TIMER

                    if (abilities[indx].hasDelay)
                    {
                        Invoke(nameof(StartAbility), abilities[indx].delayTime);
                    }
                    else
                    {
                        StartAbility();
                    }
                    
                    //Debug.LogWarning("<color=orange>Ability: " + abilities[indx].abilityName + " has been activated. </color>");
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
                    abilityCooldownUI.fillAmount -= abilities[indx].cooldownTime * Time.deltaTime; //SET THE UI TO MATCH
                    UIManager.Instance.StartCooldownUI(UIManager.CooldownType.Ability, abilities[indx].abilityIcon, abilities[indx].cooldownTime, abilities[indx].name);
                    //Debug.LogWarning("<color=orange>Ability: " + abilities[indx].abilityName + " is on cooldown. </color>");
                }
                else{
                    state = AbilityState.ready; //AND THEN SET THE ABILITY TO READY
                    abilityCooldownUI.fillAmount = 0f; //SET THE UI TO MATCH
                    //Debug.LogWarning("<color=orange>Ability: " + abilities[indx].abilityName + " is now ready to use. </color>");
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
    void StartAbility()
    {
        abilities[indx].whichAbility.Activate(playerScript.gameObject); //ACTIVATE THE ABILITY OF INDEX

        state = AbilityState.active; //SET THE ABILITY TO READY
        activeTime = abilities[indx].activeTime; //SET THE ACTIVE TIME TO THE ABILITY'S ACTIVE TIME AND START THE TIMER
        abilityCooldownUI.fillAmount = 1;
    }

    #endregion
    #region Collision Functions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<IEffectable>() == null)
            return;

        IEffectable target = other.gameObject.GetComponent<IEffectable>();

        if (CheckCorrectTag(abilities[indx].playerEffects, other))
            target.ApplyEffect(abilities[indx].statusEffectData);
    }

    private bool CheckCorrectTag(PlayerEffects playerEffects, Collider2D other)
    {
        PlayerScriptSinglePlayer target = other.gameObject.GetComponent<PlayerScriptSinglePlayer>();

        switch (playerEffects)
        {
            case PlayerEffects.PLAYER:
            if (playerScript.Equals(other.GetComponent<PlayerScriptSinglePlayer>()))
            {
                return true;
            }
            break;

            default:
                // hasCorrect = this.gameObject.GetComponent<Collider2D>() == other;
                Debug.LogError("Used SELF?");
            return false;
        }

        Debug.LogError("got to the end??");
        return false;
    }
    #endregion
}
