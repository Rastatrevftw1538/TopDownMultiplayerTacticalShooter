using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BPMButtonController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultImg;
    public Sprite pressedImg;

    public KeyCode keyToPress;
    // Start is called before the first frame update
    void Start()
    {
        if (!spriteRenderer) TryGetComponent<SpriteRenderer>(out spriteRenderer);
        if (defaultImg == pressedImg)
        {
            samePng = true;
            initialColor = spriteRenderer.color;
        }
    }

    // Update is called once per frame
    bool samePng = false;
    Color initialColor;
    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            spriteRenderer.sprite = pressedImg;
            spriteRenderer.color = Color.red;
        }

        if (Input.GetKeyUp(keyToPress))
        {
            spriteRenderer.sprite = defaultImg;
            spriteRenderer.color = initialColor;
        }
    }
}
