using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UiShow : NetworkBehaviour
{
    public Transform target; // Reference to the character's transform
    public GameObject cameraHolderPrefab;

    private GameObject cameraHolder;
    public GameObject internalUI;

    public override void OnStartAuthority()
    {
        cameraHolder = Instantiate(cameraHolderPrefab,this.transform.parent);
        cameraHolder.SetActive(true);
        internalUI.SetActive(true);
    }
    private void Update()
    {
        cameraHolder.transform.position = transform.position;
        //transform.LookAt(targetPosition);
    }
}