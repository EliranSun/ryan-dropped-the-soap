using interactions;
using UnityEngine;

namespace Mini_Games
{
    [CreateAssetMenu(menuName = "Mini Game/Active Condition")]
    public class MiniGameActiveCondition : InteractionCondition
    {
        public override bool Evaluate(InteractionContext context, bool toggle)
        {
            return toggle ? context.isMiniGameActive : !context.isMiniGameActive;
        }
    }
}