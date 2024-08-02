using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class WaveManager : Singleton<WaveManager>
{
    [Header("Levels")]
    public List<Level> levels = new List<Level>();
    private Wave currentWaveObj;
    public List<GameObject> spawnAreas = new List<GameObject>();

    [Header("Wave Statistics")]
    public int currentWave;
    public float spawnInterval;
    public float enemiesKilled;
    public int currentLevel;

    //CAMERA STUFF
    private GameObject clientCamera;
    private CameraShake cameraShake;

    private GameObject player;
    [HideInInspector]public bool startNext = false;

    // Start is called before the first frame update
    void Start()
    {
        //find player
        player = GameObject.FindWithTag("Player");

        //Camera stuff
        //clientCamera = ClientCamera.Instance.gameObject;
        //cameraShake  = ClientCamera.Instance.cameraShake;

        //START THE FIRST WAVE
        currentWave = 0; //always start on wave 1
        currentLevel = 0;
        StartWave(levels[currentLevel].waves[currentWave]);
    }

    // Update is called once per frame
    bool endedPreviousWave = false;
    void Update()
    {
        Debug.LogError("You need to kill: " + levels[currentLevel].waves[currentWave].AmtEnemies() + " enemies to advance to the next wave.");
        if (enemiesKilled == levels[currentLevel - 1].waves[currentWave - 1].AmtEnemies())
        {
            endedPreviousWave = true;
            if (endedPreviousWave && currentWave <= levels[currentLevel].waves.Count - 1 && startNext)
            {
                endedPreviousWave = false;
                enemiesKilled = 0f;
                currentWave++;

                //camera shake for wave start
                if (cameraShake)
                    cameraShake.enabled = true;

                StartWave(levels[currentLevel].waves[currentWave]);
            }
            else if (currentWave <= levels[currentLevel].waves.Count)
            {
                SPGameManager.Instance.EndedLevel();
                //SPGameManager gameManager = SPGameManager.Instance;
                //gameManager.EndedLevel();
            }
        }
    }

    void StartWave(Wave wave)
    {
        //wave = levels[currentLevel - 1].waves[currentWave - 1];
        UIManager.Instance.ChangeWaveNumber(currentWave + 1);

        wave.GenerateEnemies(spawnAreas[currentLevel]);
        return;
    }

    public void ResetWaveData()
    {
        foreach(Wave wave in levels[currentLevel].waves)
        {
            wave.ResetData();
        }
    }

}