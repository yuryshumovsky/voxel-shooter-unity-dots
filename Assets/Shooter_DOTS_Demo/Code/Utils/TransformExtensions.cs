using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Utils
{
    public static class TransformExtensions
    {
        public static void DestroyAllChildren(this GameObject go)
        {
            foreach (Transform transform in go.transform)
            {
                UnityEngine.Object.Destroy(transform.gameObject);
            }
        }
    }
}