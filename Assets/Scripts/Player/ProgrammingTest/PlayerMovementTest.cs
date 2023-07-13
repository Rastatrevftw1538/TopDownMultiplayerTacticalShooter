using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTest : MonoBehaviour
{
    Rigidbody2D rb;
    public float normalAcceleration;

    [HideInInspector]
    public float acceleration;
    [HideInInspector]
    public Vector2 movementInput;

    public Transform arrow;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        acceleration = normalAcceleration;
    }

    void Update(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        arrow.up = (mousePos - (Vector2)transform.position).normalized;
    }

    void FixedUpdate(){
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity += movementInput * acceleration * Time.fixedDeltaTime;
    }
}
