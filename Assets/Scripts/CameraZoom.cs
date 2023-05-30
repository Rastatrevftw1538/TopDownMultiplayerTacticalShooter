using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraZoom : MonoBehaviour
{
    public Camera playerCamera;
    Weapon gun;
    public Toggle toggle;
    void Awake()
    {
        playerCamera.fieldOfView = 60f;
        gun = this.GetComponent<Weapon>();
        toggle.onValueChanged.AddListener(ZoomCamera);
    }
    public void ZoomCamera(bool isOn){
        if(isOn){
            playerCamera.fieldOfView = 60f;
        }
        else{
            playerCamera.fieldOfView = gun.zoomValue;
        }
    }
}
