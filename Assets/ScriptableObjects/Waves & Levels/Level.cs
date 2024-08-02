using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level", menuName = "Levels and Waves/Level")]
public class Level : ScriptableObject
{
    public List<Wave> waves = new List<Wave>();
    public GameObject levelDoor;

    // Start is called before the first frame update
    void Start()
    {
        levelDoor = GameObject.FindWithTag("LevelDoor");
    }

    public void StartLevel(Wave wave, GameObject spawnArea)
    {
        wave.GenerateEnemies(spawnArea);
    }

    public void SetLevelDoor(bool set)
    {
        levelDoor.SetActive(set);
    }
}
