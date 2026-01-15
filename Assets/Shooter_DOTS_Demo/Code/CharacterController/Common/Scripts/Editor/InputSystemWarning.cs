namespace Shooter_DOTS_Demo.Code.CharacterController.Common.Scripts.Editor
{
#if !ENABLE_INPUT_SYSTEM
    [InitializeOnLoad]
    class InputSystemWarning
    {
        static InputSystemWarning()
        {
            Debug.LogWarning("Warning: The Standard Characters use the \"Input System\" package for input handling. Character control input will not work until the \"Input System\" package has been imported.");
        }
    }
#endif
}
