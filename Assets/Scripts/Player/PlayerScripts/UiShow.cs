using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UiShow : NetworkBehaviour
{
    public GameObject cameraHolderPrefab;

    private GameObject cameraHolder;
    public GameObject internalUI;

    private Transform playerTransform;

    public override void OnStartAuthority()
    {
        cameraHolder = Instantiate(cameraHolderPrefab,this.transform.parent);
        cameraHolder.SetActive(true);
        internalUI.SetActive(true);
        playerTransform = this.transform.GetChild(0).GetComponent<Transform>();
    }
    private void Update(){
    if (cameraHolder != null && playerTransform != null)
        {
            // Apply the offset to the camera holder position to keep the player centered
            //cameraHolder.transform.localPosition = new Vector3(this.transform.localPosition.x,this.transform.localPosition.y,0f);

            // Reset the camera holder rotation to prevent unwanted rotation
            cameraHolder.transform.localPosition = new Vector3(this.transform.position.x,this.transform.position.y,0f);
        }
    }
}