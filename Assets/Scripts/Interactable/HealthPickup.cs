using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, IInteractable
{
    [Header("Health Stats")]
    public float addHealth;
    public float points;
    public AudioClip pickupSound;

    [Header("Float Speed & Height")]
    public float floatSpeed;
    public float floatHeight;

    float initialY;
    void Start()
    {
        initialY = transform.position.y;
    }

    public void Interact()
    {
        if (SoundFXManager.Instance) SoundFXManager.Instance.PlaySoundFXClip(pickupSound, transform);
        player.AddHealth(addHealth);
        //Debug.LogError("Picked up health.");
        if(UIManager.Instance) UIManager.Instance.AddPoints(points);
        Destroy(gameObject);
    }

    void Update()
    {
        Vector3 pos = transform.position;
        float newY = initialY + floatHeight * Mathf.Sin(Time.time * floatSpeed);
        transform.position = new Vector3(pos.x, newY, pos.z);
    }

    PlayerHealthSinglePlayer player;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(!player) player = collision.gameObject.GetComponent<PlayerHealthSinglePlayer>();
            Interact();
        }
    }
}
