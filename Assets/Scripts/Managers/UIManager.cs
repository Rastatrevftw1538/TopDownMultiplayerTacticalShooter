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
using System.Text;
using DigitalRuby.Tween;

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
    public TextMeshProUGUI powerupText;
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

    float initPowerupTextSize;
    void Start()
    {
        SetPoints(0f);
        //postProcessing = GetComponent<Volume>();
        //arrowToPoint = GetComponent<UIArrowToShow>();

        powerupUIIcon = powerupUI.transform.GetChild(0).GetComponent<Image>();
        powerupUICd = powerupUI.transform.GetChild(1).GetComponent<Image>();
        initPowerupTextSize = powerupText.fontSize;
        StartCoroutine(nameof(WorldZoomStart));
    }

    float abilityCd;
    float powerupCd;
    void Update()
    {
        if(powerupCd >= 0)
        {
            powerupCd -= Time.deltaTime;
            powerupUICd.fillAmount -= powerupCd; 
        }

        if (startFlashBuff)
        {
            powerupText.fontSize += 1f * Time.deltaTime;
        }
    }

    private void FixedUpdate()
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
    }

    /*public void SetArrowTarget(GameObject arrowTrgt)
    {
        arrowToPoint.enabled = true;
        arrowToPoint.SetArrowTarget(arrowTrgt);
    }*/

    bool didZoom = false;
    public float zoomSpeed = 10f;
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

        waveDisplay.text = $"WAVE {num}";
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
    public void StartCooldownUI(CooldownType type, Sprite icon, float cdTime, string name)
    {
        //Debug.LogError("active time of the powerup is: " + cdTime);
        lastType = type;
        //SET THE SPRITE
        if (type == CooldownType.Powerup)
        {
            powerupText.text = name;
            powerupText.gameObject.SetActive(true);
            if (icon != null)
                powerupUIIcon.sprite = icon;
            powerupUIIcon.gameObject.SetActive(true);

            powerupCd = cdTime;
            powerupUICd.fillAmount = 1f;

            StartCoroutine(nameof(FlashBuffName));
            Invoke(nameof(ResetCooldownUI), cdTime);
        }
    }

    private void ResetCooldownUI()
    {
        //Debug.LogError("called reset ui");
        powerupUIIcon.gameObject.SetActive(false);
    }

    bool startFlashBuff;
    IEnumerator FlashBuffName()
    {
        startFlashBuff = true;
        powerupText.fontSize = 1f;
        yield return new WaitForSeconds(1f);
        startFlashBuff = false;
        powerupText.gameObject.SetActive(false);
    }

    IEnumerator FlashEnemyRemaining()
    {
        StringBuilder stringBuilder = new StringBuilder();
        Color tempColor = waveDisplay.color;
        stringBuilder.Append($"ENEMIES REMAINING: {enemiesLeft}");
        enemiesLeftDisplay.text = stringBuilder.ToString();
        for (int i = 0; i < waveFlashes; i++)
        {
            waveDisplay.color = waveFlashColor;
            yield return new WaitForSeconds(waveFlashLength);
        }
        waveDisplay.color = tempColor;
    }

    public void AddPoints(float num)
    {
        if (!pointsDisplay || num == 0) return;
        StringBuilder stringBuilder = new StringBuilder();
        points += num;
        stringBuilder.Append(points);
        TweenColorText(pointsDisplay);
        pointsDisplay.text = stringBuilder.ToString();
    }

    public void DeductPoints(float num)
    {
        if (!pointsDisplay || num == 0) return;
        if (points - num < 0)
        {
            int min = 0;
            pointsDisplay.text = min.ToString(); 
            return;
        }

        StringBuilder stringBuilder = new StringBuilder();
        points -= num;
        stringBuilder.Append(points);

        pointsDisplay.text = stringBuilder.ToString();
    }

    public void SetPoints(float num)
    {
        if (!pointsDisplay) return;
        points = num;
        pointsDisplay.text = points.ToString();
    }

    //BPMManager bpmManager;
    public void ShowDefeat()
    {
        //if (WaveManager.Instance != null)
        //    WaveManager.Instance.ResetWaveData();
        //if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        BPMManager.instance.audioSource.Stop();

        Cursor.visible = true;
        SetPoints(0);
        if (BPMManager.instance) BPMManager.instance.audioSource.Stop();
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

        //if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        BPMManager.instance.audioSource.Stop();
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

    private void TweenColorText(TextMeshProUGUI TMP)
    {
        System.Action<ITween<Color>> updateColor = (t) =>
        {
            TMP.color = t.CurrentValue;
        };

        Color endColor = UnityEngine.Random.ColorHSV(0.0f, 1.0f, 0.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f);

        // completion defaults to null if not passed in
        TMP.gameObject.Tween("PointsDisplay", TMP.color, endColor, 1.0f, TweenScaleFunctions.QuadraticEaseOut, updateColor);
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

        /*if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        if(bpmManager) Destroy(bpmManager.gameObject);*/

        Destroy(BPMManager.instance.gameObject);

        if (this.gameObject != null)
            Destroy(this.gameObject);

        SetCameraShakeListener(true);
    }

    public void ReturnToMainMenu()
    {
        SetCameraShakeListener(true);
        //if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        //if(bpmManager) bpmManager.audioSource.Play();
        BPMManager.instance.audioSource.Play();

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
        //if (!bpmManager) bpmManager = GameObject.FindObjectOfType<BPMManager>().GetComponent<BPMManager>();
        //if (bpmManager) bpmManager.audioSource.Play();
        BPMManager.instance.audioSource.Play();
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

