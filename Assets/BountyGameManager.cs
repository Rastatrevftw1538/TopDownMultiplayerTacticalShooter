using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class BountyGameManager : NetworkBehaviour
{

    [SerializeField] private GameObject ui;
    public static BountyGameManager instance;

    [SyncVar][SerializeField] public List<GameObject> redTeam;
    [SyncVar][SerializeField] public List<GameObject> blueTeam;

    [SyncVar] public int blueTeamScore;
    [SyncVar] public int redTeamScore;
    [SyncVar] public int gameTime = 180;

    [HideInInspector]
    public int connectedPlayersCount = 0;
    private bool gameStarted = false;

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
        // Handle player death here
        // You can access the playerHealth object to get more information about the player who died

        // Example:
        Debug.Log("Player died: " + playerHealth.gameObject.name);
    }
    public IEnumerator StartGame()
    {

        // Initialize scores
        blueTeamScore = 0;
        redTeamScore = 0;
        int playerindex = 0;
        ui.transform.GetChild(2).GetComponent<TMP_Text>().text = gameTime.ToString();
        ui.transform.GetChild(0).GetComponent<TMP_Text>().text = "Blue: " + blueTeamScore.ToString();
        ui.transform.GetChild(1).GetComponent<TMP_Text>().text = "Red: " + redTeamScore.ToString();
        Debug.Log("Values Set!");
        Debug.Log(NetworkServer.connections.Count);
        GameObject bountyLogicObject = Instantiate(CustomNetworkManager.singleton.spawnPrefabs[2]);
        NetworkClient.RegisterPrefab(bountyLogicObject);
        foreach(NetworkConnectionToClient conn in NetworkServer.connections.Values){
            if(conn.isAuthenticated){
                Debug.Log("Player index: "+playerindex+" Player: "+ conn.identity.gameObject.name);
                addToTeam(conn.identity.gameObject, playerindex);
                NetworkServer.Spawn(bountyLogicObject, conn.identity.connectionToClient);
                // NetworkIdentity identity = bountyLogicObject.GetComponent<NetworkIdentity>();
                // if (identity != null)
                // {
                //     conn.identity.AssignClientAuthority(identity.connectionToClient);
                // }
            playerindex++;
            }
        }

        // Start the game
        yield return StartCoroutine(GameLoop());

        // End the game
        EndGame();
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
        ui.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Blue: "+blueTeamScore.ToString();
        ui.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Red: "+redTeamScore.ToString();
        // Implement your game logic here
        // Track the number of defeated enemies and update the bounty

        // For example, you can have a method called DefeatEnemy() that increments the bounty
        // whenever an enemy is defeated. Call this method whenever an enemy is defeated.

        // Wait for the game to finish (e.g., based on a timer)
        yield return new WaitForSeconds(gameTime);

        // Check the winning team and handle the end of the game
        DetermineWinner();
    }

    private void DetermineWinner()
    {
        // Compare the scores and determine the winning team
        if (blueTeamScore > redTeamScore)
        {
            // Blue team wins
            // Handle the win condition
        }
        else if (redTeamScore > blueTeamScore)
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
        blueTeamScore = 0;
        redTeamScore = 0;
        foreach(NetworkConnectionToClient conn in NetworkServer.connections.Values){
            conn.identity.transform.GetChild(3).GetComponent<BountyLogic>().bountyPoints = 1;
        }

        // Restart the game or perform other actions as needed
        StartCoroutine(StartGame());
    }

    private void OnBountyChanged(int oldValue, int newValue)
    {
        // Handle the bounty value change (e.g., update UI)
        // You can access the bounty value from the Bounty Prefab and update the UI accordingly
        // For example:
        GameObject bountyPrefab = FindObjectOfType<BountyLogic>().gameObject;
        if (bountyPrefab != null)
        {
            //bountyPrefab.UpdateBountyUI(newValue);
        }
    }
}