namespace Character_Creator.scripts
{
    public class EnrichedPlayerChoice
    {
        public EnrichedPlayerChoice(string choice, InteractionData originalInteraction)
        {
            Choice = choice;
            OriginalInteraction = originalInteraction;
        }

        public string Choice { get; }
        public InteractionData OriginalInteraction { get; }
    }
}