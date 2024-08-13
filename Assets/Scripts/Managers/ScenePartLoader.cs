using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEditor.Build.Reporting;

public enum CheckMethod
{
    Distance,
    Trigger
}
public class ScenePartLoader : MonoBehaviour
{

    public Transform player;
    public CheckMethod checkMethod;
    public float loadRange;

    //Scene state
    private bool isLoaded;
    private bool shouldLoad;
    void Awake()
    {
        //verify if the scene is already open to avoid opening a scene twice
        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == gameObject.name)
                {
                    isLoaded = true;
                }
            }
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        //Checking which method to use
        if (checkMethod == CheckMethod.Distance)
        {
            DistanceCheck();
        }
        else if (checkMethod == CheckMethod.Trigger)
        {
            TriggerCheck();
        }
    }

    void DistanceCheck()
    {
        //Checking if the player is within the range
        if (Vector2.Distance(player.position, transform.position) < loadRange)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }

    void LoadScene()
    {
        if (!isLoaded)
        {
            //Loading the scene, using the gameobject name as it's the same as the name of the scene to load
            //"Level" + SPManager.Instance.currentLevel,
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            //We set it to true to avoid loading the scene twice
            isLoaded = true;
        }
    }

    void UnLoadScene()
    {
        if (isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!WaveManager.Instance) return;
        if (scene.name == "ArenaSinglePlayer" || isLoaded || WaveManager.Instance.startedRoutine) return;
        //Debug.LogError(scene.name);

        //Debug.LogError("loaded scene called from " + this.gameObject.name);
        if(GameObject.FindGameObjectsWithTag("SpawnHolder").Length > 0)
            WaveManager.Instance.currentSpawnArea = GameObject.FindGameObjectsWithTag("SpawnHolder")[GameObject.FindGameObjectsWithTag("SpawnHolder").Length-1];

        if (GameObject.FindGameObjectsWithTag("LevelDoor").Length > 0)
        {
            WaveManager.Instance.GetLevelDoor(GameObject.FindGameObjectsWithTag("LevelDoor")[GameObject.FindGameObjectsWithTag("LevelDoor").Length - 1]);
            //Debug.LogError("level door is object called: " + GameObject.FindGameObjectsWithTag("LevelDoor")[GameObject.FindGameObjectsWithTag("LevelDoor").Length - 1].name);
        }
        else
        {
            //Debug.LogError("cannot find a level door...");
        }
        //WaveManager.Instance.StartWave(WaveManager.Instance.levels[WaveManager.Instance.currentLevel].waves[WaveManager.Instance.currentWave]);
         WaveManager.Instance.toBuffer = false;
         WaveManager.Instance.StartWave();
        //Debug.LogError("buffer set to FALSE");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = false;
        }
    }

    void TriggerCheck()
    {
        //shouldLoad is set from the Trigger methods
        if (shouldLoad)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }



}

