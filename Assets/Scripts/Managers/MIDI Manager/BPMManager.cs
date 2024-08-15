using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

public class BPMManager : Singleton<BPMManager>
{
    [Header("BPM Indicator")]
    public GameObject BPMIndicatorBar;
    public GameObject BPMIndicatorProgress;
    public GameObject BPMIndicatorToHit;
    public Sprite defaultImg;
    public Sprite pressedImg;

    [Header("Songs")]
    public List<AudioClip> gameSongs = new List<AudioClip>();
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public AudioLowPassFilter filter;

    private SpriteRenderer spriteRenderer;

    private const float c_MINUTE = 60f;
    private float m_MIN = 0f;
    private float m_MAX;
    private float lowerRange;
    private float upperRange;
    private float percentToBeat; //KEEPS TRACK OF HOW CLOSE YOU ARE TO A BEAT
    private float BPS;

    public float BPM; //BPM OF THE SONG
    public float errorWindow; //THE AMOUNT OF TIME AFTER AND BEFORE THE BEAT THAT THE PLAYER CAN SHOOT FOR A BONUS

    private void Awake()
    {
        // gameSong = GameObject.Find("Audio Manager").GetComponent<AudioSource>().clip;
        AudioClip randSong = gameSongs[Random.Range(0, gameSongs.Count)];

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = randSong;
        audioSource.Play();

        filter = GetComponent<AudioLowPassFilter>();

        BPM = UniBpmAnalyzer.AnalyzeBpm(randSong);
        BPM = BPM/2; //FIXING THE BPM (SOME SONGS WILL BE DIFFERENT)
    }

    private void Start()
    {
        //GameObject.Find("NoteManager").GetComponent<BeatScroller>().hasStarted = true;
        percentToBeat = 0f;
        BPS = c_MINUTE / BPM;
        m_MAX = BPS;

        //BETWEENT THESE TWO VALUES, IS WHEN THE PLAYER IS GOOD TO SHOOT FOR A BONUS
        upperRange = m_MAX - errorWindow;
        lowerRange = m_MIN + errorWindow;
    }

    public Color canClick = Color.red; //JUST FOR DEBUGGING AND HELPING DESIGNERS VISUALIZE WHEN TO CLICK, WILL TRANSITION INTO MATH
    public void FixedUpdate()
    {
        percentToBeat += Time.deltaTime * Time.timeScale;

        if (percentToBeat >= BPS)
            percentToBeat = m_MIN;
        
        //if(percentToBeat <= lowerRange || percentToBeat <= upperRange)
        if((percentToBeat <= lowerRange && percentToBeat >= m_MIN) || (percentToBeat >= upperRange && percentToBeat <= m_MAX))
        {
            canClick = Color.green;
            //Debug.LogError("DO Click");
        }
        else
        {
            canClick = Color.red;
            //Debug.LogError("CANT Click");
        }

        MoveBPMIndicator();
    }

    public void Update()
    {
        
    }

    Transform BPMProgressTransform;
    private void MoveBPMIndicator()
    {
        //WIP
        /*if (!BPMProgressTransform)
            BPMProgressTransform = BPMIndicatorProgress.transform;

        BPMIndicatorProgress.transform.position = new Vector3(
            BPMProgressTransform.position.x + Time.deltaTime * BPM,
            BPMProgressTransform.position.y,
            BPMProgressTransform.position.z);*/
    }

    public bool CanClick()
    {
        if (canClick == Color.red)
            return false;
        else
            return true;
    }
}