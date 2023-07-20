using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class HeistGameManager : NetworkBehaviour
{
    [SerializeField] public GameObject Level;
    private GameObject baseObjects;
    public int baseHealth = 1000;
    [SyncVar] private Base redBase;
    [SyncVar] private Base blueBase;
    [SyncVar] private Base currentVulnerableBase;
    //[SerializeField] public GameObject heistBase;
    [SerializeField] private GameObject ui;
    public static HeistGameManager instance;
    [SyncVar] [SerializeField] public List<GameObject> redTeam;
    [SyncVar] [SerializeField] public List<GameObject> blueTeam;
    [SyncVar] public List<GameObject> redTeamDead;
    [SyncVar] public List<GameObject> blueTeamDead;
    [SyncVar] public List<GameObject> playersToRespawn;
    [SyncVar] public float gameTime = 180;
    [SyncVar] private float currentTime;
    public int rounds = 3;
    [SyncVar] private int currentRounds = 1;

    [HideInInspector]
    public int connectedPlayersCount = 0;
    private bool gameStarted = false;

    public float teamRespawnTime = 2f;

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
        baseObjects = Level.transform.Find("BaseSpawnPoints").gameObject;
        redBase = baseObjects.transform.Find("Base_Red").GetComponent<Base>();
        blueBase = baseObjects.transform.Find("Base_Blue").GetComponent<Base>();

        currentTime = gameTime;
        //SUBSCRIBE TO EVENT 'ChangeBaseState' CALLED WITHIN 'Base.cs', INVOKE 'GetBaseData'
        EvtSystem.EventDispatcher.AddListener<ChangeBaseState>(GetBaseData);

        //SUBSCRIBE TO EVENT 'PlayerDied' CALLED WITHIN 'PlayerHealth.cs', INVOKE 'AddPlayerDied'
        EvtSystem.EventDispatcher.AddListener<PlayerDied>(AddPlayerDied);

        //SUBSCRIBE TO EVENT
        //EvtSystem.EventDispatcher.AddListener<StartTeamRespawn>(StartTeamRespawn);
    }

