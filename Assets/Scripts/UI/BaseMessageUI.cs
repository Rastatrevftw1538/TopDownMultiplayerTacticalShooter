using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class BaseMessageUI : MonoBehaviour
{
    private TMP_Text message;
    private Color[] teamColor = { Color.red, Color.blue };
    
    [Header("UI Message")]
    public string announcement = "BASE IS VULNERABLE! ATTACK NOW!";

    void Start()
    {
        this.gameObject.SetActive(false);
        message = this.GetComponent<TMP_Text>();
        EvtSystem.EventDispatcher.AddListener<ChangeBaseState>(UpdateUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateUI(ChangeBaseState evtData)
    {
        //IF THE EVENT WE JUST RECEIVED TOLD US THAT THE BASE IS VULNERABLE, DISPLAY THE MESSAGE. IF NOT, DON'T
        if (evtData.isBaseVulnerable == false)
            this.gameObject.SetActive(false);
        else
        {
            this.gameObject.SetActive(true);

            if (evtData.team == PlayerScript.Team.Red)
                message.color = teamColor[0];
            else if (evtData.team == PlayerScript.Team.Blue)
                message.color = teamColor[1];
            else
                message.color = Color.yellow; //DEBUG COLOR

            //MESSAGE COMPONENTS
            message.fontWeight = FontWeight.Bold;
            message.text = announcement;
        }
    }
}
