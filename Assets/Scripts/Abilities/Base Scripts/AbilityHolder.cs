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

    AbilityState state = AbilityState.ready;

    public KeyCode debugKey;

    private void Update()
    {
        //DIFFERENT ABILITY STATES
        switch(state){
            //READY
            case AbilityState.ready:
                //IF THE DEBUG KEY IS PRESSED, ACTIVATE THE ABILITY
                if (Input.GetKeyDown(debugKey))
                {
                    abilities[0].Activate(this.gameObject); //FOR NOW, JUST ACTIVATE THE FIRST ABILITY
                    state = AbilityState.active; //SET THE ABILITY TO READY
                    activeTime = abilities[0].activeTime; //SET THE ACTIVE TIME TO THE ABILITY'S ACTIVE TIME AND START THE 
                    
                    Debug.Log("activated");
                }
            break;
            
            //ACTIVE
            case AbilityState.active:
                if(activeTime > 0){ //WHILE THE ABILITY IS ACTIVE
                    activeTime -= Time.deltaTime; //SUBTRACT TIME UNTIL IT HITS 0
                }
                else{
                    state = AbilityState.cooldown; //AND THEN PUT THE ABILITY ON COOLDOWN
                    cooldownTime = abilities[0].cooldownTime; //SET THE COOLDOWN TIME TO THE ABILITY'S COOLDOWN TIME AND THEN START THE COOLDOWN TIMER
                }
            break;

            //COOLDOWN
            case AbilityState.cooldown:
                if(cooldownTime > 0){ //WHILE THE ABILITY IS ON COOLDOWN
                    cooldownTime -= Time.deltaTime; //SUBTRACT TIME FROM THE COOLDOWN TIMER UNTIL THE ABILITY IS READY AGAIN
                }
                else{
                    state = AbilityState.ready; //AND THEN SET THE ABILITY TO READY
                }
            break;  

        }
    }
}
