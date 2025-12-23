using interactions;
using UnityEngine;

namespace Mini_Games.conditions
{
    [CreateAssetMenu(menuName = "Conditions/Zeke Employee Condition")]
    public class ZekeEmployeeCondition : InteractionCondition
    {
        public bool isGoodEmployeeFlowToggle;

        public override bool Evaluate(InteractionContext context)
        {
            return isGoodEmployeeFlowToggle ? context.isGoodEmployeeFlow : !context.isGoodEmployeeFlow;
        }
    }
}