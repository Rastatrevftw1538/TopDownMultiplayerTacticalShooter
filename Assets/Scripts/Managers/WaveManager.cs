using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class WaveManager : Singleton<WaveManager>
{
    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();
    public GameObject spawnAreas;

    [Header("Wave Statistics")]
    public int currentWave;
    public float spawnInterval;
    public float enemiesKilled;

    //CAMERA STUFF
    private GameObject clientCamera;
    private CameraShake cameraShake;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //find player
        player = GameObject.Find("Player - SinglePlayer");

        //Camera stuff
        //clientCamera = ClientCamera.Instance.gameObject;
        //cameraShake  = ClientCamera.Instance.cameraShake;

        //START THE FIRST WAVE
        currentWave = 1; //always start on wave 1
        StartWave(waves[currentWave - 1]);
    }

    // Update is called once per frame
    bool endedPreviousWave = false;
    void Update()
    {
        Debug.LogError("You need to kill: " + waves[currentWave - 1].AmtEnemies() + " enemies to advance to the next wave.");
        if (enemiesKilled == waves[currentWave - 1].AmtEnemies())
        {
            endedPreviousWave = true;
            if (endedPreviousWave && currentWave <= waves.Count - 1)
            {
                endedPreviousWave = false;
                enemiesKilled = 0f;
                currentWave++;

                //camera shake for wave start
                if (cameraShake)
                    cameraShake.enabled = true;

                StartWave(waves[currentWave - 1]);
            }
            else if (currentWave <= waves.Count)
            {
                //SPGameManager.Instance.EndedLevel();
                SPGameManager gameManager = SPGameManager.Instance;
                gameManager.EndedLevel();
            }
        }
    }

    void StartWave(Wave wave)
    {
        wave = waves[currentWave - 1];
        UIManager.Instance.ChangeWaveNumber(currentWave);

        wave.GenerateEnemies(spawnAreas);
        return;
    }

    public void ResetWaveData()
    {
        foreach(Wave wave in waves)
        {
            wave.ResetData();
        }
    }

}