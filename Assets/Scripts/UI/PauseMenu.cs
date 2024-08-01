using UnityEngine.SceneManagement;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : Singleton<PauseMenu>
{

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Display()
    {
        gameObject.SetActive(true);
        PlayerScriptSinglePlayer.Instance.DisplayCursor(true);
    }

    public void OpenEpisode()
    {
        gameObject.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReturnToGame()
    {
        gameObject.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
