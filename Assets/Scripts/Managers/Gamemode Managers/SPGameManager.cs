using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPGameManager : MonoBehaviour
{
    public enum Wave
    {
        One = 1,
        Two = 2,
        Three = 3
    };

    [Header("Spawn Areas & Waves")]
    public Wave wave;
    public List<GameObject> spawnAreas = new List<GameObject>();
    public float spawnInterval;

    public List<GameObject> wave1SpawnAreas;
    void OnGUI()
    {

    }

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        //find player
        player = GameObject.Find("Player - SinglePlayer");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnWave()
    {

    }
}
