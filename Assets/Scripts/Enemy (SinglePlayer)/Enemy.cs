using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public float pointsPerHit { get; set; }
    void TakeDamage(float damage)
    {

    }
}
