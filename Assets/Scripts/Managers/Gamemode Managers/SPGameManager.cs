using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SPGameManager : Singleton<SPGameManager>
{
    //change to interface enemy later, and do custom editor stuff
    private static List<Wave> waves = new List<Wave>();
    [System.Serializable]
    public struct Wave
    {
        public List<GameObject> _spawnAreas;
        public List<GameObject> _enemies;
        public float _amtEnemies;
        public float _waveCost;
        //List<IEnemy> _enemies;
        /*Wave(List<GameObject> spawnAreas, List<IEnemy> enemies)
        {
            _spawnAreas = spawnAreas;
            _enemies = enemies;
        }*/


        Wave(List<GameObject> spawnAreas, List<GameObject> enemies)
        {
            _spawnAreas = spawnAreas;
            _enemies = enemies;
            _amtEnemies = 0f;
            _waveCost = 0f;
        }

        public void SpawnEnemies(GameObject currentSpawn)
        {
            foreach (GameObject enemy in _enemies)
            {
                if (currentSpawn != null)
                {
                    //look into Random.insideUnitSphere
                    GameObject.Instantiate(enemy, currentSpawn.transform);
                    _amtEnemies++;
                }
            }
        }

        public float AmtEnemies()
        {
            if (_amtEnemies == 0f)
            {
                _amtEnemies = _spawnAreas.Count * _enemies.Count;
            }
            return _amtEnemies;
        }
    }
    
    [Header("Spawn Areas & Waves")]
    public int currentWave;
    //public List<GameObject> spawnAreas = new List<GameObject>();
    public float spawnInterval;
    public float enemiesKilled;

    //going to make dynamic instead of hardcoding each wave (time crunch for now)
    /*public List<GameObject> wave1SpawnAreas = new List<GameObject>();
    public List<GameObject> wave2SpawnAreas = new List<GameObject>();
    public List<GameObject> wave3SpawnAreas = new List<GameObject>();
    public List<GameObject> wave4SpawnAreas = new List<GameObject>();

    public List<GameObject> wave1Enemies = new List<GameObject>();
    public List<GameObject> wave2Enemies = new List<GameObject>();
    public List<GameObject> wave3Enemies = new List<GameObject>();
    public List<GameObject> wave4Enemies = new List<GameObject>();*/

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

        //going to clean this up DONT WORRY TREVOR
        /*WaveOne._spawnAreas = wave1SpawnAreas;
        WaveOne._enemies = wave1Enemies;

        WaveTwo._spawnAreas = wave2SpawnAreas;
        WaveTwo._enemies = wave2Enemies;

        WaveThree._spawnAreas = wave3SpawnAreas;
        WaveThree._enemies = wave3Enemies;

        WaveFour._spawnAreas =  wave4SpawnAreas;
        WaveFour._enemies = wave4Enemies;

        waves.Add(WaveOne);
        waves.Add(WaveTwo);
        waves.Add(WaveThree);
        waves.Add(WaveFour);*/

        //Camera stuff
        clientCamera = ClientCamera.Instance.gameObject;
        cameraShake  = ClientCamera.Instance.cameraShake;

        //START THE FIRST WAVE
        currentWave = 1; //always start on wave 1
        StartWave(waves[currentWave - 1]);

        Debug.LogError("Starting Wave one");
    }

    // Update is called once per frame
    bool endedPreviousWave = false;
    void Update()
    {
        /*if (enemiesKilled == WaveOne.AmtEnemies())
         {
             endedPreviousWave = true;
             if (endedPreviousWave)
             {
                 enemiesKilled = 0f;
                 currentWave++;
                 endedPreviousWave = false;

                 //StartWave(WaveTwo);
                 EndedPhase();
                 Debug.LogError("Starting Wave Two");
                 //StartWave
             }
         }
        */
        //Debug.LogError("you need " + waves[currentWave - 1].AmtEnemies());
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

                StartWave(EndedWave());
            }
        }
    }

    async void StartWave(Wave wave)
    {
        wave = waves[currentWave - 1];
        UIManager.Instance.ChangeWaveNumber(currentWave);

        await Task.Run(() => EndedWave());

        //If there are still waves to spawn,
        if (currentWave <= waves.Count)
        {
            //for every spawn point area in this wave,
            foreach (GameObject spawnPoint in wave._spawnAreas)
            {
                //spawn those enemies, in each of the spawn points
                if(spawnPoint != null)
                    wave.SpawnEnemies(spawnPoint);
            }
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
    public float enemyCost;
}