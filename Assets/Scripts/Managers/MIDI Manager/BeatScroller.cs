using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour //scrolls the arrows downwards
{
    public float beatTempo;

    public bool hasStarted = false;

	// Use this for initialization
	void Start ()
    {
        beatTempo = GameObject.Find("BPM Manager").GetComponent<BPMManager>().BPM;
        GameObject.Find("BPM Manager").GetComponent<BPMManager>().BPM = beatTempo;
        //beatTempo = beatTempo / 4f;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(hasStarted)
        {
            /*if(Input.anyKeyDown)
            {
                hasStarted = true;
            }*/
            transform.position -= new Vector3(0f, beatTempo * Time.deltaTime, 0f);
        }
            
	}
}
