using UnityEngine;
using UnityEngine.EventSystems;

namespace Shooter_DOTS_Demo.Code.Input.Services
{
    public class StandaloneInputService : IInputService
    {
        private UnityEngine.Camera _mainCamera;

        public UnityEngine.Camera CameraMain
        {
            get
            {
                if (_mainCamera == null && UnityEngine.Camera.main != null)
                    _mainCamera = UnityEngine.Camera.main;

                return _mainCamera;
            }
        }

        public Vector2 GetScreenMousePosition() =>
            CameraMain ? (Vector2)UnityEngine.Input.mousePosition : new Vector2();

        public bool HasAxisInput() => GetHorizontalAxis() != 0 || GetVerticalAxis() != 0;

        public float GetVerticalAxis() => UnityEngine.Input.GetAxis("Vertical");
        public float GetHorizontalAxis() => UnityEngine.Input.GetAxis("Horizontal");
        
        public bool GetLeftMouseButton() =>
            UnityEngine.Input.GetMouseButton(0) && !IsPointerOverGameObject();

        public bool GetLeftMouseButtonDown() =>
            UnityEngine.Input.GetMouseButtonDown(0) && !IsPointerOverGameObject();

        public bool GetLeftMouseButtonUp() =>
            UnityEngine.Input.GetMouseButtonUp(0) && !IsPointerOverGameObject();

        private static bool IsPointerOverGameObject() =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}