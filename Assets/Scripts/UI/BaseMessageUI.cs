using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class BaseMessageUI : UI
{
    void Start()
    {
        message = this.GetComponent<TMP_Text>();
        this.gameObject.SetActive(false);
        //EvtSystem.EventDispatcher.AddListener<ChangeBaseState>(UpdateUI);
        //EvtSystem.EventDispatcher.AddListener<DisableUI>(DisableUI);
        //EvtSystem.EventDispatcher.AddListener<ReplaceUI>(ReplaceUI);
    }

    public override void UpdateUI(ChangeBaseState evtData)
    {
        //IF THE EVENT WE JUST RECEIVED TOLD US THAT THE BASE IS VULNERABLE, DISPLAY THE MESSAGE. IF NOT, DON'T
        if (evtData.isBaseVulnerable == false)
            this.gameObject.SetActive(false); //IF IT'S RECIEVED THAT THE BASE IS NO LONGER VULNERABLE, DE-ACTIVATE THE GAME OBJECT
        else
        {
            this.gameObject.SetActive(true);

            if (evtData.team == PlayerScript.Team.Red)
                message.color = teamColors[0];
            else if (evtData.team == PlayerScript.Team.Blue)
                message.color = teamColors[1];
            else
                message.color = Color.yellow; //DEBUG COLOR

            //MESSAGE COMPONENTS
            message.fontWeight = FontWeight.Bold;
            message.text = announcement;
        }
    }

    private void ReplaceUI(ReplaceUI evtData)
    {
        message.text = evtData.replacementMessage;
    }
}
