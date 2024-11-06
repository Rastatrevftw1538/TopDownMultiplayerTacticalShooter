using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CrosshairCursor : MonoBehaviour
{
    // Start is called before the first frame update
    //public Sprite mouseSprite;
    //private Camera playerCamera;
    void Start()
    {
        Cursor.visible = false;
        //mouseSprite = GetComponent<Sprite>();

        // if(mouseSprite) Cursor.SetCursor()
    }

    // Update is called once per frame
    Camera cam;
    void Update()
    {
        if (!cam)
        {
            cam = Camera.main;
        }
        //terrible i know
        //if(!playerCamera) playerCamera = GameObject.Find("ClientCamera").GetComponent<Camera>();

        Vector3 mouseCursorPos = cam.ScreenToWorldPoint(Input.mousePosition );
        mouseCursorPos.z = cam.transform.position.z + cam.nearClipPlane;
        //transform.position = new Vector3(mouseCursorPos.x, mouseCursorPos.y, 0);
        transform.position = mouseCursorPos;
    }
}
