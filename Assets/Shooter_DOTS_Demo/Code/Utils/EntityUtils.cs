using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Utils
{
    public static class EntityUtils
    {
        public static Entity CreateEntity()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager entityManager = world.EntityManager;

            return entityManager.CreateEntity();
        }
    }
}