using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Abilities/Game Design/DamageOt")]
public class DamageOverTime : DamageAbility
{
    private CircleCollider2D aura;
    
    public override void Activate(GameObject parent)
    {
        //StartDash(parent);
        AbilityHolder player = parent.GetComponent<AbilityHolder>();
        SpawnAura(player, true);
    }

    public override void BeginCooldown(GameObject parent)
    {
        AbilityHolder player = parent.GetComponent<AbilityHolder>();
        SpawnAura(player, false);
    }

    private void SpawnAura(AbilityHolder player, bool toSpawn)
    {
        if (toSpawn)
        {
            Debug.LogError("Calls Aura");
            aura = player.gameObject.AddComponent<CircleCollider2D>();
            aura.radius = 5f;
            aura.isTrigger = true;
        }
        else
        {
            Debug.LogError("Destroy Aura");
            Destroy(aura);
        }
        //IF YOU WANT THE CIRCLE TO GET BIGGER OVER TIME, USE INVOKE REPEATING
    }
}
