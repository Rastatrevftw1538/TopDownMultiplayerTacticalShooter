using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPickup : MonoBehaviour, IInteractable
{
    [Header("Pickup Components & Stats")]
    public StatusEffectData statusEffect;
    public AudioClip pickupSound;
    public float points;
    private SpriteRenderer sprite;
    string buffName;

    [Header("Float Speed & Height")]
    public float floatSpeed;
    public float floatHeight;
    public void Interact()
    {
        if (SoundFXManager.Instance) SoundFXManager.Instance.PlaySoundFXClip(pickupSound, transform);
        if (UIManager.Instance) UIManager.Instance.StartCooldownUI(UIManager.CooldownType.Powerup, sprite.sprite, statusEffect.activeTime, buffName);
        player.ApplyEffect(statusEffect);
        Debug.LogError("Picked up powerup of type: " + statusEffect.Name);
        if (UIManager.Instance) UIManager.Instance.AddPoints(points);
        Destroy(gameObject);
    }

    float initialY;
    void Start()
    {
        initialY = transform.position.y;
        TryGetComponent<SpriteRenderer>(out sprite);
        buffName = statusEffect.name;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        float newY = initialY + floatHeight * Mathf.Sin(Time.time * floatSpeed);
        transform.position = new Vector3(pos.x, newY, pos.z);
    }

    static PlayerScriptSinglePlayer player;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!player) player = collision.gameObject.GetComponent<PlayerScriptSinglePlayer>();
            Interact();
        }
    }
}
