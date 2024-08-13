using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArrowToShow : MonoBehaviour
{
    public SpriteRenderer arrow; //the sprite/model that points to something
    public GameObject arrowTarget; //what the arrow actually points to
    public Transform origin;

    public float distanceUntilTarget;
    void Start()
    {
        arrow = gameObject.GetComponent<SpriteRenderer>();
        origin = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!arrowTarget) return;
        Vector3 v = origin.position - arrowTarget.transform.position;
        arrow.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg);
    }

    public void SetArrowTarget(GameObject arrowTrgt)
    {
        if (!arrowTrgt) return;
        arrowTrgt = arrowTarget;
    }
}
