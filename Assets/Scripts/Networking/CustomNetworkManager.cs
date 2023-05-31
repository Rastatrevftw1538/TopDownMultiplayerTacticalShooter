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
        /*
        Debug.Log("Client Started");
        SceneManager.LoadScene(1);
        GameObject player = Instantiate(playerPrefab);
        NetworkIdentity networkIdentity = player.GetComponent<NetworkIdentity>();
        NetworkServer.Spawn(player,networkIdentity.connectionToClient);
        Debug.Log("Server Add Player"+networkIdentity.connectionToClient.connectionId);
        //SpawnPlayerWithAuthority(conn);
        */
    }
    public override void OnClientConnect()
    {
        Debug.Log("Client Connected");
        
    }
    public override void OnClientDisconnect()
    {
        SceneManager.LoadScene(0);
    }
}
