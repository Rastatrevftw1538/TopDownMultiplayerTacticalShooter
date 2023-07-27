using System.Globalization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickControllerAbility : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImg;
    private Image joystickImg;
    private Vector3 inputVector;
    [SerializeField]
    public float deadZone = 0.2f;
    public float notFiringZone = 0.8f;

    [HideInInspector]
    public bool isShooting;

    private Quaternion rotation;

    private float joystickAngle;

    private void Start()
    {
        bgImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos -= bgImg.rectTransform.rect.position;

            float x = (pos.x / bgImg.rectTransform.rect.width) * 2 - 1;
            float y = (pos.y / bgImg.rectTransform.rect.height) * 2 - 1;

            inputVector = new Vector3(x, y, 0);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;


            // Rotate the joystick handle to face the angle

            if (inputVector.magnitude < deadZone){
                bgImg.color = Color.white;
                inputVector = Vector2.zero;
            }
            // If the input vector is outside the not-firing zone, set isShooting to true
            
            if(inputVector.magnitude > notFiringZone){
                isShooting = true;
                bgImg.color = Color.red;
            }
            else{
                bgImg.color = Color.green;
                isShooting = false;
            }
            // Get the angle of the input vector
            joystickAngle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg;

            joystickImg.rectTransform.anchoredPosition = new Vector2(inputVector.x * (bgImg.rectTransform.rect.width / 3), inputVector.y * (bgImg.rectTransform.rect.height / 3));
        }

        // Update the rotation of the player based on the input vector
        rotation = Quaternion.Euler(0,0,-1*(joystickAngle));
    }

    //When you push the joystick
    public virtual void OnPointerDown(PointerEventData ped)
    {
        
        OnDrag(ped);
        //this.GetComponent<BlinkAbility>().MoveCharacter(this.transform.position, this.transform.forward);
    }

    //When you don't push the joystick
    public virtual void OnPointerUp(PointerEventData ped)
    {
        bgImg.color = Color.white;
        //inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
        isShooting = false;
        //EventSystem.current.SetSelectedGameObject(null);
    }

    public Quaternion GetRotationInput()
    {
        return rotation;
    }
}
