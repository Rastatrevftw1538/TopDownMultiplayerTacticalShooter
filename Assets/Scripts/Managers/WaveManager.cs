using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Diagnostics.Eventing.Reader;

public class WaveManager : Singleton<WaveManager>
{
    [Header("Levels")]
    public List<Level> levels = new List<Level>();
    public GameObject currentSpawnArea;
    public GameObject currentLevelDoor;
    public GameObject spawnAnimPrefab;
    //LOADING THE CURRENT SPAWN AREA IN ScenePartLoader.cs
    //STARTING THE CURRENT LEVEL IN ScenePartLoader.cs

    [Header("Debug")]
    public int currentWave;
    public float spawnInterval;
    public float enemiesKilled;
    public int currentLevel;
    public bool pauseWaves;


    //CAMERA STUFF
    private GameObject clientCamera;
    private CameraShake cameraShake;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        //START THE FIRST WAVE, FIRST LEVEL
        currentWave = 0;
        currentLevel = 0;
        enemiesKilled = 0;

        /*Invoke(nameof(StartWaveCO), 0f);*/
        //StartCoroutine(nameof(StartWaveCO));
    }

    PauseMenu pauseMenu;
    public bool toBuffer = true;
    public bool startedRoutine = false;
    void Update()
    {
        if (!pauseMenu) pauseMenu = PauseMenu.instance;
        if (toBuffer || pauseWaves)
        {
            return;
        }

        beatEnoughEnemies = (enemiesKilled >= levels[currentLevel].waves[currentWave].AmtEnemies());
        if (beatEnoughEnemies)
        {
            CheckWave();
        }
    }

    void ShowArrow()
    {
        Debug.LogError("pointing to " + currentLevelDoor.name);
        if (UIManager.Instance) UIManager.Instance.ShowUIArrow(currentLevelDoor.transform); //UI ARROW POINTING TO THE NEXT LEVEL
    }

    bool beatEnoughEnemies;
    private void CheckWave()
    {
        //if you have reached the amount of enemies killed for this wave
        if (!beatEnoughEnemies) return;

        //Debug.LogError("made it thru bc level " + currentLevel + ", wave " + currentWave + " only has to kill " + levels[currentLevel].waves[currentWave].AmtEnemies() + " enemies.");
        enemiesKilled = 0;
        //if increasing the index by 1 wont be out of bounds...
        int idxCheckW = currentWave + 1;
        int idxCheckL = currentLevel + 1;
        if(!(idxCheckW >= levels[currentLevel].waves.Count))
        {
            //then increase the wave count and start the next wave
            currentWave++;

            StartCoroutine(StartWaveCO());
        }
        //if increasing the index by 1 wont be out of bounds...
        else if (!(idxCheckL >= levels.Count))//COMPLETED A LEVEL
        {
            StartCoroutine(BeatLevelTransition());
            //that must mean you beat all the waves in the level, so increase the level count and reset the wave count
            currentWave = 0;
            currentLevel++;
            startedRoutine = false;
            toBuffer = true;

            SetLevelDoor(false);
            //NEXT LEVEL WILL BE CALLED THROUGH THE ScenePartLoader.cs
        }
        else
        {
            //if it will be out of bounds, then that means you just beat the game
            currentLevel = 0;
            currentWave = 0;
            UIManager.Instance.ShowVictory();
            Destroy(this);
            Destroy(this.gameObject);
            return;
        }
    }
    public void SetLevelDoor(bool set)
    {
         currentLevelDoor.SetActive(set);
    }

    IEnumerator BeatLevelTransition()
    {
        BPMManager.instance.audioSource.Pause();
        BPMManager.instance.levelBeatPhase = true;
        pauseMenu.canPause = false;
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(0.7f);
        pauseMenu.canPause = true;
        Time.timeScale = 1f;
        BPMManager.instance.audioSource.UnPause();
        BPMManager.instance.levelBeatPhase = false;

    }

    Animator currentAnim;
    SpriteRenderer currentSpawnSprite;
/*    private IEnumerator PlaySpawnAnim()
    {
        
    }*/

    public void StartWave()
    {
        StartCoroutine(StartWaveCO());
    }

    public float GetAmtEnemies()
    {
        return levels[currentLevel].waves[currentWave].AmtEnemies();
    }

    public void UpdateEnemiesLeft()
    {
        if (UIManager.Instance) UIManager.Instance.UpdateEnemiesLeft(GetAmtEnemies() - enemiesKilled);
    }

    public IEnumerator StartWaveCO()
    {
        startedRoutine = true;
        toBuffer = true;
        yield return new WaitForSeconds(spawnInterval);
        toBuffer = false;

        //wave = levels[currentLevel - 1].waves[currentWave - 1];
        Debug.LogError("Starting Level " + (currentLevel + 1) + ", Wave " + (currentWave + 1));
        UIManager.Instance.ChangeWaveNumber(currentWave + 1);
        UpdateEnemiesLeft();

        if (currentSpawnArea)
        {
            levels[currentLevel].PlaySpawnSound(currentSpawnArea.transform);
            levels[currentLevel].waves[currentWave].GenerateEnemies(currentSpawnArea, spawnAnimPrefab);
        }
    }

    public void GetLevelDoor(GameObject levelDoor)
    {
        try
        {
            currentLevelDoor = GameObject.FindGameObjectsWithTag("LevelDoor")[GameObject.FindGameObjectsWithTag("LevelDoor").Length - 1];
        }
        catch (Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }
}