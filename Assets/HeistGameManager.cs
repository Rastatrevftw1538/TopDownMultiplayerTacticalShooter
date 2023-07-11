using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class HeistGameManager : NetworkBehaviour
{
    [SerializeField] public GameObject Level;
    private GameObject heistSpawnPoints;
    [SerializeField] public GameObject heistBase;
    [SerializeField] private GameObject ui;
    public static HeistGameManager instance;
    [SyncVar][SerializeField] public List<GameObject> redTeam;
    [SyncVar][SerializeField] public List<GameObject> blueTeam;

    [SyncVar] public int blueTeamBaseHealth;
    [SyncVar] public int redTeamBaseHealth;
    [SyncVar] public int gameTime = 180;
    public int rounds = 3;
    [SyncVar] private int currentRounds = 1;

    [HideInInspector]
    public int connectedPlayersCount = 0;
    private bool gameStarted = false;

    public bool HasGameStarted(){
        return this.gameStarted;
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        ui = this.transform.GetChild(0).gameObject;
        heistSpawnPoints = Level.transform.GetChild(0).gameObject;
    }
    public void OnPlayerConnected(NetworkConnection conn)
    {
        PlayerHealth playerHealth = conn.identity.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.PlayerDied += OnPlayerDied;
        }
    }
    private void OnPlayerDied(PlayerHealth playerHealth)
    {
        Debug.Log("Player died: " + playerHealth.gameObject.name);
    }
    public void StartGame()
    {
        // Initialize scores
        foreach(Transform baseLocal in heistSpawnPoints.GetComponentsInChildren<Transform>()){
            Instantiate(heistBase,baseLocal.position,baseLocal.rotation,heistSpawnPoints.transform);
        }
        gameStarted = true;
        blueTeamBaseHealth = 1000;
        redTeamBaseHealth = 1000;
        int playerCount = 1;
        ui.transform.GetChild(2).GetComponent<TMP_Text>().text = gameTime.ToString();
        ui.transform.GetChild(0).GetComponent<TMP_Text>().text = "Blue: " + blueTeamBaseHealth.ToString();
        ui.transform.GetChild(1).GetComponent<TMP_Text>().text = "Red: " + redTeamBaseHealth.ToString();
        Debug.Log("Values Set!");
        Debug.Log(NetworkServer.connections.Count);
        while(playerCount < NetworkServer.connections.Count){
            foreach(NetworkConnectionToClient conn in NetworkServer.connections.Values){
                if(conn.isReady){
                    Debug.Log("Player index: "+playerCount+" Player: "+ conn.identity.gameObject.name);
                    addToTeam(conn.identity.gameObject, playerCount-1);
                    //NetworkServer.Spawn(bountyLogicObject, conn.identity.connectionToClient);
                    // NetworkIdentity identity = bountyLogicObject.GetComponent<NetworkIdentity>();
                    // if (identity != null)
                    // {
                    //     conn.identity.AssignClientAuthority(identity.connectionToClient);
                    // }
                playerCount++;
                }
            }
        }
        /*
        while(currentRounds <= rounds){
            // Start the game
            StartCoroutine(GameLoop());
        }
        
        DetermineWinner();
        // End the game
        EndGame();
        */
    }
    [ClientRpc]
    private void addToTeam(GameObject player,int index)
    {
        if (index % 2 == 0)
        {
            redTeam.Add(player);
            player.GetComponent<PlayerScript>().PlayerTeam = PlayerScript.Team.Red;
        }
        else
        {
            blueTeam.Add(player);
            player.GetComponent<PlayerScript>().PlayerTeam = PlayerScript.Team.Blue;
        }
    }

    private IEnumerator GameLoop()
    {
        ui.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = gameTime.ToString();
        ui.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Blue: "+blueTeamBaseHealth.ToString();
        ui.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Red: "+redTeamBaseHealth.ToString();
        // Implement your game logic here
        // Track the number of defeated enemies and update the bounty

        // For example, you can have a method called DefeatEnemy() that increments the bounty
        // whenever an enemy is defeated. Call this method whenever an enemy is defeated.

        // Wait for the game to finish (e.g., based on a timer)
        yield return new WaitForSeconds(gameTime);

        // Check the winning team and handle the end of the game
    }

    private void DetermineWinner()
    {
        // Compare the scores and determine the winning team
        if (blueTeamBaseHealth > redTeamBaseHealth)
        {
            // Blue team wins
            // Handle the win condition
        }
        else if (redTeamBaseHealth > blueTeamBaseHealth)
        {
            // Red team wins
            // Handle the win condition
        }
        else
        {
            // It's a tie, handle the tie condition
        }
    }

    private void EndGame()
    {
        // Clean up and reset the game state
        // For example, reset the scores and bounty
        blueTeamBaseHealth = 0;
        redTeamBaseHealth = 0;
        foreach(NetworkConnectionToClient conn in NetworkServer.connections.Values){
            conn.identity.transform.GetChild(3).GetComponent<BountyLogic>().bountyPoints = 1;
        }

        // Restart the game or perform other actions as needed
        StartGame();
    }
}