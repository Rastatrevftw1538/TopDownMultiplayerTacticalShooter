using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SPGameManager : Singleton<SPGameManager>
{
    public int currentLevel;
    public AudioClip defeatSound;
    //public List<Level> levels = new List<Level>();

    private void Awake()
    {
        currentLevel = 1;
    }
    private void Start()
    {
        
    }
    private void StartLevel()
    {
        UIManager.Instance.SetWaveDisplay(true);
        UIManager.Instance.ChangeWaveNumber(1);
    }

    public void EndedLevel()
    {
        //UIManager.Instance.ShowVictory();

        //remove the door to the next level
        //probably some camera movement polish stuff here later
        UIManager.Instance.SetWaveDisplay(false);
        WaveManager.Instance.currentLevel++;
        //levels[currentLevel - 1].door.SetActive(false);
        Debug.LogError("Ended Level (all waves are complete for this scene.");
    }

    public void WonGame()
    {
        Debug.LogError("YOU BEAT THE GAME!!");
        UIManager.Instance.ShowVictory();
    }
}