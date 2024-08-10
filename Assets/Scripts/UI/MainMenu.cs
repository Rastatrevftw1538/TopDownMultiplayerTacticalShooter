using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject loadingInterface;
    public AudioClip clickSound;
    public AudioClip clickPlaySound;
    public AudioClip hoverSound;
    public Image loadingProgressBar;
    private AudioSource audioSource;
    //List of the scenes to load from Main Menu
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void StartGame()
    {
        HideMenu();
        ShowLoadingScreen();
        //Load the Scene asynchronously in the background
        scenesToLoad.Add(SceneManager.LoadSceneAsync("ArenaSinglePlayer"));
        //Additive mode adds the Scene to the current loaded Scenes, in this case Gameplay scene
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Level1", LoadSceneMode.Additive));
        StartCoroutine(LoadingScreen());
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound, 0.4f);
    }

    public void PlayClickPlaySound()
    {
        audioSource.PlayOneShot(clickSound, 0.4f);
    }

    public void PlayHoverSound()
    {
        audioSource.PlayOneShot(hoverSound, 0.4f);
    }

    public void ReturnToMenu()
    {
        HideMenu();
        ShowLoadingScreen();
        StartCoroutine(LoadingScreen());
    }

    public void StartGameSO()
    {
        HideMenu();
        ShowLoadingScreen();
        //Load the Scene asynchronously in the background
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Gameplay"));
        //Additive mode adds the Scene to the current loaded Scenes, in this case Gameplay scene
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Level01Part01", LoadSceneMode.Additive));
        StartCoroutine(LoadingScreen());
    }

    public void HideMenu()
    {
        menu.SetActive(false);
    }

    public void ShowLoadingScreen()
    {
        loadingInterface.SetActive(true);
    }

    IEnumerator LoadingScreen()
    {
        float totalProgress=0;
        //Iterate through all the scenes to load
        for(int i=0; i<scenesToLoad.Count; ++i)
        {
            while (!scenesToLoad[i].isDone)
            {
                //Adding the scene progress to the total progress
                totalProgress += scenesToLoad[i].progress;
                //the fillAmount needs a value between 0 and 1, so we devide the progress by the number of scenes to load
                loadingProgressBar.fillAmount = totalProgress/scenesToLoad.Count;
                yield return null;
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
