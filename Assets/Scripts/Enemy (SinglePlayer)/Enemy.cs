using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public float maxHealth { get; set; }
    public float pointsPerHit { get; set; }
    static Transform target { get; set; }
    static PlayerHealthSinglePlayer player { get; set; }
    public AudioClip hitSound { get; set; }
    public AudioClip movementSound { get; set; }
    public AudioClip firingSound { get; set; }
    public AudioClip defeatSound { get; set; }
    public float dropChance { get; set; }
    public GameObject dropObject {get;set;}

    void TakeDamage(float damage);
}
