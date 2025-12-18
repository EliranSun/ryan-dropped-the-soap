namespace Dialog
{
    public class InteractionData
    {
        public readonly NarrationDialogLine DialogLine;
        public readonly InteractableObjectName InteractableObjectName;
        public readonly InteractableObjectType InteractableObjectType;
        public readonly string Name;

        public InteractionData(
            string gameObjectName,
            InteractableObjectName interactableObjectName,
            InteractableObjectType type,
            NarrationDialogLine dialogLine)
        {
            Name = gameObjectName;
            DialogLine = dialogLine;
            InteractableObjectName = interactableObjectName;
            InteractableObjectType = type;
        }
    }

    public class EnrichedPlayerChoice
    {
        public EnrichedPlayerChoice(string choice, InteractionData originalInteraction)
        {
            Choice = choice;
            OriginalInteraction = originalInteraction;
        }

        public EnrichedPlayerChoice(string choice, InteractionData originalInteraction, PlayerDataEnum playerDataType)
        {
            Choice = choice;
            OriginalInteraction = originalInteraction;
            PlayerDataType = playerDataType;
        }

        public PlayerDataEnum PlayerDataType { get; }
        public string Choice { get; }
        public InteractionData OriginalInteraction { get; }
    }
}