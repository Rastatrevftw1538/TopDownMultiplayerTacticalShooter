using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SPGameManager : Singleton<SPGameManager>
{
    public float currentLevel;

    private void Start()
    {
        
    }
    private void StartLevel()
    {

    }

    public void EndedLevel()
    {
        UIManager.Instance.ShowVictory();
        Debug.LogError("Ended Level (all waves are complete for this scene.");
    }
}