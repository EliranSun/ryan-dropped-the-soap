using interactions;
using UnityEngine;

namespace Mini_Games.conditions
{
    [CreateAssetMenu(menuName = "Conditions/Mini Game Active Condition")]
    public class MiniGameActiveCondition : InteractionCondition
    {
        public MiniGameName miniGameName;

        public override bool Evaluate(InteractionContext context)
        {
            if (miniGameName == MiniGameName.None)
                return !context.isMiniGameActive;

            return miniGameName.ToString().ToLower() == context.miniGameName.ToLower().Trim();
        }
    }
}