using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour 
{
    public static PauseMenu instance;
    public GameObject pauseMenu;
    public KeyCode pauseKey;
    public bool canPause;
    [HideInInspector] public bool _isPaused;
    private AudioSource _audioSource;
    private AudioLowPassFilter _audioLowPassFilter;
    private CameraShake cameraShake;
    void Awake()
    {
        instance = this;
        TryGetComponent<AudioSource>(out _audioSource);
        //_audioLowPassFilter = GetComponent<AudioLowPassFilter>();
        _isPaused = false;
        canPause = true;
        pauseMenu.SetActive(false);
    }

    private void Start()
    {
        cameraShake = Camera.main.transform.GetComponent<CameraShake>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey) && !_isPaused && canPause)
            Display();
        else if(Input.GetKeyDown(pauseKey) && _isPaused && canPause)
            ReturnToGame();
    }

    BPMManager bpmManager;
    public void Display()
    {
        bpmManager = BPMManager.instance;
        BPMManager.instance.audioSource.Pause();

        _audioSource.Play();

        StartCoroutine(cameraShake.CustomCameraShake(0.0f, 0.0f));
        //PauseMusicEffect(true);
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
        if(UIManager.Instance)UIManager.Instance.ResetSingletons();
    }

    private void PauseMusicEffect(bool set)
    {
        /*if (!bpmManager) bpmManager = FindObjectOfType<BPMManager>();
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
        }*/
    }

    public void ReturnToGame()
    {
        _audioSource.Stop();
        Time.timeScale = 1.0f;
        BPMManager.instance.audioSource.UnPause();

        //PauseMusicEffect(false);
        _isPaused = false;
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
