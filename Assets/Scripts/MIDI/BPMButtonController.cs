using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BPMButtonController : MonoBehaviour
{
    private Image imageRender;
    public Sprite defaultImg;
    public Sprite pressedImg;

    public KeyCode keyToPress;
    // Start is called before the first frame update
    void Start()
    {
        if (!imageRender) TryGetComponent<Image>(out imageRender);
        if (defaultImg == pressedImg)
        {
            samePng = true;
            initialColor = imageRender.color;
        }
    }

    // Update is called once per frame
    bool samePng = false;
    Color initialColor;
    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            imageRender.sprite = pressedImg;
            imageRender.color = Color.red;
        }

        if (Input.GetKeyUp(keyToPress))
        {
            imageRender.sprite = defaultImg;
            imageRender.color = initialColor;
        }
    }
}
