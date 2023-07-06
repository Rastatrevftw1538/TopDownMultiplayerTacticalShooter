using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityType : MonoBehaviour
{
    public enum TypeOfAbility
    {
        PLAYER,
        ENVIRONMENT
    }

    TypeOfAbility player = TypeOfAbility.PLAYER;
    TypeOfAbility environment = TypeOfAbility.ENVIRONMENT;
}
