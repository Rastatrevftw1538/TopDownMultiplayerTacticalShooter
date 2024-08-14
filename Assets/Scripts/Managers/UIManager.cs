using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System;

public class UIManager : Singleton<UIManager>
{
    [Header("Display/UI")]
    public TextMeshProUGUI waveDisplay;
    public TextMeshProUGUI enemiesLeftDisplay;
    public TextMeshProUGUI pointsDisplay;
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    public UIArrowToShow uiArrowToTarget;
    public AudioClip defeatSound;
    public AudioClip victorySound;
    public GameObject powerupUI;
    private Image powerupUICd;
    private Image powerupUIIcon;
    public CinemachineImpulseListener impulseListener;
    private CinemachineVirtualCamera cinemachineCam;
    public bool shouldStartZoomed;
    //public UIArrowToShow arrowToPoint;

    public float points;

    [Header("Display Lengths")]
    public float waveFlashes;
    public float waveFlashLength;
    public Color waveFlashColor;

    [HideInInspector] public enum CooldownType
    {
        Powerup, Ability
    }

    void Start()
    {
        SetPoints(0f);
        //postProcessing = GetComponent<Volume>();
        //arrowToPoint = GetComponent<UIArrowToShow>();

        powerupUIIcon = powerupUI.transform.GetChild(0).GetComponent<Image>();
        powerupUICd = powerupUI.transform.GetChild(1).GetComponent<Image>();

        StartCoroutine(nameof(WorldZoomStart));
    }

    float abilityCd;
    float powerupCd;
    void Update()
    {
        //camera start zoom
        if (!didZoom && shouldStartZoomed)
        {
            if (cinemachineCam.m_Lens.OrthographicSize >= initialOrtho)
                cinemachineCam.m_Lens.OrthographicSize -= zoomSpeed * Time.fixedDeltaTime * Time.timeScale;
            else
            {
                cinemachineCam.m_Lens.OrthographicSize = initialOrtho;
                didZoom = true;
            }
        }

        if(powerupCd >= 0)
        {
            powerupCd -= Time.deltaTime;
            powerupUICd.fillAmount -= powerupCd * Time.deltaTime; 
        }
    }

    /*public void SetArrowTarget(GameObject arrowTrgt)
    {
        arrowToPoint.enabled = true;
        arrowToPoint.SetArrowTarget(arrowTrgt);
    }*/

    bool didZoom = false;
    float zoomSpeed = 3f;
    float initialOrtho;
    public IEnumerator WorldZoomStart()
    {
        didZoom = true;
        if (!cinemachineCam) cinemachineCam = GameObject.FindGameObjectWithTag("Cinemachine Camera").GetComponent<CinemachineVirtualCamera>();
        initialOrtho = cinemachineCam.m_Lens.OrthographicSize;
        cinemachineCam.m_Lens.OrthographicSize = 50f;

        if (WaveManager.Instance) WaveManager.Instance.toBuffer = true;
        yield return new WaitForSeconds(1f);
        if (WaveManager.Instance) WaveManager.Instance.toBuffer = false;
        didZoom = false;
    }

    public void ChangeWaveNumber(float num)
    {
        if (!waveDisplay) return;

        waveDisplay.text = "WAVE " + num;
        StartCoroutine(FlashWaveNumber());
    }

    float enemiesLeft;
    public void UpdateEnemiesLeft(float num)
    {
        if(!enemiesLeftDisplay) return;
        enemiesLeft = num;
        StartCoroutine(nameof(FlashEnemyRemaining));
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

    CooldownType lastType;
    public void StartCooldownUI(CooldownType type, Sprite icon, float cdTime)
    {
        lastType = type;
        //SET THE SPRITE
        if (type == CooldownType.Powerup)
        {
            if (icon != null)
                powerupUIIcon.sprite = icon;

            powerupCd = cdTime;
            powerupUICd.fillAmount = 1f;
        }

        Invoke(nameof(ResetCooldownUI), cdTime);
    }

    private void ResetCooldownUI()
    {
        if (lastType == CooldownType.Powerup)
        {
            powerupUIIcon = null;
        }
    }

    IEnumerator FlashEnemyRemaining()
    {
        Color tempColor = waveDisplay.color;
        enemiesLeftDisplay.text = "ENEMIES REMAINING: " + enemiesLeft;
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
        pointsDisplay.text = points.ToString();
    }

    public void SubtractPoints(float num)
    {
        if (!pointsDisplay) return;

        points -= num;
        pointsDisplay.text = points.ToString();
    }

    BPMManager bpmManager;
    public void ShowDefeat()
    {
        //if (WaveManager.Instance != null)
        //    WaveManager.Instance.ResetWaveData();
        if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        bpmManager.audioSource.Stop();

        Cursor.visible = true;
        SetPoints(0);
        if (BPMManager.Instance) BPMManager.Instance.audioSource.Stop();
        PlaySound(defeatSound, 0.3f);
        defeatScreen.SetActive(true);
        SetCameraShakeListener(false);
        Time.timeScale = 0.0f;
    }

    private void SetCameraShakeListener(bool set)
    {
        if (!impulseListener) return;
            impulseListener.enabled = set;
    }

    private void PlaySound(AudioClip sound, float volume = 1f)
    {
        if (!SoundFXManager.Instance || !sound) return;
        SoundFXManager.Instance.PlaySoundFXClip(sound, transform, volume);
    }

    public void ShowVictory()
    {
        /*if (WaveManager.Instance != null)
        {
            WaveManager.Instance.ResetWaveData();
    
        }*/

        if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        bpmManager.audioSource.Stop();
        SetCameraShakeListener(false);
        Cursor.visible = true;
        SetPoints(0);
        PlaySound(victorySound, 0.3f);
        victoryScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void SetWaveDisplay(bool set)
    {
        waveDisplay.gameObject.SetActive(set);
    }

    public void ResetSingletons()
    {
        Time.timeScale = 1.0f;
        if (SPGameManager.Instance != null)
            Destroy(SPGameManager.Instance.gameObject);

        if (WaveManager.Instance != null)
            Destroy(WaveManager.Instance.gameObject);

        if (PlayerHealthSinglePlayer.Instance != null)
            Destroy(PlayerHealthSinglePlayer.Instance.gameObject);

        if (PlayerScriptSinglePlayer.Instance != null)
            Destroy(PlayerScriptSinglePlayer.Instance.gameObject);

        if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        if(bpmManager) Destroy(bpmManager.gameObject);

        if (this.gameObject != null)
            Destroy(this.gameObject);

        SetCameraShakeListener(true);
    }

    public void ReturnToMainMenu()
    {
        SetCameraShakeListener(true);
        if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        if(bpmManager) bpmManager.audioSource.Play();

        Time.timeScale = 1.0f;
        foreach (Scene sceneLoaded in SceneManager.GetAllScenes())
            SceneManager.UnloadSceneAsync(sceneLoaded);

        SceneManager.LoadSceneAsync("MainMenuSP", LoadSceneMode.Single);
        //MainMenu mainMenu = GameObject.FindObjectOfType<MainMenu>();
        //mainMenu.ReturnToMenu();

        ResetSingletons();
    }

    public void ResetScene()
    {
        SetCameraShakeListener(true);
        if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        if (bpmManager) bpmManager.audioSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowUIArrow(Transform targetTransform)
    {
        uiArrowToTarget.Show(targetTransform);
    }

    public void HideUIArrow()
    {
        uiArrowToTarget.Hide();
    }
}

