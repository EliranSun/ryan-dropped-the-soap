using System.Collections.Generic;

namespace Mini_Games.Lockpick
{
    public static class ExpressionTranslations
    {
        private static readonly Dictionary<Expressions.Expression, string> HebrewTranslations =
            new()
            {
                { Expressions.Expression.Blank, "ריק" },
                { Expressions.Expression.Sternness, "חומרה" },
                { Expressions.Expression.Indignation, "זעם" },
                { Expressions.Expression.Anger, "כעס" },
                { Expressions.Expression.Rage, "זעם" },
                { Expressions.Expression.Disdain, "בוז" },
                { Expressions.Expression.Aversion, "סלידה" },
                { Expressions.Expression.Disgust, "גועל" },
                { Expressions.Expression.Revulsion, "גועל נפש" },
                { Expressions.Expression.Concern, "דאגה" },
                { Expressions.Expression.Anxiety, "חרדה" },
                { Expressions.Expression.Fear, "פחד" },
                { Expressions.Expression.Terror, "אימה" }
            };

        /// <summary>
        ///     Gets the Hebrew translation for the given expression
        /// </summary>
        /// <param name="expression">The expression to translate</param>
        /// <returns>Hebrew translation of the expression</returns>
        public static string GetHebrewTranslation(Expressions.Expression expression)
        {
            return HebrewTranslations.TryGetValue(expression, out var translation)
                ? translation
                : expression.ToString().ToUpper(); // Fallback to English if translation not found
        }
    }
}