using Character_Creator.scripts;

public class InteractionStateService
{
    private static InteractionStateService _instance;

    private InteractionData _currentInteraction;
    public static InteractionStateService Instance => _instance ??= new InteractionStateService();

    public void SetCurrentInteraction(InteractionData interaction)
    {
        _currentInteraction = interaction;
    }

    public InteractionData GetCurrentInteraction()
    {
        return _currentInteraction;
    }
}