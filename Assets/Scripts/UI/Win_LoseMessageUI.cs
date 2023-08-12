using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class Win_LoseMessageUI : UI
{
    void Start()
    {
        message = this.GetComponent<TMP_Text>();
        this.gameObject.SetActive(false);
        //EvtSystem.EventDispatcher.AddListener<BaseDestroyed>(UpdateUI);
        //EvtSystem.EventDispatcher.AddListener<DisableUI>(DisableUI);
        //EvtSystem.EventDispatcher.AddListener<TiedGame>(TiedGameUI);
    }

    public override void UpdateUI(BaseDestroyed evtData)
    {
        this.gameObject.SetActive(true);

        //MESSAGE COMPONENTS
        message.fontWeight = FontWeight.Bold;

        //DETERMINE COLOR OF MESSAGE
        if (evtData.thisBase.team == PlayerScript.Team.Red)
        {
            message.color = teamColors[1];
            message.text = "BLUE TEAM WINS.";
        }
        else if (evtData.thisBase.team == PlayerScript.Team.Blue)
        {
            message.color = teamColors[0];
            message.text = "RED TEAM WINS.";
        }
        else {
            message.color = Color.yellow; //DEBUG COLOR
            message.text = "SOMEBODY WON, BUT THE CODE IS BROKEN.";
        }

        MakePriorityUI();
    }

    private void TiedGameUI(TiedGame evtData)
    {
        this.gameObject.SetActive(true);

        //MESSAGE COMPONENTS
        message.fontWeight = FontWeight.Bold;

        message.color = Color.yellow;
        message.text  = "TIED GAME!";

        MakePriorityUI();
    }
}
