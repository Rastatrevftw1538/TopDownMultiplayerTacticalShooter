using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/GAME DESIGN/Ability Base")]
public class Ability : ScriptableObject
{
    public new string name;

    public enum AbilityType
    {
        PLAYER,
        ENVIRONMENT
    }

    public enum PlayerEffects
    {
        DAMAGE,
        HEALING,
        MOVEMENT,
        FOV
    }

    public enum ActivationMethod
    {
        HITSCAN,
        PROJECTILE,
        ON_SELF
    }

    public enum AbilityAppliance
    {
        INSTANT,
        OVER_TIME
    }

    public AbilityType abilityType;
    public PlayerEffects playerEffects;
    public ActivationMethod activationMethod;
    public AbilityAppliance abilityAppliance;

    [Header("Ability Time (If instant ability, leave Active Time = 0")]
    public float activeTime;
    public float cooldownTime;

    //public List<AbilityType> abilityType = new List<AbilityType>();

    //REFERENCE TO THE PARENT GAMEOBJECT, IN MOST CASES THIS WILL BE THE PLAYER
    public virtual void Activate(GameObject parent) { }

    public virtual void BeginCooldown(GameObject parent){ }
}
