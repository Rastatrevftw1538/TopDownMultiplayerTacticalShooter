using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sanford.Multimedia.Midi;

public class BPMManager : MonoBehaviour
{
    [Header("BPM Indicator")]
    public GameObject BPMIndicatorBar;
    public GameObject BPMIndicatorProgress;
    public GameObject BPMIndicatorToHit;
    private AudioSource audioSource;
    private AudioClip gameSong;
    [SerializeField]private Intervals[] intervals;
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
    private float lowerRange;
    private float upperRange;
    private float percentToBeat; //KEEPS TRACK OF HOW CLOSE YOU ARE TO A BEAT
    private float BPS;

    public float BPM; //BPM OF THE SONG
    public float errorWindow; //THE AMOUNT OF TIME AFTER AND BEFORE THE BEAT THAT THE PLAYER CAN SHOOT FOR A BONUS

    private void Awake()
    {
        _instance = this;
       // gameSong = GameObject.Find("Audio Manager").GetComponent<AudioSource>().clip;
        gameSong = GetComponent<AudioSource>().clip;
        audioSource = GetComponent<AudioSource>();
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

    public Color canClick = Color.red; //JUST FOR DEBUGGING AND HELPING DESIGNERS VISUALIZE WHEN TO CLICK, WILL TRANSITION INTO MATH
    public void FixedUpdate()
    {
        percentToBeat += Time.deltaTime;

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

    /*private void Update()
    {
        foreach(Intervals interval in intervals)
        {
            float sampledTime = (audioSource.timeSamples / (gameSong.frequency * interval.GetIntervalLength(BPM)));
            interval.CheckForNewInterval(sampledTime);
        }
    }*/

    Transform BPMProgressTransform;
    private void MoveBPMIndicator()
    {
        //WIP
        if (!BPMProgressTransform)
            BPMProgressTransform = BPMIndicatorProgress.transform;

        BPMIndicatorProgress.transform.position = new Vector3(
            BPMProgressTransform.position.x + Time.deltaTime * BPM,
            BPMProgressTransform.position.y,
            BPMProgressTransform.position.z);
    }

    public bool CanClick()
    {
        if (canClick == Color.red)
            return false;
        else
            return true;
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;
    private int _lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * _steps);
    }

    public void CheckForNewInterval(float interval)
    {
        
        if(Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
        }
    }
}