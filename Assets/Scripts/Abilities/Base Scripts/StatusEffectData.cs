using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/Game Design/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string Name;
    public float  valueOverTime;
    public float  movementPenalty;
    public float  tickSpeed;
    public float  activeTime;
    public bool   isDOT;
    public bool   isSlow;
    public float  damageBuff;

    //public ParticleSystem EffectParticles;
}
