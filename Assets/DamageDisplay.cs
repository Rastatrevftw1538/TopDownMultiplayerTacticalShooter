using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    public TextMeshPro text;
    public Animator anim;
    public Color color;

    Color initColor;
    float initSize;
    public void Start()
    {
        if(!text) TryGetComponent<TextMeshPro>(out text);
        if(!anim) TryGetComponent<Animator>(out anim);
        //anim.Play("DamageDisplay", -1, 0f);
    }

    private void OnEnable()
    {
        anim.Play("DamageDisplay", -1, 0f);
        text.color = color;
        initColor = text.color;
        initSize = text.fontSize;
        StartCoroutine(SelfDisable());
    }

    private void OnDisable()
    {
        text.text = "";
    }

    IEnumerator SelfDisable()
    {
        //Debug.LogError("Enabled!: " + text.text);
        yield return new WaitForSeconds(1f);
        text.color = initColor;
        text.fontSize = initSize;
        this.gameObject.SetActive(false);
    }
}
