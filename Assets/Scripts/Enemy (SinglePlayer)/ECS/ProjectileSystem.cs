/*using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;
using UnityEngine;

public struct ProjectileSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();

        foreach(Entity entity in allEntities)
        {
            if (entityManager.HasComponent<ProjectileComponent>(entity) && entityManager.HasComponent<ProjectileLifeTimeComponent>(entity))
            {
                //MOVE THE PROJECTILE
                LocalToWorld projectileTransform = entityManager.GetComponentData<LocalToWorld>(entity);
                ProjectileComponent projectileComponent = entityManager.GetComponentData<ProjectileComponent>(entity);

                projectileTransform.Position  += projectileComponent.speed * Time.fixedDeltaTime * projectileTransform.Right; 
            }
        }
    }

    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
    */