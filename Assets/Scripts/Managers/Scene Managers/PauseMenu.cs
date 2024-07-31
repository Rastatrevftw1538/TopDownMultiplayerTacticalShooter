using UnityEngine.SceneManagement;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Display()
    {
        gameObject.SetActive(true);
        PlayerScriptSinglePlayer.Instance.DisplayCursor(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReturnToGame()
    {
        gameObject.SetActive(false);
        PlayerScriptSinglePlayer.Instance.DisplayCursor(false);
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
