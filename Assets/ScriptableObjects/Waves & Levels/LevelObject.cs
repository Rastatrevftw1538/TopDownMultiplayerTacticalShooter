using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Levels and Waves")]
public class LevelObject : ScriptableObject
{
    public List<Wave> waves = new List<Wave>();
    public GameObject levelLayout;
    private GameObject levelDoor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartLevel(Transform transform)
    {
        //spawns the entire level
        Instantiate(levelLayout, transform);
    }

    public void SetLevelDoor(bool set)
    {
        levelDoor.SetActive(set);
    }
}
