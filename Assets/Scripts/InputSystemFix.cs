using UnityEngine;
using UnityEngine.InputSystem;

// Fix issue where input actions are not registered when domain reload is disabled
public static class EnableProjectWideInputActions
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void OnSubsystemRegistration()
    {
        InputSystem.actions.Enable();
    }
}