using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class BaseCountDownUI : UI
{
    //COMPONENTS

    //VARIABLES
    private bool hasReceivedMessage = false;
    private float _seconds = 0f;
    private float _miliseconds = 0f;

    void Start()
    {
        this.gameObject.SetActive(false);
        message = this.GetComponent<TMP_Text>();
        //EvtSystem.EventDispatcher.AddListener<StartTeamRespawn>(UpdateUI);
        //EvtSystem.EventDispatcher.AddListener<DisableUI>(DisableUI);
    }

    // Update is called once per frame
    void Update()
    {
        formatCountdown();
    }

    public override void UpdateUI(StartTeamRespawn evtData)
    {
        this.gameObject.SetActive(true);
        hasReceivedMessage = true;

        if (evtData.team == PlayerScript.Team.Red)
            message.color = teamColors[0];
        else if (evtData.team == PlayerScript.Team.Blue)
            message.color = teamColors[1];
        else
            message.color = Color.yellow; //DEBUG COLOR

        //MESSAGE COMPONENTS
        message.fontWeight = FontWeight.Bold;

        message.text = "";
        _seconds = evtData.respawnTime;
        _miliseconds = evtData.respawnTime * 100;
    }

    private void formatCountdown()
    {
        _seconds -= Time.deltaTime;
        _miliseconds -= Time.deltaTime * 100;

        if (hasReceivedMessage && _seconds >= 0)
        {
            message.text = "Players respawn in: " + string.Format("{0}", _seconds.ToString().Substring(0,4));
            if (_miliseconds <= 0)
                _miliseconds = _seconds * 100;
        }
        else
            this.gameObject.SetActive(false); //IF THE COUNTDOWN IS OVER, DE-ACTIVATE THE GAME OBJECT
    }
}
