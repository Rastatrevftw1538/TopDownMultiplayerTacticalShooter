/*public class AbilityType : MonoBehaviour
{
    public virtual void Activate(GameObject parent) { }

    public virtual void BeginCooldown(GameObject parent) { }
}*/

#region ENUMS
[System.Serializable]
public enum AbilityClass
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

#region ABILITY TYPES
public class HealingAbility : Ability, IHealable
{

}

public class DamageAbility : Ability, IDamageable
{

}

public class MovementAbility : Ability, IMoveable
{
    public float speed { get; set; }
}

public class FOVAbility : Ability, ISightBlockable
{

}

#endregion

#region INTERFACES
public interface IMoveable
{
    float speed { get; set; }
}

public interface IDamageable
{

}

public interface IHealable
{

}

public interface ISightBlockable
{

}

public interface IEffectable
{
    public void ApplyEffect(StatusEffectData data);
    public void RemoveEffect();
    public void HandleEffect();
}
#endregion