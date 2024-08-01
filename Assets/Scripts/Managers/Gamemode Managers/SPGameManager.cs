using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SPGameManager : Singleton<SPGameManager>
{
    public int currentLevel;

    public List<Level> levels = new List<Level>();

    private void Start()
    {
        
    }
    private void StartLevel()
    {

    }

    public void EndedLevel()
    {
        //UIManager.Instance.ShowVictory();
        
        //remove the door to the next level
        //probably some camera movement polish stuff here later
        levels[currentLevel - 1].door.SetActive(false);
        Debug.LogError("Ended Level (all waves are complete for this scene.");
    }
}