using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SPGameManager : Singleton<SPGameManager>
{
    //change to interface enemy later, and do custom editor stuff
    
    [Header("Waves")]
    [NonReorderable] public List<Wave> waves = new List<Wave>();
    [Header("Wave Statistics")]
    public int currentWave;
    //public List<GameObject> spawnAreas = new List<GameObject>();
    public float spawnInterval;
    public float enemiesKilled;

    //CAMERA STUFF
    private GameObject clientCamera;
    private CameraShake cameraShake;

    private GameObject player;
    //Wave WaveOne;
    //Wave WaveTwo;
    //Wave WaveThree;
    //Wave WaveFour;

    // Start is called before the first frame update
    void Start()
    {
        //find player
        player = GameObject.Find("Player - SinglePlayer");

        //Camera stuff
        clientCamera = ClientCamera.Instance.gameObject;
        cameraShake  = ClientCamera.Instance.cameraShake;

        //START THE FIRST WAVE
        currentWave = 1; //always start on wave 1
        //waves[currentWave - 1].SpawnEnemies();
        waves[currentWave - 1].GenerateEnemies();

        Debug.LogError("Starting Wave one");
    }

    // Update is called once per frame
    bool endedPreviousWave = false;
    void Update()
    {
        Debug.LogError("you need " + waves[currentWave - 1].AmtEnemies());
        //Debug.LogError("on wave" + currentWave);
        if (enemiesKilled == waves[currentWave - 1].AmtEnemies())
        {
            endedPreviousWave = true;
            if (endedPreviousWave)
            {
                endedPreviousWave = false;
                enemiesKilled = 0f;
                currentWave++;

                //camera shake for wave start
                if(cameraShake)
                    cameraShake.enabled = true;

                //StartWave(EndedWave());
            }
        }
    }

    async void StartWave(Wave wave)
    {
        wave = waves[currentWave - 1];
        UIManager.Instance.ChangeWaveNumber(currentWave);

        //await Task.Run(() => EndedWave());

        //If there are still waves to spawn,
        if (currentWave <= waves.Count)
        {
            //wave.SpawnEnemies();
            return;
        }
        //if you completed all the waves, start the next level
        EndedLevel();
    }

    async void StartLevel()
    {
        await Task.Run(() => EndedLevel());

        //start new unity scene
    }

    private Wave EndedWave()
    {
        return waves[currentWave - 1];
    }

    private void EndedLevel()
    {
        UIManager.Instance.ShowVictory();
        Debug.LogError("Ended Level (all waves are complete for this scene.");
    }
   
}
[System.Serializable]
public struct WaveEnemy
{
    public GameObject enemyPrefab;
    public int cost;
}