using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Net.Sockets;

public class CustomNetworkManager : NetworkManager
{
    [HideInInspector]
    public HeistGameManager gameManager;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server Started");
        Invoke(nameof(SearchForGameManager),0.5f);

        EvtSystem.EventDispatcher.AddListener<EndGame>(EndHost);
    }
    private void Start() {
        // Get the local IP address of the device
        string localIPAddress = GetLocalIPAddress();

        // Set the network address of the NetworkManager to the local IP address
        networkAddress = localIPAddress;
    }
    public string GetLocalIPAddress()
    {
            string localHost = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(localHost);

            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddress.ToString();
                }
            }
            // If no suitable IP address is found, return a default value (localhost)
            return "127.0.0.1";
    }
    private void SearchForGameManager() {
        Scene gameScene = SceneManager.GetSceneByName("LevelTemplate");
        if (gameScene.IsValid() && gameScene.isLoaded)
        {
            Debug.Log("Searching for It!");
            gameManager = GameObject.FindObjectOfType<HeistGameManager>();
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
        HeistGameManager.instance.OnPlayerConnected(conn);
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
    public override void Update() {
        bool allReady = false;
        foreach(NetworkConnectionToClient conn in NetworkServer.connections.Values){
            if(conn.isReady){
                allReady = true;
            }
            else{
                allReady = false;
                break;
            }
        }
        if(NetworkServer.connections.Count >= 2 && allReady && gameManager.HasGameStarted()==false){
                Debug.Log("Game Starting!!!");
                gameManager.StartGame();
            }
        
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
    }
    
    private void EndHost(EndGame evtData)
    {
        //StopHost();
    }
}

