using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class BaseCountDownUI : MonoBehaviour
{
    //COMPONENTS
    private TMP_Text message;
    private Color[] teamColor = { Color.red, Color.blue };

    //VARIABLES
    private bool hasReceivedMessage = false;
    private float _seconds = 0f;
    private float _miliseconds = 0f;

    [Header("UI Message")]
    public float announcement;

    void Start()
    {
        this.gameObject.SetActive(false);
        message = this.GetComponent<TMP_Text>();
        EvtSystem.EventDispatcher.AddListener<StartTeamRespawn>(UpdateUI);
    }

    // Update is called once per frame
    void Update()
    {
        _seconds -= Time.deltaTime;
        //_miliseconds -= Time.deltaTime * 100;

        if (hasReceivedMessage && _seconds >= 0)
            message.text = string.Format("{00}", _seconds);
        else
            this.gameObject.SetActive(false);
    }

    void UpdateUI(StartTeamRespawn evtData)
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

        message.text = "";
        _seconds = evtData.respawnTime;
        hasReceivedMessage = true;
    }
}
