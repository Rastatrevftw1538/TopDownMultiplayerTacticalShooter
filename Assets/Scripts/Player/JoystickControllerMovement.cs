using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickControllerMovement : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImg;
    private Image joystickImg;
    private Vector3 inputVector;
    [SerializeField]
    private float deadZone = 0.2f;
    [SerializeField]
    private float walkZone = 0.7f;

    [HideInInspector]
    
    public bool isWalking;
    private void Start()
    {
        bgImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
        //walkZone = bgImg.rectTransform.rect.height /4;
        
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

        if (inputVector.magnitude < deadZone)
        {
            bgImg.color = Color.white;
            isWalking=true;
            inputVector = Vector2.zero;
        }
        if(inputVector.magnitude <= walkZone && inputVector.magnitude > deadZone){
            isWalking=true;
            bgImg.color = Color.green;
        }
        else{
            bgImg.color = Color.red;
            isWalking=false;
            
        }
        joystickImg.rectTransform.anchoredPosition =
        new Vector2(inputVector.x * (bgImg.rectTransform.rect.width / 3), inputVector.y * (bgImg.rectTransform.rect.height / 3));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        bgImg.color = Color.white;
        isWalking=true;
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
        //EventSystem.current.SetSelectedGameObject(null);
    }

    public float GetHorizontalInput()
    {
        return inputVector.x;
    }

    public float GetVerticalInput()
    {
        return inputVector.y;
    }
}
