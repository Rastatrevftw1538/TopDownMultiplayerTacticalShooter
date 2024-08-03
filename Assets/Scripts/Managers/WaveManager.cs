using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class WaveManager : Singleton<WaveManager>
{
    [Header("Levels")]
    public List<Level> levels = new List<Level>();
    public GameObject currentSpawnArea;

    [Header("Wave Statistics")]
    public int currentWave;
    public float spawnInterval;
    public float enemiesKilled;
    public int currentLevel;

    //CAMERA STUFF
    private GameObject clientCamera;
    private CameraShake cameraShake;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //find player
        player = GameObject.FindWithTag("Player");
        //Camera stuff
        //clientCamera = ClientCamera.Instance.gameObject;
        //cameraShake  = ClientCamera.Instance.cameraShake;

        //START THE FIRST WAVE
        currentWave = 0; //always start on wave 1, i.e INDEX 0 of the levels list
        currentLevel = 0;
        GetLevelDoor(null);
        //StartWave(levels[currentLevel].waves[currentWave]);
    }
    //LOADING THE CURRENT SPAWN AREA IN ScenePartLoader.cs
    //STARTING THE CURRENT LEVEL IN ScenePartLoader.cs

    // Update is called once per frame
    bool endedPreviousWave = false;
    bool firstWave = true;
    public bool toBuffer = true;
    void Update()
    {
        if(currentLevel >= levels.Count)
        {
            UIManager.Instance.ShowVictory();
            Destroy(this.gameObject);
            return;
        }
        if (toBuffer)
        {
            return;
        }
        if (currentSpawnArea == null)
        {
            Debug.LogError("currentSpawnArea is null");
            return;
        }

        if (firstWave)
        {
            StartWave(levels[currentLevel].waves[currentWave]);
            Debug.LogError("first wave spawned.");
            firstWave = false;
        }

        //Debug.LogError(levels[currentLevel].waves.Count);
        //Debug.LogError("You need to kill: " + levels[currentLevel].waves[currentWave].AmtEnemies() + " enemies to advance to the next wave.");
        if (enemiesKilled >= levels[currentLevel].waves[currentWave].AmtEnemies() && (currentWave+1 <= (levels[currentLevel].waves.Count)))
        {
            endedPreviousWave = true;
            enemiesKilled = 0f;
            if (endedPreviousWave)
            {
                levels[currentLevel].waves[currentWave].MarkComplete(true);
                if ((currentWave+1) == levels[currentLevel].waves.Count-1)
                {
                    currentWave++;
                    //Debug.LogError("calls last wave because " + (currentWave+1) + " and " + (currentWave + 1) + " is equal to " + levels[currentLevel].waves.Count);
                    StartWave(levels[currentLevel].waves[currentWave]);
                    return;
                }
                else if(currentWave+2 <= levels[currentLevel].waves.Count-1)
                {
                    currentWave++;
                    //Debug.LogError("calls next wave because the next wave would be" + (currentWave + 1) + " and " + (currentWave + 1) + "is less than or equal to " + (levels[currentLevel].waves.Count-1));
                    endedPreviousWave = false;

                    //camera shake for wave start
                    if (cameraShake)
                        cameraShake.enabled = true;

                    StartWave(levels[currentLevel].waves[currentWave]);
                }
            }
        }
        else if(CheckLevelCompletion())
        {
            toBuffer = true;
            currentLevel++;
            currentWave = 0;
            Debug.LogError("Completed this level...");
            firstWave = true;
            SetLevelDoor(false);
            //SPGameManager.Instance.EndedLevel();
        }
    }

    public void SetLevelDoor(bool set)
    {
        levels[currentLevel].levelDoor.SetActive(set);
    }

    public void StartWave(Wave wave)
    {
        //wave = levels[currentLevel - 1].waves[currentWave - 1];
        Debug.LogError("Starting Level " + (currentLevel+1) + ", Wave " + (currentWave+1));
        UIManager.Instance.ChangeWaveNumber(currentWave + 1);



        wave.GenerateEnemies(currentSpawnArea);
        return;
    }

    private bool CheckLevelCompletion()
    {
        //Debug.LogError("Checking level completion...");
        bool complete = true;
        foreach(Wave wave in levels[currentLevel].waves)
        {
            if (!wave.IsComplete()) return false;
        }
        return complete;
    }

    public void GetLevelDoor(GameObject levelDoor)
    {
        levels[currentLevel].levelDoor = levelDoor;
    }

    public void ResetWaveData()
    {
        foreach(Wave wave in levels[currentLevel].waves)
        {
            wave.ResetData();
        }
    }
}