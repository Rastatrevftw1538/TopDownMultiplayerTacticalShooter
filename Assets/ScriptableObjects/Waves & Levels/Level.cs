using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level", menuName = "Levels and Waves/Level")]
public class Level : ScriptableObject
{
    public List<Wave> waves = new List<Wave>();
    public GameObject levelDoor;
    [SerializeField] private AudioClip spawnSound;
    private bool _isCompleted;

    // Start is called before the first frame update
    void Start()
    {
        //levelDoor = GameObject.FindWithTag("LevelDoor");
    }

    public void StartLevel(Wave wave, GameObject spawnArea)
    {
        wave.GenerateEnemies(spawnArea);
    }

    public void SetLevelDoor(bool set)
    {
        levelDoor.SetActive(set);
    }

    public void SetLevelCompletion(bool set)
    {
        _isCompleted = set;
    }

    public bool GetLevelCompletion()
    {
        return _isCompleted;
    }

    public void PlaySpawnSound(Transform where)
    {
        if (!SoundFXManager.Instance || !spawnSound) return;
        SoundFXManager.Instance.PlaySoundFXClip(spawnSound, where, 0.3f);
    }
}
