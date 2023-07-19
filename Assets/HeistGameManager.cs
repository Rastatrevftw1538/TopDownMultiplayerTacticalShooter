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
    //[SerializeField] public GameObject heistBase;
    [SerializeField] private GameObject ui;
    public static HeistGameManager instance;
    [SyncVar] [SerializeField] public List<GameObject> redTeam;
    [SyncVar] [SerializeField] public List<GameObject> blueTeam;

    [SyncVar] private List<GameObject> redTeamDead;
    [SyncVar] private List<GameObject> blueTeamDead;
    private List<GameObject> teamToRespawn;

    [SyncVar] public int blueTeamBaseHealth;
    [SyncVar] public int redTeamBaseHealth;
    [SyncVar] public int gameTime = 180;
    public int rounds = 3;
    [SyncVar] private int currentRounds = 1;

    [HideInInspector]
    public int connectedPlayersCount = 0;
    private bool gameStarted = false;

    public float teamRespawnTime;

    public bool HasGameStarted() {
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
        heistSpawnPoints = Level.transform.Find("BaseSpawnPoints").gameObject;

        //SUBSCRIBE TO EVENT 'ChangeBaseState' CALLED WITHIN 'Base.cs', INVOKE 'GetBaseData'
        EvtSystem.EventDispatcher.AddListener<ChangeBaseState>(GetBaseData);

        //SUBSCRIBE TO EVENT 'PlayerDied' CALLED WITHIN 'PlayerHealth.cs', INVOKE 'AddPlayerDied'
        EvtSystem.EventDispatcher.AddListener<PlayerDied>(AddPlayerDied);
    }

    private void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if(gameStarted)
            foreach (GameObject player in players){
                Debug.Log("Player "+player.name);
                Debug.Log("Team: "+player.GetComponent<PlayerScript>().PlayerTeam);
                if(player.GetComponent<PlayerScript>().PlayerTeam == PlayerScript.Team.Red){
                    if(!redTeam.Contains(player))
                        redTeam.Add(player);
                }
                else if(player.GetComponent<PlayerScript>().PlayerTeam == PlayerScript.Team.Blue){
                    if(!blueTeam.Contains(player))
                        blueTeam.Add(player);
                } 
            }
    }
    public void OnPlayerConnected(NetworkConnection conn)
    {
        PlayerHealth playerHealth = conn.identity.GetComponent<PlayerHealth>();
        if (!playerHealth.checkIfAlive)
        {
            OnPlayerDied(playerHealth);
        }
    }
    private void OnPlayerDied(PlayerHealth playerHealth)
    {
        Debug.Log("Player died: " + playerHealth.gameObject.name);
    }
    [ClientRpc]
    public void StartGame()
    {
        // Initialize scores
        /*
        foreach (GameObject baseLocal in heistSpawnPoints.GetComponentsInChildren<GameObject>()){
            if (!baseLocal.name.Equals("BaseSpawnPoints")) {
                Debug.Log("Base Team Affiliation: " + baseLocal.tag);
            }
        }*/
        gameStarted = true;
        blueTeamBaseHealth = 1000;
        redTeamBaseHealth = 1000;
        int playerCount = 1;
        ui.transform.GetChild(2).GetComponent<TMP_Text>().text = gameTime.ToString();
        ui.transform.GetChild(0).GetComponent<TMP_Text>().text = "Blue: " + blueTeamBaseHealth.ToString();
        ui.transform.GetChild(1).GetComponent<TMP_Text>().text = "Red: " + redTeamBaseHealth.ToString();
        Debug.Log("Values Set!");
        Debug.Log(NetworkServer.connections.Count);
        while (playerCount < NetworkServer.connections.Count) {
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values) {
                if (conn.isReady) {
                    Debug.Log("Player index: " + playerCount + " Player: " + conn.identity.gameObject.name);
                    addToTeam(conn.identity.gameObject, playerCount - 1);
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
    private void addToTeam(GameObject player, int index)
    {
        if (index % 2 == 0)
        {
            player.GetComponent<PlayerScript>().PlayerTeam = PlayerScript.Team.Red;
        }
        else
        {
            player.GetComponent<PlayerScript>().PlayerTeam = PlayerScript.Team.Blue;
        }

    }

    private IEnumerator GameLoop()
    {
        ui.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = gameTime.ToString();
        ui.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Blue: " + blueTeamBaseHealth.ToString();
        ui.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Red: " + redTeamBaseHealth.ToString();
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
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values) {
            conn.identity.transform.GetChild(3).GetComponent<BountyLogic>().bountyPoints = 1;
        }

        // Restart the game or perform other actions as needed
        StartGame();
    }

    //BASE STUFF
    private void GetBaseData(ChangeBaseState evtData)
    {
        if (evtData.isBaseVulnerable) //IF THE BASE IS VULNERABLE,
        {
            // START THE RESPAWN TIMER
            EvtSystem.EventDispatcher.AddListener<StartTeamRespawn>(StartTeamRespawn);
        }
        else //IF THE BASE IS NOT VULNERABLE,
        {

        }
    }

    private void StartTeamRespawn(StartTeamRespawn evtData)
    {
        if (redTeamDead.Count == redTeam.Count) //IF THE AMOUNT OF DEAD PLAYERS EQUALS THE AMOUNT OF TEAM PLAYERS, START THE RESPAWN
        {
            foreach (GameObject deadPlayer in redTeamDead) //GET ALL GAMEOBJECTS IN THE DEAD TEAM LIST
            {
                PlayerHealth playerHealthScript = deadPlayer.GetComponent<PlayerHealth>(); //GET ALL OF THE 'PlayerHealth' SCRIPTS IN THE GAMEOBJECTS INSIDE THE DEAD PLAYERS LIST,
                playerHealthScript.Respawn(teamRespawnTime); //RESPAWN THEM ALL IN 'teamRespawnTime' seconds.
            }
        }
        else if (blueTeamDead.Count == blueTeam.Count) //IF THE AMOUNT OF DEAD PLAYERS EQUALS THE AMOUNT OF TEAM PLAYERS, START THE RESPAWN
        {
            foreach (GameObject deadPlayer in redTeamDead) //GET ALL GAMEOBJECTS IN THE DEAD TEAM LIST
            {
                PlayerHealth playerHealthScript = deadPlayer.GetComponent<PlayerHealth>(); //GET ALL OF THE 'PlayerHealth' SCRIPTS IN THE GAMEOBJECTS INSIDE THE DEAD PLAYERS LIST,
                playerHealthScript.Respawn(teamRespawnTime); //RESPAWN THEM ALL IN 'teamRespawnTime' seconds.
            }
        }

        //CLEAR ALL THE DEAD TEAM MEMBERS LIST
        redTeamDead.Clear();
        blueTeamDead.Clear();
    }

    private void AddPlayerDied(PlayerDied evtData)
    {
        //GET THE PLAYER THAT DIED, AND ADD THEM TO THE LIST OF DEAD PLAYERS ON THEIR CORRESPONDING TEAM
        PlayerScript playerScript = evtData.playerThatDied.GetComponent<PlayerScript>(); //GETS THE CURRENT PLAYER SCRIPT ATTACHED TO THE PLAYER THAT JUST DIED

        if (playerScript.playerTeam == PlayerScript.Team.Red)
        {
            //ADD THE DEAD PLAYER TO THE LIST OF DEAD TEAM MEMBERS
            redTeamDead.Add(playerScript.gameObject); //IF THIS DOESN'T WORK, MAKE A DIRECT REFERENCE THROUGH THE EVTDATA
        }
        else
        {
            blueTeamDead.Add(playerScript.gameObject);
        }

        if (redTeamDead.Count == redTeam.Count || blueTeamDead.Count == blueTeam.Count) //IF EITHER OF THE TEAMS ARE FULLY DEAD
        {
            ChangeBaseState baseState = new ChangeBaseState();
            baseState.isBaseVulnerable = true;
            EvtSystem.EventDispatcher.Raise<ChangeBaseState>(baseState);
        }
    }

    private void setTeam(List<GameObject> oldValue, List<GameObject> newValue)
    {
        oldValue = newValue;
    }
}