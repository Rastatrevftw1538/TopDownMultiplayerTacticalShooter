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

        if (this.gameObject != null)
        {
            this.gameObject.SetActive(false);
        }

        EvtSystem.EventDispatcher.AddListener<DisplayUI>(UpdateUI);
        EvtSystem.EventDispatcher.AddListener<DisableUI>(DisableUI);
        EvtSystem.EventDispatcher.AddListener<ReplaceUI>(ReplaceUI);

        defActiveTime = activeTime;
    }

    public override void UpdateUI(DisplayUI evtData)
    {

        if (this.gameObject != null)
        {
            this.gameObject.SetActive(true);
            activeTime = defActiveTime;
            hasRecievedMessage = true;

            message.color = evtData.colorOfText;
            message.text = evtData.textToDisplay;

            message.fontWeight = FontWeight.Bold;

            Invoke(nameof(DisableUI), activeTime);
        }
    }
}
