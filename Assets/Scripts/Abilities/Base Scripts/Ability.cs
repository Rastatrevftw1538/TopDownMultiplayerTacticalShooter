using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/GAME DESIGN/Ability Base")]
public class Ability : ScriptableObject
{
    public new string name;

    //ADD A PUBLIC GETTER, PRIVATE SETTER FOR A WAIT TIME 
    //CUSTOM UNITY EDITOR SCRIPTING TO SHOW THE WAIT-TIME IN THE INSPECTOR IF ABILITY APPLIANCE IS SET TO 'OVER-TIME'
    public float waitTime;
    public AbilityType _abilityType; 
    public PlayerEffects _playerEffects;
    public ActivationMethod _activationMethod;
    public AbilityAppliance _abilityAppliance;

    [Header("Ability Time (If instant ability, leave Active Time = 0")]
    public float activeTime;
    public float cooldownTime;

    //public List<AbilityType> abilityType = new List<AbilityType>();

    //REFERENCE TO THE PARENT GAMEOBJECT, IN MOST CASES THIS WILL BE THE PLAYER
    public virtual void Activate(GameObject parent) { }

    public virtual void BeginCooldown(GameObject parent){ }
}
