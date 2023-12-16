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
    private CustomNetworkManager networkManager;
    [SerializeField] public GameObject Level;
    private GameObject baseObjects;
    public int baseHealth = 1000;
    private BaseEffects[] allBases;
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
    public  bool endedGame = false;

    public float teamRespawnTime = 2f;
    public AudioSource respawnAudioSource;
    public List<AudioClip> respawnAudios = new List<AudioClip>();

    public bool HasGameStarted() {
        return this.gameStarted;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    [SyncVar] public float bluePoints;
    [SyncVar] public float redPoints;

    TMP_Text BlueScoreUI;
    TMP_Text RedScoreUI;
    TMP_Text GameTimeUI;
    TMP_Text IPAddressUI;

    private void Start()
    {
        ui = this.transform.GetChild(0).gameObject;
        allBases = FindObjectsOfType<BaseEffects>();

        currentTime = gameTime;

        //SUBSCRIBE TO EVENT 'PlayerDied' CALLED WITHIN 'PlayerHealth.cs', INVOKE 'AddPlayerDied'
        EvtSystem.EventDispatcher.AddListener<PlayerDied>(AddPlayerDied);

        EvtSystem.EventDispatcher.AddListener<EndGame>(EndGameAndDisplay);

        //CACHING THE COMPONENTS
        BlueScoreUI = ui.transform.GetChild(0).GetComponent<TMP_Text>();
        RedScoreUI  = ui.transform.GetChild(1).GetComponent<TMP_Text>();
        GameTimeUI  = ui.transform.GetChild(2).GetComponent<TMP_Text>();
        IPAddressUI = ui.transform.GetChild(6).GetComponent<TMP_Text>();

        respawnAudioSource = this.GetComponent<AudioSource>();

        IPAddressUI.text = "The IP Address is: " + networkManager.GetLocalIPAddress();
        bluePoints = 0f;
        redPoints  = 0f;
    }

    private void FixedUpdate() {
        if (gameStarted)
        {
            currentTime -= Time.deltaTime;

            GameTimeUI.text  = currentTime.ToString();
            BlueScoreUI.text = "Blue: " + bluePoints;
            RedScoreUI.text  = "Red:  " + redPoints;
        }
    }


    public GameObject[] players;
    PlayerScript currentPlayer;
    bool hasCheckedPlayers = false;
    private void Update()
    {
        if (!hasCheckedPlayers && gameStarted)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            Debug.LogError("Set Teams and set Colors");
            //ADD PLAYERS TO TEAMS
            foreach (GameObject player in players)
            {
                if (player.GetComponent<PlayerScript>() != null)
                    currentPlayer = player.GetComponent<PlayerScript>();

                if (currentPlayer != null)
                {
                    //currentPlayer.gameObject.transform.position.z = -3;
                    if (currentPlayer.PlayerTeam == PlayerScript.Team.Red)
                    {
                        if (!redTeam.Contains(player))
                            redTeam.Add(player);
                    }
                    else if (currentPlayer.PlayerTeam == PlayerScript.Team.Blue)
                    {
                        if (!blueTeam.Contains(player))
                            blueTeam.Add(player);


                    }
                }
            }
            setColorsOfPlayers(players);
            hasCheckedPlayers = true;
        }

        //DETERMINE WINNER
        if(currentTime <= 0 && !endedGame){
            DetermineWinnerForTimeOut();
        }
        
    }

    public void OnPlayerConnected(NetworkConnection conn)
    {
        if(gameStarted){
            print("<color=red> Player: "+conn.identity.name+" joined late</color>");
            addToTeam(conn.identity.gameObject,NetworkServer.connections.Count-1);
            players = GameObject.FindGameObjectsWithTag("Player");

            hasCheckedPlayers = false;
        }
    }

    //[Command(requiresAuthority = false)]
    [ClientRpc]
    public void StartGame()
    {
        // Initialize scores
        gameStarted = true;
        int playerCount = 1;

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
        Invoke(nameof(EndGame), timeBeforeRestartingGame);
    }

    private void EndGameAndDisplay(EndGame evtData)
    {
        DisplayUI displayWinner = new DisplayUI();
        if (evtData.BlueWin())
        {
            displayWinner.colorOfText = Color.blue;
            displayWinner.textToDisplay = "BLUE TEAM WINS!!!!";
        }
        else
        {
            displayWinner.colorOfText = Color.red;
            displayWinner.textToDisplay = "RED TEAM WINS!!!!";
        }

        foreach (GameObject allPlayers in players)
        {
            try { PlayerScript currentPlayer = allPlayers.GetComponent<PlayerScript>(); }
            catch { }

            currentPlayer.setCanMove(false);
        }

        EvtSystem.EventDispatcher.Raise<DisplayUI>(displayWinner);

        endedGame = true;
    }

    [ClientRpc]
    private void EndGame()
    {
        //SET FUNCTIONALITY IN THE 'EndHost' FUNCTION IN 'CustomNetworkManager.cs'
        EndGame endGame = new EndGame();
        endGame.bluePoints = bluePoints;
        endGame.redPoints  = redPoints;


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

        //DEATH SOUND
        int rnd = UnityEngine.Random.Range(0,respawnAudios.Count - 1);
        respawnAudioSource.PlayOneShot(respawnAudios[rnd]);


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

    private void StopGameFunctionality()
    {
        //HANDLE WHAT YOU WANT TO BE INTERACTABLE AFTER THE GAME ENDS
        
    }

    [Command]
    private void setColorsOfPlayers()
    {
        setColorsOfPlayers(players);
    }

    [ClientRpc]
    public void setColorsOfPlayers(GameObject[] players)
    {
        PlayerScript currentPlayer;
        SpriteRenderer currentPlayerSprite;
        foreach (GameObject player in players)
        {
            currentPlayer = player.GetComponent<PlayerScript>();
            currentPlayerSprite = player.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>();
            if (currentPlayer.playerTeam == PlayerScript.Team.Red)
            {
                Debug.LogError("SET PLAYER COLOR TO RED");
                currentPlayerSprite.color = Color.red;
            }
            else if (currentPlayer.playerTeam == PlayerScript.Team.Blue)
            {
                Debug.LogError("SET PLAYER COLOR TO BLUE");
                currentPlayerSprite.color = Color.blue;
            }
            else
            {
                //DEBUG COLOR
                currentPlayerSprite.color = Color.magenta;
            }

            currentPlayer.hasSetColors = true;
        }
        
    }

    public void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        int teamIndex = 0;
        if (conn.identity.gameObject.GetComponent<PlayerScript>().playerTeam == PlayerScript.Team.Blue)
            if (blueTeam.Contains(conn.identity.gameObject))
            {
                teamIndex = blueTeam.IndexOf(conn.identity.gameObject);

                redTeam.Remove(conn.identity.gameObject);
            }
            else if (conn.identity.gameObject.GetComponent<PlayerScript>().playerTeam == PlayerScript.Team.Red)
                if (blueTeam.Contains(conn.identity.gameObject))
                {
                    teamIndex = redTeam.IndexOf(conn.identity.gameObject);

                    redTeam.Remove(conn.identity.gameObject);
                }
        Debug.Log($"<color = red>Player on team {teamIndex} disconnected.</color>");
    }
    public void DisconnectFromGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            networkManager.StopHost();
            networkManager.gameObject.GetComponent<CustomNetworkDiscovery>().StopDiscovery();
        }
        else if (NetworkClient.isConnected)
        {
            networkManager.StopClient();
            networkManager.gameObject.GetComponent<CustomNetworkDiscovery>().StopDiscovery();
        }
    }
}