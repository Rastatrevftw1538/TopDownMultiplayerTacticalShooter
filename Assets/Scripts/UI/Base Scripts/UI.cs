using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

[RequireComponent(typeof(TMP_Text))]
public class UI : NetworkBehaviour
{
    [Header("Message Components")]
    public string announcement = "";
    public Color messageColor;
    public float activeTime;
    [HideInInspector]public float defActiveTime;
    [HideInInspector]public bool hasRecievedMessage;

    //OFTEN USED COMPONENTS
    [HideInInspector]public TMP_Text message;
    [HideInInspector]public Color[] teamColors = { Color.red, Color.blue };

    public virtual void UpdateUI(ChangeBaseState evtData)
    {

    }

    public virtual void UpdateUI(StartTeamRespawn evtData)
    {

    }

    public virtual void UpdateUI(BaseDestroyed evtData)
    {

    }

    public virtual void UpdateUI(DisplayUI evtData)
    {

    }

    public void DisableUI(DisableUI evtData)
    {
        if(this.gameObject != evtData.priorityUI)
            this.gameObject.SetActive(false);
    }

    public void DisableUI()
    {
        this.gameObject.SetActive(false);
        hasRecievedMessage = false;
    }

    public void MakePriorityUI()
    {
        //DISABLE ALL OTHER UI'S
        DisableUI disableUI = new DisableUI();
        disableUI.priorityUI = this.gameObject;
        EvtSystem.EventDispatcher.Raise<DisableUI>(disableUI);
    }

    public void ReplaceUI(ReplaceUI evtData)
    {
        message.text = evtData.replacementMessage;
    }
}
