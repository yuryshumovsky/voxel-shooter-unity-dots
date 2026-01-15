using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Input.Services
{
    public interface IInputService
    {
        float GetVerticalAxis();
        float GetHorizontalAxis();
        bool HasAxisInput();

        bool GetLeftMouseButtonDown();
        Vector2 GetScreenMousePosition();
        bool GetLeftMouseButtonUp();
    }
}