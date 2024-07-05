using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiShowSP : MonoBehaviour
{
    public GameObject cameraHolderPrefab;
    private int yOffset =5;

    private GameObject cameraHolder;
    private Transform cameraTransform;
    public GameObject internalUI;

    private Transform playerTransform;

    public void Start()
    {
        cameraHolder = Instantiate(cameraHolderPrefab,this.transform.parent);
        cameraHolder.SetActive(true);
        internalUI.SetActive(true);
        if(this.transform.GetComponent<PlayerScript>().PlayerDevice == PlayerScript.SetDeviceType.Mobile){
            internalUI.transform.Find("ControllerHolder").gameObject.SetActive(true);
        }
        else{
            internalUI.transform.Find("ControllerHolder").gameObject.SetActive(false);
        }
        playerTransform = this.transform.GetChild(0).GetComponent<Transform>();
        cameraTransform = cameraHolder.transform;
    }
    
    private void Update(){
    if (cameraHolder != null && playerTransform != null)
        {
            Vector3 forwardVector = playerTransform.up;
            //print("<color=purple> player rotation: "+ forwardVector.y+"</color>");
            
            if (forwardVector.y > 0){
                yOffset = 5;
                
            }
            else if(forwardVector.y < 0){
                yOffset = -5;
            }

            cameraTransform.GetChild(0).transform.localPosition = Vector3.Lerp(cameraHolder.transform.GetChild(0).transform.localPosition,new Vector3(cameraHolder.transform.GetChild(0).transform.localPosition.x,yOffset,cameraHolder.transform.GetChild(0).transform.localPosition.z),2.5f*Time.deltaTime);
            // Apply the offset to the camera holder position to keep the player centered
            cameraTransform.localPosition = new Vector3(this.transform.localPosition.x,this.transform.localPosition.y+yOffset,0f);

            // Reset the camera holder rotation to prevent unwanted rotation
            cameraTransform.localPosition = new Vector3(this.transform.position.x,this.transform.position.y,0f);
        }
    }
    
}