private void FixedUpdate() {
    if(gameStarted)
    currentTime -= Time.deltaTime;
        ui.transform.GetChild(2).GetComponent<TMP_Text>().text = ((int)currentTime).ToString();
        ui.transform.GetChild(0).GetComponent<TMP_Text>().text = "Blue: " + blueBase.GetHealth();
        ui.transform.GetChild(1).GetComponent<TMP_Text>().text = "Red: " + redBase.GetHealth();
}
    private void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if(gameStarted){
            //ADD PLAYERS TO TEAMS
            foreach (GameObject player in players){
                if(player.GetComponent<PlayerScript>().PlayerTeam == PlayerScript.Team.Red){
                    if(!redTeam.Contains(player))
                        redTeam.Add(player);
                }
                else if(player.GetComponent<PlayerScript>().PlayerTeam == PlayerScript.Team.Blue){
                    if(!blueTeam.Contains(player))
                        blueTeam.Add(player);
                }
            }
            
            //DETERMINE WINNER
            if(currentTime == 0){
                DetermineWinnerForTimeOut();
            }
        }
    }
    public void OnPlayerConnected(NetworkConnection conn)
    {
        if(gameStarted){
            print("<color=red> Player: "+conn.identity.name+" joined late</color>");
            addToTeam(conn.identity.gameObject,NetworkServer.connections.Count-1);
        }
        /*
        PlayerHealth playerHealth = conn.identity.GetComponent<PlayerHealth>();
        if (!playerHealth.checkIfAlive)
        {
            OnPlayerDied(playerHealth);
        }
        */
    }
    //[Command(requiresAuthority = false)]
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
        redBase.setHealth(baseHealth);
        blueBase.setHealth(baseHealth);
        int playerCount = 1;
        ui.transform.GetChild(2).GetComponent<TMP_Text>().text = gameTime.ToString();
        ui.transform.GetChild(0).GetComponent<TMP_Text>().text = "Blue: " + blueBase.GetHealth();
        ui.transform.GetChild(1).GetComponent<TMP_Text>().text = "Red: " + redBase.GetHealth();
        if(redTeam.Count == 0 & blueTeam.Count == 0){
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

    private void DetermineWinnerForTimeOut()
    {
        // Compare the scores and determine the winning team
        if (blueBase.GetHealth() > redBase.GetHealth())
        {
            // Blue team wins
            // Handle the win condition
        }
        else if (redBase.GetHealth() > blueBase.GetHealth())
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
        blueBase.setHealth(baseHealth);
        redBase.setHealth(baseHealth);

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
    }

    public void ClearDead()
    {
        redTeamDead.Clear();
        blueTeamDead.Clear();
        playersToRespawn.Clear();

        //RAISE NEW EVENT FOR UI TO HEAR
        ChangeBaseState baseState = new ChangeBaseState();
        baseState.isBaseVulnerable = false;
        baseState.thisBase = currentVulnerableBase;
        baseState.team = currentVulnerableBase.team;
        EvtSystem.EventDispatcher.Raise<ChangeBaseState>(baseState);
    }

    private void StartTeamRespawn(StartTeamRespawn evtData)
    {
        Debug.Log("<color=yellow>STARTING TEAM RESPAWN</color>");
        //RESPAWN ALL THE DEAD PLAYERS
        foreach (GameObject deadPlayer in evtData.teamToRespawn)
        {
            PlayerHealth playerHealthScript = deadPlayer.GetComponent<PlayerHealth>(); //GET ALL OF THE 'PlayerHealth' SCRIPTS IN THE GAMEOBJECTS INSIDE THE DEAD PLAYERS LIST,
            playerHealthScript.Respawn(teamRespawnTime); //RESPAWN THEM ALL IN 'teamRespawnTime' seconds.
        }

        //CLEAR ALL THE DEAD TEAM MEMBERS LIST
        Invoke("ClearDead", teamRespawnTime);

        //CHANGE THE BASE STATE AGAIN

    }

    private void AddPlayerDied(PlayerDied evtData)
    {
        //GET THE PLAYER THAT DIED, AND ADD THEM TO THE LIST OF DEAD PLAYERS ON THEIR CORRESPONDING TEAM
        PlayerScript playerScript = evtData.playerThatDied.GetComponent<PlayerScript>(); //GETS THE CURRENT PLAYER SCRIPT ATTACHED TO THE PLAYER THAT JUST DIED
        if (playerScript.playerTeam == PlayerScript.Team.Red && !searchDuplicates(playersToRespawn.ToArray(), evtData.playerThatDied))
        {
            //ADD THE DEAD PLAYER TO THE LIST OF DEAD TEAM MEMBERS
            redTeamDead.Add(playerScript.gameObject); //IF THIS DOESN'T WORK, MAKE A DIRECT REFERENCE THROUGH THE EVTDATA
            Debug.LogWarning("<color=red>ADDED PLAYER TO DEAD TEAM </color>");
        }
        else
        {
            blueTeamDead.Add(playerScript.gameObject);
            Debug.LogWarning("<color=blue>ADDED PLAYER TO DEAD TEAM </color>");
        }

        if (!searchDuplicates(playersToRespawn.ToArray(), evtData.playerThatDied))
            playersToRespawn.Add(evtData.playerThatDied);


        //IF EITHER OF THE TEAMS ARE FULLY DEAD
        if (redTeamDead.Count >= redTeam.Count)
        {
            StartBasePhaseEvent(redBase);
        }
        else if (blueTeamDead.Count >= blueTeam.Count)
        {
            StartBasePhaseEvent(blueBase);
        }
    }

    private bool searchDuplicates(GameObject[] arr, GameObject find)
    {
        for (int i = 0; i < arr.Length; i++)
            if (arr[i] == find)
                return true;

        return false;
    }

    private void StartBasePhaseEvent(Base whichBase)
    {
        //START THE BASE VULNERABLE PHASE
        ChangeBaseState baseState = new ChangeBaseState();
        baseState.isBaseVulnerable = true;
        baseState.thisBase = whichBase;
        baseState.team = whichBase.team;

        currentVulnerableBase = whichBase;

        Debug.LogWarning("<color=purple>STARTING BASE PHASE</color>");
        EvtSystem.EventDispatcher.Raise<ChangeBaseState>(baseState);

        //RAISE A START RESPAWN TEAM PHASE
        StartTeamRespawn startTeamRespawn = new StartTeamRespawn();
        startTeamRespawn.teamToRespawn = playersToRespawn;
        startTeamRespawn.respawnTime = teamRespawnTime;
        startTeamRespawn.team = whichBase.team;
        EvtSystem.EventDispatcher.Raise<StartTeamRespawn>(startTeamRespawn);
    }
}