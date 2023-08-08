using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BountyLogic : NetworkBehaviour
{
    public TMP_Text bountyPlayerText;
    public TMP_Text bountyUI;
    public string bountyPlayerName;
    private GameObject player;
    public int maxBountyPoints = 7;
    [HideInInspector]
    public int bountyPoints = 1;

    private void Start() {
        player = GameObject.FindObjectOfType<PlayerScript>().gameObject;
        AttachToPlayer(player.name.ToString());
    }
    public void AttachToPlayer(string name)
    {
        bountyPlayerName = "Bounty for: "+name;
        bountyPlayerText.text = bountyPlayerName;
        bountyUI.text = bountyPoints.ToString();
    }
    private void Update() {
        this.transform.position = player.transform.position;
    }
    public void CollectBountyPoint()
    {
    if (bountyPoints < maxBountyPoints)
    {
        bountyPoints++;
        //playerGameMode.UpdateBounty(bountyPoints);
    }
    else
    {
        //ResetBountyPoints();
    }
}
}
