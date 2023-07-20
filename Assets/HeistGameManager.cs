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
    //[SerializeField] public GameObject heistBase;
    [SerializeField] private GameObject ui;
    public static HeistGameManager instance;
    [SyncVar] [SerializeField] public List<GameObject> redTeam;
    [SyncVar] [SerializeField] public List<GameObject> blueTeam;
    [SyncVar] [HideInInspector] public List<GameObject> redTeamDead;
    [SyncVar] [HideInInspector] public List<GameObject> blueTeamDead;
    [SyncVar] public List<GameObject> playersToRespawn;
    [SyncVar] public float gameTime = 180;
    [SyncVar] private float currentTime;
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
        baseObjects = Level.transform.Find("BaseSpawnPoints").gameObject;
        redBase = baseObjects.transform.Find("Base_Red").GetComponent<Base>();
        blueBase = baseObjects.transform.Find("Base_Blue").GetComponent<Base>();

        currentTime = gameTime;
        //SUBSCRIBE TO EVENT 'ChangeBaseState' CALLED WITHIN 'Base.cs', INVOKE 'GetBaseData'
        EvtSystem.EventDispatcher.AddListener<ChangeBaseState>(GetBaseData);

        //SUBSCRIBE TO EVENT 'PlayerDied' CALLED WITHIN 'PlayerHealth.cs', INVOKE 'AddPlayerDied'
        EvtSystem.EventDispatcher.AddListener<PlayerDied>(AddPlayerDied);
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
            foreach (GameObject player in players){
                if(player.GetComponent<PlayerScript>().PlayerTeam == PlayerScript.Team.Red){
                    if(!redTeam.Contains(player))
                        redTeam.Add(player);
                }
                else if(player.GetComponent<PlayerScript>().PlayerTeam == PlayerScript.Team.Blue){
                    if(!blueTeam.Contains(player))
                        blueTeam.Add(player);
                }
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (!playerHealth.checkIfAlive){
                    if(!playersToRespawn.Contains(playerHealth.gameObject))
                        print("SOMEONE DIED!");
                        playersToRespawn.Add(playerHealth.gameObject);
                        OnPlayerDied(playerHealth);
                    }
            }
            if(playersToRespawn.Count > 0){
                if(blueTeamDead.Count == blueTeam.Count && !blueBase.canHit){
                    blueBase.StartPhase(true);
                    Invoke("StartTeamRespawnCall", teamRespawnTime);
                    if(blueBase.GetHealth() == 0){

                    }
                }
                if(redTeamDead.Count == redTeam.Count && !redBase.canHit){
                    redBase.StartPhase(true);
                    Invoke("StartTeamRespawnCall", teamRespawnTime);
                    if(redBase.GetHealth() == 0){

                    }
                }
            }
            if(currentTime == 0){
                DetermineWinnerForTimeOut();
            }
        }
    }
    public void OnPlayerConnected(NetworkConnection conn)
    {
        /*
        PlayerHealth playerHealth = conn.identity.GetComponent<PlayerHealth>();
        if (!playerHealth.checkIfAlive)
        {
            OnPlayerDied(playerHealth);
        }
        */
    }
    //[Command(requiresAuthority = false)]
    private void OnPlayerDied(PlayerHealth playerHealth)
    {
        Debug.Log("<color=red>GRIM REAPER CHECKING HIS LIST!</color>");
        if(playerHealth.GetComponent<PlayerScript>().PlayerTeam == PlayerScript.Team.Red){
            if(!redTeamDead.Contains(playerHealth.gameObject))
                redTeamDead.Add(playerHealth.gameObject);
                Debug.Log(playerHealth.gameObject.name+ " added to Dead List");
        }
        else if(playerHealth.GetComponent<PlayerScript>().PlayerTeam == PlayerScript.Team.Blue){
            if(!blueTeamDead.Contains(playerHealth.gameObject))
                blueTeamDead.Add(playerHealth.gameObject);
                Debug.Log(playerHealth.gameObject.name+ " added to Dead List");
    }
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
        else //IF THE BASE IS NOT VULNERABLE,
        {

        }
    }
    private void StartTeamRespawnCall(){
        foreach(GameObject deadPlayer in playersToRespawn){
            PlayerHealth playerHealthScript = deadPlayer.GetComponent<PlayerHealth>();
            playerHealthScript.Respawn(teamRespawnTime);
            if(blueTeamDead.Count == blueTeam.Count && !blueBase.canHit){
                blueBase.StartPhase(false);
            }
            if(redTeamDead.Count == redTeam.Count && !redBase.canHit){
                redBase.StartPhase(false);
            }
        }
        ClearDead();
    }

    public void ClearDead()
    {
        redTeamDead.Clear();
        blueTeamDead.Clear();
        playersToRespawn.Clear();
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
        playersToRespawn.Clear();
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