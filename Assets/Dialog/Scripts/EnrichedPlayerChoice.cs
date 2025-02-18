using Character_Creator.scripts;

namespace Dialog.Scripts
{
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