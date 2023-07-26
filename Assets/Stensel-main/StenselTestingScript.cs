using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stensel;
using Stensel.Configuration;


public class StenselTestingScript : MonoBehaviour
{
    public MetricsGroup Metrics;
    public DatumReference data;
    public int var;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        var +=1;
    data.Set(var);


    }

//at the destruciton 
    void OnDestroy(){
        Recorder.Save(Metrics);
    }

}
