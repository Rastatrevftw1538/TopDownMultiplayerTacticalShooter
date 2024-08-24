using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;
using UnityEngine.InputSystem;

public class BPMManager : MonoBehaviour
{
    public static BPMManager instance;
    [Header("BPM Indicator")]
    /*    public GameObject BPMIndicatorBar;
        public GameObject BPMIndicatorProgress;
        public GameObject BPMIndicatorToHit;*/
    public float BPM; //BPM OF THE SONG
    public Color canClick = Color.red; //JUST FOR DEBUGGING AND HELPING DESIGNERS VISUALIZE WHEN TO CLICK, WILL TRANSITION INTO MATH
    public GameObject BPMNote;
    public Transform BPMNoteSpawn;
    [HideInInspector] public bool startPlaying;

    [Header("Songs")]
    public List<AudioClip> gameSongs = new List<AudioClip>();
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public AudioLowPassFilter filter;

    [Header("Points")]
    public float pointsPerNote;
    public float pointsPerGoodNote;
    public float pointsPerPerfectNote;
    public float pointsDeductOnMiss; // leave 0 if you dont want point deduction

    [Header("Error Windows")]
    public float normErrorWindow;
    public float goodErrorWindow;
    public float perfectErrorWindow;

    [Header("Hit On Beat Feedback")]
    //public BeatScroller beatScroller;

    public GameObject
        normHitFeedback, 
        goodHitFeedback, 
        perfectHitFeedback, 
        missHitFeedback;

    //i will fix this...
    private SpriteRenderer 
        normHitFeedbackSprite,
        goodHitFeedbackSprite,
        perfectHitFeedbackSprite,
        missFeedbackSprite;

    private ParticleSystem 
        normHitFeedbackParticles, 
        goodHitFeedbackParticles, 
        perfectHitFeedbackParticles,
        missFeedbackParticles;

    //the gameobject thats actually being used
    public GameObject actualFeedback;
    private SpriteRenderer feedbackSprite;
    private GameObject feedbackParticles;

    private const float c_MINUTE = 60f;
    private float m_MIN = 0f;
    private float m_MAX;
    private float lowerRange;
    private float upperRange;
    private float percentToBeat; //KEEPS TRACK OF HOW CLOSE YOU ARE TO A BEAT
    private float BPS;

