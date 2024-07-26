using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SPGameManager : Singleton<SPGameManager>
{
    //change to interface enemy later, and do custom editor stuff
    private static List<Wave> waves = new List<Wave>();
    public struct Wave
    {
        public List<GameObject> _spawnAreas;
        public List<GameObject> _enemies;
        public float _amtEnemies;
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
            _amtEnemies = spawnAreas.Count * enemies.Count;
        }

        public void SpawnEnemies(GameObject currentSpawn)
        {
            foreach (GameObject enemy in _enemies)
            {
                GameObject.Instantiate(enemy, currentSpawn.transform);
                _amtEnemies++;
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
    public List<GameObject> wave1SpawnAreas = new List<GameObject>();
    public List<GameObject> wave2SpawnAreas = new List<GameObject>();
    public List<GameObject> wave3SpawnAreas = new List<GameObject>();

    public List<GameObject> wave1Enemies = new List<GameObject>();
    public List<GameObject> wave2Enemies = new List<GameObject>();
    public List<GameObject> wave3Enemies = new List<GameObject>();

    [Header("Temp UI")]
    public GameObject victoryScreen;

    void OnGUI()
    {

    }

    private GameObject player;
    Wave WaveOne;
    Wave WaveTwo;
    Wave WaveThree;
    // Start is called before the first frame update
    void Start()
    {
        //find player
        player = GameObject.Find("Player - SinglePlayer");

        //going to clean this up DONT WORRY TREVOR
        WaveOne._spawnAreas = wave1SpawnAreas;
        WaveOne._enemies = wave1Enemies;

        WaveTwo._spawnAreas = wave2SpawnAreas;
        WaveTwo._enemies = wave2Enemies;

        WaveThree._spawnAreas = wave3SpawnAreas;
        WaveThree._enemies = wave3Enemies;

        waves.Add(WaveOne);
        waves.Add(WaveTwo);
        waves.Add(WaveThree);

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
        if(enemiesKilled == waves[currentWave - 1].AmtEnemies())
        {
            endedPreviousWave = true;
            if (endedPreviousWave)
            {
                StartWave(EndedWave());
                endedPreviousWave = false;
            }
        }
    }

    async void StartWave(Wave wave)
    {
        wave = waves[currentWave - 1];
        await Task.Run(() => EndedWave());

        //If there are still waves to spawn,
        if (currentWave <= waves.Count)
        {
            //for every spawn point area in this wave,
            foreach (GameObject spawnPoint in wave._spawnAreas)
            {
                //spawn those enemies, in each of the spawn points
                wave.SpawnEnemies(spawnPoint);
            }
            currentWave++;
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
        victoryScreen.SetActive(true);
        Debug.LogError("Ended Level (all waves are complete for this scene.");
    }
}
