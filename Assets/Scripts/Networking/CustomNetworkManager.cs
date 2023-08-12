using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Net.Sockets;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;


public class CustomNetworkManager : NetworkManager
{
    [HideInInspector]
    //public HeistGameManager gameManager;// Reference to the StartMenu UI script
    public ChaseGameManager gameManager;
    public StartMenu startMenu; // Reference to the StartMenu UI script

    // Relay related variables
    private Guid hostAllocationId;
    public string joinCode;

    private bool isLoggedIn = false;

    // Relay allocation region (optional, you can set this based on player's choice)
    private string allocationRegion = "us"; // For example, "us", "eu", etc.
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server Started");
        CreateRelayAllocation();
        Invoke(nameof(SearchForGameManager),0.5f);
        Invoke(nameof(SetLocalIP),0.5f);
        networkAddress = GetLocalIPAddress();
        EvtSystem.EventDispatcher.AddListener<EndGame>(EndHost);
    }
    private void Start() {
        SetLocalIP();
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
    private void SetLocalIP(){
        networkAddress = GetLocalIPAddress();
    }
    private void SearchForGameManager() {
        //Scene gameScene = SceneManager.GetSceneByName("LevelTemplate");
        Scene gameScene = SceneManager.GetSceneByName("VaultChase");
        if (gameScene.IsValid() && gameScene.isLoaded)
        {
            Debug.Log("Searching for It!");
            //gameManager = GameObject.FindObjectOfType<HeistGameManager>();
            gameManager = GameObject.FindObjectOfType<ChaseGameManager>();
            if (gameManager!=null){
                    Debug.Log("Found It!");
                }
        }
    }

    public async void UnityLogin()
		{
			try
			{
				await UnityServices.InitializeAsync();
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Logged into Unity, player ID: " + AuthenticationService.Instance.PlayerId);
                isLoggedIn = true;
            }
			catch (Exception e)
			{
                isLoggedIn = false;
                Debug.Log(e);
			}
		}
    private async void CreateRelayAllocation()
    {
        Debug.Log("Creating Relay Allocation...");

        // Determine the region to use (user-selected or hardcoded)
        //string region = allocationRegion;

        // Important: Once the allocation is created, you have ten seconds to bind
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        hostAllocationId = allocation.AllocationId;
        Debug.Log($"Host Allocation ID: {hostAllocationId}");
        Invoke(nameof(CheckRelayAllocation),0.5f);

    }

    private async void CheckRelayAllocation()
    {
        Debug.Log("Checking Relay Allocation...");

        try
        {
            // Get the join code for the relay allocation
            joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocationId);
            Debug.Log("Join Code: " + joinCode);

        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // Player has connected to the server
        Debug.Log("Player connected: " + conn.identity);

        // Notify the game manager that a player has connected
        //HeistGameManager.instance.OnPlayerConnected(conn);
        ChaseGameManager.instance.OnPlayerConnected(conn);
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
    private void Update() {
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


