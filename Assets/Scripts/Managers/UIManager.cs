using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Display/UI")]
    public TextMeshProUGUI waveDisplay;
    public TextMeshProUGUI pointsDisplay;
    public GameObject victoryScreen;
    public GameObject defeatScreen;

    public float points;

    [Header("Display Lengths")]
    public float waveFlashes;
    public float waveFlashLength;
    public Color waveFlashColor;

    void Start()
    {
        points = 0f;
        DontDestroyOnLoad(waveDisplay.gameObject);
        DontDestroyOnLoad(pointsDisplay.gameObject);
        DontDestroyOnLoad(victoryScreen.gameObject);
        DontDestroyOnLoad(defeatScreen.gameObject);
    }

    void FixedUpdate()
    {

    }

    public void ChangeWaveNumber(float num)
    {
        if (!waveDisplay) return;

        waveDisplay.text = "WAVE " + num;
        StartCoroutine(FlashWaveNumber());
    }

    IEnumerator FlashWaveNumber()
    {
        Color tempColor = waveDisplay.color;
        for (int i = 0; i < waveFlashes; i++)
        {
            waveDisplay.color = waveFlashColor;
            yield return new WaitForSeconds(waveFlashLength);
        }
        waveDisplay.color = tempColor;
    }

    public void AddPoints(float num)
    {
        if (!pointsDisplay) return;

        points += num;
        pointsDisplay.text = points.ToString();
    }

    public void SetPoints(float num)
    {
        if (!pointsDisplay) return;
        points = num;
    }

    public void SubtractPoints(float num)
    {
        if (!pointsDisplay) return;

        points -= num;
        pointsDisplay.text = points.ToString();
    }

    public void ShowDefeat()
    {
        //if (WaveManager.Instance != null)
        //    WaveManager.Instance.ResetWaveData();

        defeatScreen.SetActive(true);
    }

    public void ShowVictory()
    {
        /*if (WaveManager.Instance != null)
        {
            WaveManager.Instance.ResetWaveData();
    
        }*/
        SetPoints(0);

        victoryScreen.SetActive(true);
    }

    public void SetWaveDisplay(bool set)
    {
        waveDisplay.gameObject.SetActive(set);
    }

    public void ResetSingletons()
    {
        if (SPGameManager.Instance != null)
            Destroy(SPGameManager.Instance.gameObject);

        if (WaveManager.Instance != null)
            Destroy(WaveManager.Instance.gameObject);

        if (PlayerHealthSinglePlayer.Instance != null)
            Destroy(PlayerHealthSinglePlayer.Instance.gameObject);

        if (PlayerScriptSinglePlayer.Instance != null)
            Destroy(PlayerScriptSinglePlayer.Instance.gameObject);

        if (this.gameObject != null)
            Destroy(this.gameObject);
    }

    public void ReturnToMainMenu()
    {
        foreach(Scene sceneLoaded in SceneManager.GetAllScenes())
            SceneManager.UnloadSceneAsync(sceneLoaded);

        SceneManager.LoadSceneAsync("MainMenuSP", LoadSceneMode.Single);
        //MainMenu mainMenu = GameObject.FindObjectOfType<MainMenu>();
        //mainMenu.ReturnToMenu();

        ResetSingletons();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

