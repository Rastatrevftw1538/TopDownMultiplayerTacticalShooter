using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#region ENUMS
[System.Serializable]
public enum AbilityType
{
    DAMAGE = 0,
    HEALING = 1,
    MOVEMENT = 2,
    FOV = 3
}

[System.Serializable]
public enum PlayerEffects
{
    PLAYER = 0,
    ENEMY = 1,
    TEAM = 2
}

[System.Serializable]
public enum ActivationMethod
{
    HITSCAN = 0,
    PROJECTILE = 1,
    ON_SELF = 2,
    AURA = 3
}

[System.Serializable]
public enum AbilityAppliance
{
    INSTANT = 0,
    OVER_TIME = 1
}
#endregion

#region CLASSES
public class HealingAbility : Ability
{

}

public class DamageAbility : Ability
{

}

public class MovementAbility : Ability
{
    //public float waitTime;
}

public class FOVAbility : Ability
{

}

#endregion