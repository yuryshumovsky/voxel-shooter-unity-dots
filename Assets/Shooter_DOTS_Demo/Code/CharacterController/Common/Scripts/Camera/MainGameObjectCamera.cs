using UnityEngine;

namespace Shooter_DOTS_Demo.Code.CharacterController.Common.Scripts.Camera
{
    public class MainGameObjectCamera : MonoBehaviour
    {
        public static UnityEngine.Camera Instance;

        void Awake()
        {
            Instance = GetComponent<UnityEngine.Camera>();
        }
    }
}
