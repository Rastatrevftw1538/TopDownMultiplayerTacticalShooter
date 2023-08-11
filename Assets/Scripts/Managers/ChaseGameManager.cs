using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class ChaseGameManager : NetworkBehaviour
{
    [SerializeField] public GameObject Level;
    private GameObject baseObjects;
    public int baseHealth = 1000;
    [SyncVar] private BaseEffects currentVulnerableBase;
    //private List<BaseEffects> allBases = new List<BaseEffects>();
    private BaseEffects[] allBases;
    //[SerializeField] public GameObject heistBase;
    [SerializeField] private GameObject ui;
    public static ChaseGameManager instance;
    [SyncVar] [SerializeField] public List<GameObject> redTeam;
    [SyncVar] [SerializeField] public List<GameObject> blueTeam;
    [SyncVar] public List<GameObject> redTeamDead;
    [SyncVar] public List<GameObject> blueTeamDead;
    [SyncVar] public List<GameObject> playersToRespawn;
    [SyncVar] public float gameTime = 180;
    [SyncVar] private float currentTime;
    public int rounds = 3;
    [SyncVar] private int currentRounds = 1;
    public float timeBeforeRestartingGame = 10f;

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

    string UI_Time;
    string BlueScore;
    string RedScore;

    private void Start()
    {
        ui = this.transform.GetChild(0).gameObject;
        //baseObjects = Level.transform.Find("BaseSpawnPoints").gameObject;

        /*foreach (BaseEffects bases in FindObjectsOfType<BaseEffects>())
        {
            allBases.Add(bases);
            //Debug.LogError(bases.name);
        }*/
        allBases = FindObjectsOfType<BaseEffects>();

        currentTime = gameTime;
        //SUBSCRIBE TO EVENT 'ChangeBaseState' CALLED WITHIN 'Base.cs', INVOKE 'GetBaseData'
        EvtSystem.EventDispatcher.AddListener<ChangeBaseState>(GetBaseData);

        //SUBSCRIBE TO EVENT 'PlayerDied' CALLED WITHIN 'PlayerHealth.cs', INVOKE 'AddPlayerDied'
        EvtSystem.EventDispatcher.AddListener<PlayerDied>(AddPlayerDied);

        //SUBSCRIBE TO EVENT
        //EvtSystem.EventDispatcher.AddListener<StartTeamRespawn>(StartTeamRespawn);

        //CACHING THE COMPONENTS
        UI_Time   = ui.transform.GetChild(2).GetComponent<TMP_Text>().text;
        BlueScore = ui.transform.GetChild(2).GetComponent<TMP_Text>().text;
        RedScore  = ui.transform.GetChild(2).GetComponent<TMP_Text>().text;
    }

    private void FixedUpdate() {
        if (gameStarted)
        {
            currentTime -= Time.deltaTime;

            UI_Time  = currentTime.ToString();
            //BlueScore = "Blue: " + blueBase.GetHealth();
            //RedScore  = "Red:  " + redBase.GetHealth();
        }
    }


    GameObject[] players;
    PlayerScript currentPlayer;
    bool hasCheckedPlayers = false;
    private void Update()
    {
        if (!hasCheckedPlayers && gameStarted)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            hasCheckedPlayers = true;
        }

        if(gameStarted){
            //ADD PLAYERS TO TEAMS
            foreach (GameObject player in players){
                currentPlayer = GetComponent<PlayerScript>();
                if (currentPlayer.PlayerTeam == PlayerScript.Team.Red){
                    if(!redTeam.Contains(player))
                        redTeam.Add(player);
                }
                else if(currentPlayer.PlayerTeam == PlayerScript.Team.Blue){
                    if(!blueTeam.Contains(player))
                        blueTeam.Add(player);
                }
            }
            
            //DETERMINE WINNER
            if(currentTime <= 0){
                DetermineWinnerForTimeOut();
            }
        }
    }
    public void OnPlayerConnected(NetworkConnection conn)
    {
        if(gameStarted){
            print("<color=red> Player: "+conn.identity.name+" joined late</color>");
            addToTeam(conn.identity.gameObject,NetworkServer.connections.Count-1);
            players = GameObject.FindGameObjectsWithTag("Player");
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
        //redBase.setHealth(baseHealth);
        //blueBase.setHealth(baseHealth);
        int playerCount = 1;
        UI_Time   = gameTime.ToString();
        //BlueScore = "Blue: " + blueBase.GetHealth();
        //RedScore  = "Red:  " + redBase.GetHealth();

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
        
    }

    [ClientRpc]
    private void EndGame()
    {
        //SET FUNCTIONALITY IN THE 'EndHost' FUNCTION IN 'CustomNetworkManager.cs'
        EndGame endGame = new EndGame();
        EvtSystem.EventDispatcher.Raise<EndGame>(endGame);



        Debug.LogError("Ended the game.");
    }

    private void EndGameThruEvent(BaseDestroyed evtData)
    {
        Invoke(nameof(EndGame), timeBeforeRestartingGame);
    }

    //BASE STUFF
    private void GetBaseData(ChangeBaseState evtData)
    {
        //if (evtData.isBaseVulnerable) //IF THE BASE IS VULNERABLE,
        //{
            //ADD A LISTENER FOR THE DEATH OF AN ENTIRE TEAM
            //EvtSystem.EventDispatcher.AddListener<StartTeamRespawn>(StartTeamRespawn);
        //}
    }

    public void ClearDead()
    {
        redTeamDead.Clear();
        blueTeamDead.Clear();
        playersToRespawn.Clear();

        //RAISE NEW EVENT FOR UI TO HEAR
        ChangeBaseState baseState = new ChangeBaseState();
        baseState.isBaseVulnerable = false;
        baseState.thisBaseEffects = currentVulnerableBase;
        //baseState.team = currentVulnerableBase.team;
        EvtSystem.EventDispatcher.Raise<ChangeBaseState>(baseState);
    }

    private void AddPlayerDied(PlayerDied evtData)
    {
        //GET THE PLAYER THAT DIED, AND ADD THEM TO THE LIST OF DEAD PLAYERS ON THEIR CORRESPONDING TEAM
        PlayerScript playerScript = evtData.playerThatDied.GetComponent<PlayerScript>(); //GETS THE CURRENT PLAYER SCRIPT ATTACHED TO THE PLAYER THAT JUST DIED
        if (playerScript.playerTeam == PlayerScript.Team.Red && !redTeamDead.Contains(evtData.playerThatDied))
        {
            //ADD THE DEAD PLAYER TO THE LIST OF DEAD TEAM MEMBERS
            redTeamDead.Add(playerScript.gameObject); //IF THIS DOESN'T WORK, MAKE A DIRECT REFERENCE THROUGH THE EVTDATA
            Debug.LogWarning("<color=red>ADDED PLAYER TO DEAD TEAM </color>");
        }
        else if(playerScript.playerTeam == PlayerScript.Team.Blue && !blueTeamDead.Contains(evtData.playerThatDied))
        {
            blueTeamDead.Add(playerScript.gameObject);
            Debug.LogWarning("<color=blue>ADDED PLAYER TO DEAD TEAM </color>");
        }

        if (!playersToRespawn.Contains(evtData.playerThatDied))
            playersToRespawn.Add(evtData.playerThatDied);


        /*//IF EITHER OF THE TEAMS ARE FULLY DEAD
        if (redTeamDead.Count >= redTeam.Count)
        {
            StartBasePhaseEvent(redBase);
        }
        else if (blueTeamDead.Count >= blueTeam.Count)
        {
            StartBasePhaseEvent(blueBase);
        }*/
    }

    private void StartBasePhaseEvent(BaseEffects whichBase)
    {
        //START THE BASE VULNERABLE PHASE
        ChangeBaseState baseState  = new ChangeBaseState();
        baseState.isBaseVulnerable = true;
        baseState.thisBaseEffects  = whichBase;
        //baseState.team = whichBase.team;

        currentVulnerableBase = whichBase;

        Debug.LogWarning("<color=purple>STARTING BASE PHASE</color>");
        EvtSystem.EventDispatcher.Raise<ChangeBaseState>(baseState);
    }

    private void StopGameFunctionality()
    {
        //HANDLE WHAT YOU WANT TO BE INTERACTABLE AFTER THE GAME ENDS
        
    }
}