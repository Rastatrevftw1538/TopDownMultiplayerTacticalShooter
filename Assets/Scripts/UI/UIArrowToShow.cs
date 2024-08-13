using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIArrowToShow : MonoBehaviour
{
    public RectTransform pointerRectTransform; //the sprite/model that points to something
    public Vector3 target; //what the arrow actually points to
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite reachedSprite;
    private Image pointerImage;
    private Canvas thisCanvas;

    void Start()
    {
        //pointerRectTransform = GetComponentInChildren<RectTransform>();
        pointerImage = GetComponentInChildren<Image>();
        thisCanvas = GetComponentInParent<Canvas>();

        Hide();
    }

    // Update is called once per frame
    float borderSize = 100f;
    void Update()
    {
        if (!uiCamera)
        {
            uiCamera = GameObject.Find("ClientCamera").GetComponent<Camera>();
            thisCanvas.worldCamera = uiCamera;
        }
        Vector3 targetPositionScreenPoint = uiCamera.WorldToScreenPoint(target);
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

        if (isOffScreen)
        {
            RotatePointerTowardsTargetPosition();

            pointerImage.sprite = arrowSprite;
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

            Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        }
        else
        {
            pointerImage.sprite = reachedSprite;
            Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

            pointerRectTransform.localEulerAngles = Vector3.zero;
        }
    }

    private void RotatePointerTowardsTargetPosition()
    {
        Vector3 toPosition = target;
        Vector3 fromPosition = uiCamera.gameObject.transform.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(Vector3 targetPosition)
    {
        gameObject.SetActive(true);
        this.target = targetPosition;
    }
}
