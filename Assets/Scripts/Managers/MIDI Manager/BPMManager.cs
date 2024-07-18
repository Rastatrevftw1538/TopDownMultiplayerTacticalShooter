using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;


public class BPMManager : MonoBehaviour
{
    private AudioClip gameSong;
    private static BPMManager _instance;

    public static BPMManager instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("No Instance of BPM Manager");
            }
            return _instance;
        }
    }

    private const float c_MINUTE = 60f;

    private float m_MIN = 0f;
    private float m_MAX;

    public float BPM; //BPM OF THE SONG
    public float errorWindow; //THE AMOUNT OF TIME AFTER AND BEFORE THE BEAT THAT THE PLAYER CAN SHOOT FOR A BONUS

    private float lowerRange;
    private float upperRange;

    private float percentToBeat; //KEEPS TRACK OF HOW CLOSE YOU ARE TO A BEAT

    private float BPS;

    private void Awake()
    {
        _instance = this;
        //gameSong = GameObject.Find("Audio Manager").GetComponent<AudioSource>().clip;
        gameSong = GetComponent<AudioSource>().clip;
        BPM = UniBpmAnalyzer.AnalyzeBpm(gameSong);
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

    public Color canClick = Color.red;

    public void FixedUpdate()
    {
        percentToBeat += Time.deltaTime;

        if (percentToBeat >= BPS)
            percentToBeat = m_MIN;
        
        //if(percentToBeat <= lowerRange || percentToBeat <= upperRange)
        if((percentToBeat <= lowerRange && percentToBeat >= m_MIN) || (percentToBeat >= upperRange))
        {
            canClick = Color.green;
            Debug.LogError("DO Click");
        }
        else
        {
            canClick = Color.red;
            Debug.LogError("CANT Click");
        }
    }
}
