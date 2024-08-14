using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/Game Design/Ability Base")]
[System.Serializable]
public class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    [SerializeField] public StatusEffectData statusEffectData;

    #region ENUMS
    [SerializeField] public AbilityClass abilityType;
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

    #region ACTIVATION METHOD SPECIFICATIONS
    [SerializeField] public float radius;
    [SerializeField] public float projectileSpeed;
    [SerializeField] public Color bulletColor;
    #endregion

    //REFERENCE TO THE PARENT GAMEOBJECT, IN MOST CASES THIS WILL BE THE PLAYER
    public virtual void Activate(GameObject parent) { }

    public virtual void BeginCooldown(GameObject parent){ }

    #region Get Functions
    /*
    // VARIABLES
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float getActiveTime() { return activeTime; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float getCooldownTime() { return _cooldownTime; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float getDelayTime() { return _delayTime; }

    //ABILITY TIMES
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool isInstantAbility() { return _isInstantAbility; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool hasDelay() { return _hasDelay; }

    //ABILITY ENUMS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AbilityType AbilityType() { return _abilityType; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PlayerEffects PlayerEffects() { return _playerEffects; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActivationMethod ActivationMethod() { return _activationMethod; }

    public AbilityAppliance AbilityAppliance() { return _abilityAppliance; }*/
    #endregion
}
