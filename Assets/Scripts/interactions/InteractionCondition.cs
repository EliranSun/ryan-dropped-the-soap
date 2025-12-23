using UnityEngine;

namespace interactions
{
    public abstract class InteractionCondition : ScriptableObject
    {
        public abstract bool Evaluate(InteractionContext context);
    }
}