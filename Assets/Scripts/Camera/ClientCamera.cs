using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraShake))]
public class ClientCamera : Singleton<ClientCamera>
{
    [HideInInspector] public CameraShake cameraShake;
    // Start is called before the first frame update
    void Start()
    {
        cameraShake = GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
