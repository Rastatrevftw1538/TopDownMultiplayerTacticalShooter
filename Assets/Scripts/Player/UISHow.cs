using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UISHow : NetworkBehaviour
{
    public Transform target; // Reference to the character's transform
    public GameObject cameraHolder;
    public GameObject internalUI;

    public override void OnStartAuthority()
    {
        cameraHolder.SetActive(true);
        internalUI.SetActive(true);
    }
    private void Update()
    {
        cameraHolder.transform.position = transform.position;
        cameraHolder.transform.rotation = transform.rotation;
        // Make the camera look at the character's position, but only change its rotation along the Y-axis
        //Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

        //transform.LookAt(targetPosition);
    }
}