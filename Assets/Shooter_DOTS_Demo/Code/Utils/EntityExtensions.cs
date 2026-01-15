using Unity.Collections;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Utils
{
    public static class EntityExtensions
    {
        public static Entity AddComponent<T>(this Entity entity, T component)
            where T : unmanaged, IComponentData
        {
            EntityManager.AddComponentData(entity, component);
            return entity;
        }

        public static Entity SetComponentData<T>(this Entity entity, T component) where T : unmanaged, IComponentData
        {
            EntityManager.SetComponentData(entity, component);
            return entity;
        }

        public static T GetComponentData<T>(this Entity entity) where T : unmanaged, IComponentData
        {
            return EntityManager.GetComponentData<T>(entity);
        }

        public static Entity SetName(this Entity entity, string name)
        {
            if (EntityManager.Exists(entity))
            {
                EntityManager.SetName(entity, new FixedString64Bytes(name));
            }

            return entity;
        }

        public static void Destroy(this Entity entity)
        {
            if (EntityManager.Exists(entity))
            {
                EntityManager.DestroyEntity(entity);
            }
        }

        public static bool IsComponentEnabled<T>(this Entity entity)
            where T : unmanaged, IComponentData, IEnableableComponent
        {
            if (!EntityManager.Exists(entity))
                return false;

            return EntityManager.IsComponentEnabled<T>(entity);
        }

        public static Entity SetComponentEnabled<T>(this Entity entity, bool enabled)
            where T : unmanaged, IComponentData, IEnableableComponent
        {
            if (EntityManager.Exists(entity))
            {
                EntityManager.SetComponentEnabled<T>(entity, enabled);
            }

            return entity;
        }


        private static EntityManager EntityManager
        {
            get
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                return entityManager;
            }
        }
    }
}