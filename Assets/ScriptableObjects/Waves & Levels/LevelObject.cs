using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level", menuName = "Levels and Waves/Level")]
public class LevelObject : ScriptableObject
{
    public List<Wave> waves = new List<Wave>();
    public Scene levelLayout;
    public GameObject levelDoor;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartLevel(Transform transform)
    {
        //spawns the entire level
    }

    public void SetLevelDoor(bool set)
    {
        levelDoor.SetActive(set);
    }
}
