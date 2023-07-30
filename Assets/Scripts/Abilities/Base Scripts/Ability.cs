using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/GAME DESIGN/Ability Base")]
[System.Serializable]
public class Ability : ScriptableObject
{
    public string abilityName;

    #region ENUMS
    [SerializeField] public AbilityType abilityType;
    [SerializeField] public PlayerEffects playerEffects;
    [SerializeField] public ActivationMethod activationMethod;
    [SerializeField] public AbilityAppliance abilityAppliance;
    #endregion

    #region ABILITY TIME
    //[Header("Ability Time (If instant ability, leave Active Time = 0")]
    [SerializeField] public float activeTime;
    [SerializeField] public float cooldownTime;
    [SerializeField] public float delayTime;
    [SerializeField] public bool  isInstantAbility;
    [SerializeField] public bool  hasDelay;
    #endregion

    #region WHICH ABILITY 
    [SerializeField] public Ability whichAbility;
    #endregion

    //REFERENCE TO THE PARENT GAMEOBJECT, IN MOST CASES THIS WILL BE THE PLAYER
    public virtual void Activate(GameObject parent) { }

    public virtual void BeginCooldown(GameObject parent){ }
}
