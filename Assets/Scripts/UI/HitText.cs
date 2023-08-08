using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HitText : MonoBehaviour
{
    private TMPro.TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    public void ShowHitText(Vector3 position,Weapon gun)
    {
        // Update the text
        text.text = gun.getDamage().ToString()+"!!!";

        // Position the text on the object that was hit
        transform.position = position;
        transform.LookAt(Camera.current.transform);
    }
}
