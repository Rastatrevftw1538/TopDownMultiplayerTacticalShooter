/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();
    public GameObject spawnAreas;

    [Header("Wave Statistics")]
    public int currentWave;
    public float enemiesKilled;

    //CAMERA STUFF
    private GameObject clientCamera;
    private CameraShake cameraShake;

    void Start()
    {
        //Camera stuff
        clientCamera = ClientCamera.Instance.gameObject;
        cameraShake = ClientCamera.Instance.cameraShake;

        //START THE FIRST WAVE
        currentWave = 1; //always start on wave 1
        waves[0].GenerateEnemies(spawnAreas);

        Debug.LogError("Starting Wave one");
    }

    bool endedPreviousWave = false;
    void Update()
    {
        Debug.LogError("you need " + waves[currentWave - 1].AmtEnemies());
        if (enemiesKilled == waves[currentWave - 1].AmtEnemies())
        {
            endedPreviousWave = true;
            if (endedPreviousWave)
            {
                endedPreviousWave = false;
                enemiesKilled = 0f;
                currentWave++;

                //camera shake for wave start
                if (cameraShake)
                    cameraShake.enabled = true;

                StartWave(waves[currentWave - 1]);
            }
        }
    }

    void StartWave(Wave wave)
    {
        wave = waves[currentWave - 1];
        UIManager.Instance.ChangeWaveNumber(currentWave);

        //If there are still waves to spawn,
        if (currentWave <= waves.Count)
        {
            wave.GenerateEnemies(spawnAreas);
            return;
        }
        //if you completed all the waves, start the next level
        EndedLevel();
    }

    private void EndedLevel()
    {
        UIManager.Instance.ShowVictory();
        Debug.LogError("Ended Level (all waves are complete for this scene.");
    }
}
*/