using UnityEngine;

namespace Mini_Games.Lockpick
{
    public class Expression : MonoBehaviour
    {
        [SerializeField] public ExpressionEnum expression;
        [SerializeField] public ExpressionEnum[] matchingExpressions;
    }
}