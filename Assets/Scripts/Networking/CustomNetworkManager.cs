using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    [HideInInspector]
    public BountyGameManager gameManager;
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server Started");
        Invoke("SearchForGameManager",0.5f);
        
    }
    private void SearchForGameManager() {
        Scene gameScene = SceneManager.GetSceneByName("LevelTemplate");
        if (gameScene.IsValid() && gameScene.isLoaded)
        {
            Debug.Log("Searching for It!");
            gameManager = GameObject.FindObjectOfType<BountyGameManager>();
                if(gameManager!=null){
                    Debug.Log("Found It!");
                }
        }
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // Player has connected to the server
        Debug.Log("Player connected: " + conn.identity);
        
        // Notify the game manager that a player has connected
        BountyGameManager.instance.OnPlayerConnected(conn);
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
        base.OnClientConnect();
        Debug.Log("Client Connected");
        
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        SceneManager.LoadScene(0);
    }
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        // Check if the server is full
        if (NetworkServer.connections.Count > CustomNetworkManager.singleton.maxConnections)
        {
            // Server is full, disconnect the client
            conn.Disconnect();
            return;
        }
        if(NetworkServer.connections.Count == CustomNetworkManager.singleton.maxConnections){
            Debug.Log("Game Starting!!!");
            gameManager.StartCoroutine(gameManager.StartGame());
        }
        
    }
}

