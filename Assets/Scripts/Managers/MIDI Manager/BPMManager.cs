using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMManager : MonoBehaviour
{
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

    public float BPM = 30f; //BPM OF THE SONG
    public float errorWindow; //THE AMOUNT OF TIME AFTER AND BEFORE THE BEAT THAT THE PLAYER CAN SHOOT FOR A BONUS

    private float lowerRange;
    private float upperRange;

    public float percentToBeat; //KEEPS TRACK OF HOW CLOSE YOU ARE TO A BEAT

    private float BPS;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        percentToBeat = 0f;
        BPS = c_MINUTE / BPM;
        BPS = c_MINUTE / BPM;
        m_MAX = BPS;

        //BETWEENT THESE TWO VALUES, IS WHEN THE PLAYER IS GOOD TO SHOOT FOR A BONUS
        lowerRange = m_MAX - errorWindow;
        upperRange = m_MAX + errorWindow;

        percentToBeat = Mathf.Clamp(percentToBeat, m_MIN, upperRange);
    }

    //public float percentToBeat;
    public Color canClick = Color.red;

    public void FixedUpdate()
    {
        percentToBeat += Time.deltaTime;

        if (percentToBeat >= BPS)
            percentToBeat = m_MIN;

        if(percentToBeat >= lowerRange && percentToBeat <= upperRange)
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
