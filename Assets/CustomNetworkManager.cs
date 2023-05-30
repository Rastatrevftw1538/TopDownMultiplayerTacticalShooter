using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameObject ui;
    
    public override void OnStartServer()
    {
        Debug.Log("Server Started");
    }
    public override void OnStartClient()
    {
        Debug.Log("Client Started");

    }
    public override void OnClientConnect()
    {
        
    }
    public override void OnClientDisconnect()
    {
        SceneManager.LoadScene(0);
    }
}
