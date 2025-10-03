using UnityEngine;

namespace Mini_Games.Lockpick
{
    public class Expression : MonoBehaviour
    {
        [SerializeField] public Expressions.Expression expression;
        [SerializeField] public Expressions.Expression[] matchingExpressions;
    }
}