using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    //WANT THERE TO BE A MULTITUDE OF ABILITIES THAT ARE ABLE TO BE ADDED
    public List<Ability> abilities = new List<Ability>();
    float cooldownTime;
    float activeTime;

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

    int indx;
    private void Update()
    {
        //DIFFERENT ABILITY STATES
        switch(state){
            //READY
            case AbilityState.ready:
                //IF THE DEBUG KEY IS PRESSED, ACTIVATE THE ABILITY
                //if(Input.GetKeyDown(debugKeys[0]) || Input.GetKeyDown(debugKeys[1]))//USE THIS IF EVENTS ARE BREAKING THE GAME
                if (debugKeys.Contains(e.keyCode)) //COMMENT THIS OUT IF EVENTS ARE BREAKING THE GAME
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

                    //void startAbility()
                    //{
                    //    abilities[indx].Activate(this.gameObject); //FOR NOW, JUST ACTIVATE THE FIRST ABILITY
                    //}

                    abilities[indx].Activate(this.gameObject); //FOR NOW, JUST ACTIVATE THE FIRST ABILITY
                    state = AbilityState.active; //SET THE ABILITY TO READY
                    activeTime = abilities[indx].activeTime; //SET THE ACTIVE TIME TO THE ABILITY'S ACTIVE TIME AND START THE 
                    
                    Debug.Log("activated");
                }
            break;
            
            //ACTIVE
            case AbilityState.active:
                if(activeTime > 0){ //WHILE THE ABILITY IS ACTIVE
                    activeTime -= Time.deltaTime; //SUBTRACT TIME UNTIL IT HITS 0
                }
                else{
                    abilities[indx].BeginCooldown(this.gameObject);
                    state = AbilityState.cooldown; //AND THEN PUT THE ABILITY ON COOLDOWN
                    cooldownTime = abilities[indx].cooldownTime; //SET THE COOLDOWN TIME TO THE ABILITY'S COOLDOWN TIME AND THEN START THE COOLDOWN TIMER
                }
            break;

            //COOLDOWN
            case AbilityState.cooldown:
                if(cooldownTime > 0){ //WHILE THE ABILITY IS ON COOLDOWN
                    cooldownTime -= Time.deltaTime; //SUBTRACT TIME FROM THE COOLDOWN TIMER UNTIL THE ABILITY IS READY AGAIN

                    Debug.Log("on cooldown");
                }
                else{
                    state = AbilityState.ready; //AND THEN SET THE ABILITY TO READY

                    Debug.Log("ability ready to use");
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

}