    int lastPlayedSong;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(instance);
    }

    float initVol;
    private void Start()
    {
        // gameSong = GameObject.Find("Audio Manager").GetComponent<AudioSource>().clip;
        int rand = Random.Range(0, gameSongs.Count);
        lastPlayedSong = rand;
        AudioClip randSong = gameSongs[rand];

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = randSong;
        initVol = audioSource.volume;
        audioSource.volume = 0f;

        filter = GetComponent<AudioLowPassFilter>();

        BPM = UniBpmAnalyzer.AnalyzeBpm(randSong) / 2;
        if(BPM >= 100)
        {
            BPM = BPM / 2;
        }
        //BPM = BPM / 4; //FIXING THE BPM (SOME SONGS WILL BE DIFFERENT)

        //GameObject.Find("NoteManager").GetComponent<BeatScroller>().hasStarted = true;
        percentToBeat = 0f;
        BPS = c_MINUTE / BPM;
        m_MAX = BPS;

        //BETWEENT THESE TWO VALUES, IS WHEN THE PLAYER IS GOOD TO SHOOT FOR A BONUS
        upperRange = m_MAX - normErrorWindow;
        lowerRange = m_MIN + normErrorWindow;

        currentMultiplier = 1;

        //FEEDBACK GETTERS (I WILL FIX THIS!!!)
        normHitFeedback.TryGetComponent<SpriteRenderer>(out normHitFeedbackSprite);
        normHitFeedback.transform.GetChild(0).TryGetComponent<ParticleSystem>(out normHitFeedbackParticles);

        goodHitFeedback.TryGetComponent<SpriteRenderer>(out goodHitFeedbackSprite);
        goodHitFeedback.transform.GetChild(0).TryGetComponent<ParticleSystem>(out goodHitFeedbackParticles);

        perfectHitFeedback.TryGetComponent<SpriteRenderer>(out perfectHitFeedbackSprite);
        perfectHitFeedback.transform.GetChild(0).TryGetComponent<ParticleSystem>(out perfectHitFeedbackParticles);

        missHitFeedback.TryGetComponent<SpriteRenderer>(out missFeedbackSprite);
        missHitFeedback.transform.GetChild(0).TryGetComponent<ParticleSystem>(out missFeedbackParticles);

        //FEEDBACK UI
        actualFeedback.TryGetComponent<SpriteRenderer>(out feedbackSprite);
        //actualFeedback.transform.GetChild(0).TryGetComponent<ParticleSystem>(out feedbackParticles);

        startPlaying = false;
    }

    PauseMenu pauseMenu;
    [HideInInspector] public bool hasMoved = false;
    [HideInInspector] public bool levelBeatPhase;
    [HideInInspector] public bool inDialogue;
    public void Update()
    {
        if (!BPMNoteSpawn) BPMNoteSpawn = GameObject.FindGameObjectWithTag("Note Spawn").transform;
        if (percentToBeat >= BPS && audioSource.isPlaying && !levelBeatPhase && startPlaying &&!inDialogue)
        {
            Instantiate(BPMNote, BPMNoteSpawn.position, Quaternion.identity, BPMNoteSpawn.transform);
            percentToBeat = m_MIN;
        }
        percentToBeat += Time.deltaTime * Time.timeScale;

        if (!pauseMenu) pauseMenu = GameObject.FindObjectOfType<PauseMenu>();
        if (!actualFeedback) actualFeedback = GameObject.FindGameObjectWithTag("BPM Holder");
        if(!feedbackSprite) actualFeedback.transform.parent.GetChild(0).TryGetComponent<SpriteRenderer>(out feedbackSprite);
        //if (!feedbackParticles) feedbackParticles = actualFeedback.transform.GetChild(0).gameObject;

        //wait until the player move or does something to start the music
        if (!startPlaying)
        {
            if (Input.anyKeyDown || hasMoved)
            {
                //Debug.LogError("got an input!");
                startPlaying = true;
                //beatScroller.hasStarted = true;
                audioSource.Play();
                audioSource.volume = initVol;

                // WaveManager.Instance.pauseWaves = false;
            }
        }

        //play a different song once the current one ends, and dont do this if you're paused, and dont do this if the level had just been beat.
        if (!audioSource.isPlaying && !pauseMenu._isPaused && !levelBeatPhase && !inDialogue)
        {
            RestartSong();
        }

        if (!startPlaying) return;

        //if(percentToBeat <= lowerRange || percentToBeat <= upperRange)
        if ((percentToBeat <= lowerRange && percentToBeat >= m_MIN) || (percentToBeat >= upperRange && percentToBeat <= m_MAX))
        {
            canClick = Color.green;
            //Debug.LogError("DO Click");
        }
        else
        {
            canClick = Color.red;
            //Debug.LogError("CANT Click");
        }
    }

    void RestartSong()
    {
        int rand = Random.Range(0, gameSongs.Count);
        while (rand == lastPlayedSong) rand = Random.Range(0, gameSongs.Count);
        lastPlayedSong = rand;

        AudioClip randSong = gameSongs[rand];
        audioSource.clip = randSong;
        audioSource.Play();

        BPM = UniBpmAnalyzer.AnalyzeBpm(randSong) / 2f;
        //BPM = BPM / 4; //FIXING THE BPM (SOME SONGS WILL BE DIFFERENT)

        percentToBeat = 0f;
        BPS = c_MINUTE / BPM;
        m_MAX = BPS;

        //BETWEENT THESE TWO VALUES, IS WHEN THE PLAYER IS GOOD TO SHOOT FOR A BONUS
        upperRange = m_MAX - normErrorWindow;
        lowerRange = m_MIN + normErrorWindow;
    }

    public bool CanClick()
    {
        if (canClick == Color.red)
            return false;
        else
            return true;
    }

    [Header("Multipliers")]
    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;
    public int streak = 0;

    public void NoteHit()
    {
        streak++;
        Debug.Log("current streak: " + streak);
        actualFeedback.gameObject.SetActive(true);

        //feedbackParticles.Clear();
        //feedbackParticles.Stop();
        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;
            if (multiplierTracker >= multiplierThresholds[currentMultiplier - 1])
            {
                multiplierTracker = 0;
                currentMultiplier++;
            };
        }
        UIManager.Instance.PlayMultiplierAnim(streak, CheckColor()); // play the streak animation
        // UIManager.Instance.AddPoints(points * currentMultiplier);
        //Debug.LogError("Hit on time");
        Invoke(nameof(DestroyParticles), 0.5f);
    }

    Color CheckColor()
    {
        if (streak >= 75) return Color.red;
        if (streak >= 100) return Color.green;

        switch (currentMultiplier)
        {
            case 1: return Color.white;
            case 2: return Color.cyan;
            case >= 3: return Color.yellow;
            default: return Color.white;
        }
    }

    void DestroyParticles()
    {
        if(currentParticles) Destroy(currentParticles.gameObject);
    }

    ParticleSystem currentParticles;
    public void NoteMissed(GameObject source)
    {
        streak = 0;
        multiplierTracker = 0;
        currentMultiplier = 1;
        UIManager.Instance.PlayMultiplierAnim(multiplierTracker, CheckColor());
        //Debug.LogError("Not hit on time");
        actualFeedback.gameObject.SetActive(true);

        feedbackSprite.sprite = missFeedbackSprite.sprite;
        currentParticles = Instantiate(missFeedbackParticles, feedbackSprite.transform);
        //feedbackParticles = missFeedbackParticles;
        //feedbackParticles.Play();
        Destroy(source, 0.1f);

        if (pointsDeductOnMiss == 0) return;
        UIManager.Instance.DeductPoints(pointsDeductOnMiss);
    }

    public void NormalHit()
    {
        NoteHit();
        UIManager.Instance.AddPoints(pointsPerNote * currentMultiplier);

        feedbackSprite.sprite = normHitFeedbackSprite.sprite;
        currentParticles = Instantiate(normHitFeedbackParticles, feedbackSprite.transform);
        //feedbackParticles.Play();
    }

    public void GoodHit()
    {
        NoteHit();
        UIManager.Instance.AddPoints(pointsPerGoodNote * currentMultiplier);


        feedbackSprite.sprite = goodHitFeedbackSprite.sprite;
        currentParticles = Instantiate(goodHitFeedbackParticles, feedbackSprite.transform);
        //feedbackParticles = goodHitFeedbackParticles;
        //feedbackParticles.Play();
    }

    public void PerfectHit()
    {
        NoteHit();
        UIManager.Instance.AddPoints(pointsPerPerfectNote * currentMultiplier);

        feedbackSprite.sprite = perfectHitFeedbackSprite.sprite;
        currentParticles = Instantiate(perfectHitFeedbackParticles, feedbackSprite.transform);
        //feedbackParticles = perfectHitFeedbackParticles;
    }
}