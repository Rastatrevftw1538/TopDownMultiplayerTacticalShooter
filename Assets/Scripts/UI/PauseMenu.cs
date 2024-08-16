using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : Singleton<PauseMenu>
{
    public GameObject pauseMenu;
    public KeyCode pauseKey;
    private bool _isPaused;
    private AudioSource _audioSource;
    private AudioLowPassFilter _audioLowPassFilter;
    void Awake()
    {
        //_audioSource = GetComponent<AudioSource>();
        //_audioLowPassFilter = GetComponent<AudioLowPassFilter>();
        _isPaused = false;
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey) && !_isPaused)
            Display();
        else if(Input.GetKeyDown(pauseKey) && _isPaused)
            ReturnToGame();
    }

    BPMManager bpmManager;
    public void Display()
    {
        //if (!bpmManager) bpmManager = FindObjectOfType<BPMManager>();
        //bpmManager.audioSource.Stop();

        PauseMusicEffect(true);
        _isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    public void OpenEpisode()
    {
        pauseMenu.SetActive(false);
        //SetComponents(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenuSP");
    }

    private void PauseMusicEffect(bool set)
    {
        if (!bpmManager) bpmManager = FindObjectOfType<BPMManager>();
        if (!_audioLowPassFilter) _audioLowPassFilter = bpmManager.filter;

        if (!bpmManager || !_audioLowPassFilter) return;

        if (set)
        {
            //bpmManager.audioSource.Play();
            bpmManager.audioSource.volume = 0.15f;
            _audioLowPassFilter.lowpassResonanceQ = 5f;
        }
        else
        {
            bpmManager.audioSource.volume = 0.55f;
            _audioLowPassFilter.lowpassResonanceQ = 1f;
        }
    }

    public void ReturnToGame()
    {
        //if (!bpmManager) bpmManager = FindObjectOfType<BPMManager>();
        //bpmManager.audioSource.Play();

        PauseMusicEffect(false);
        _isPaused = false;
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        //SetComponents(false);
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
