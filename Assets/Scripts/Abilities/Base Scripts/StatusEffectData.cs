using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/Game Design/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    //GENERIC
    public string Name;
    public StatusEffectTypes statusEffectType;
    public float  valueOverTime;
    public float  movementPenalty;
    public float  tickSpeed;
    public float  activeTime;
    public bool isDOT;
    public bool isSlow;


    //BASE EFFECT STUFF WE'LL MOVE LATER
    public float fireRate;
    public float fireRange;
    public float reloadTime;
    public float damageBuff;

    public int magSize;
    public int numOfBulletsPerShot;
    public int bonusPointsPerShot;


    //public ParticleSystem EffectParticles;
}
