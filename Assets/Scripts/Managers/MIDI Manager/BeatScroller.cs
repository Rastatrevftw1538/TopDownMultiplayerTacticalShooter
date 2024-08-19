using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour //scrolls the arrows downwards
{
    public float beatTempo;
	[HideInInspector] public bool hasStarted;

	// Use this for initialization
	void Start ()
    {
        //beatTempo = FindObjectOfType<BPMManager>().BPM;
        //beatTempo = beatTempo / 4f;
	}
	
	// Update is called once per frame
	void Update ()
    {
		//if(hasStarted) transform.position += new Vector3(beatTempo * Time.deltaTime, 0f, 0f);
	}
}
