using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public float pointsPerHit { get; set; }
    static Transform target { get; set; }
    static PlayerHealthSinglePlayer player { get; set; }
    void TakeDamage(float damage)
    {

    }
}
