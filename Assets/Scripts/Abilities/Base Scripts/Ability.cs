using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/GAME DESIGN/Ability Base")]
public class Ability : ScriptableObject
{
    public new string name;
    public float activeTime;
    public float cooldownTime;

    //public List<AbilityType> abilityType = new List<AbilityType>();

    //REFERENCE TO THE PARENT GAMEOBJECT, IN MOST CASES THIS WILL BE THE PLAYER
    public virtual void Activate(GameObject parent) { }
}
