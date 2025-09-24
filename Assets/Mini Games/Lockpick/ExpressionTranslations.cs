using System.Collections.Generic;

namespace Mini_Games.Lockpick
{
    public static class ExpressionTranslations
    {
        private static readonly Dictionary<ExpressionEnum, string> HebrewTranslations = new Dictionary<ExpressionEnum, string>
        {
            { ExpressionEnum.Blank, "ריק" },
            { ExpressionEnum.Sternness, "חומרה" },
            { ExpressionEnum.Indignation, "זעם" },
            { ExpressionEnum.Anger, "כעס" },
            { ExpressionEnum.Rage, "זעם" },
            { ExpressionEnum.Disdain, "בוז" },
            { ExpressionEnum.Aversion, "סלידה" },
            { ExpressionEnum.Disgust, "גועל" },
            { ExpressionEnum.Revulsion, "גועל נפש" },
            { ExpressionEnum.Concern, "דאגה" },
            { ExpressionEnum.Anxiety, "חרדה" },
            { ExpressionEnum.Fear, "פחד" },
            { ExpressionEnum.Terror, "אימה" }
        };

        /// <summary>
        /// Gets the Hebrew translation for the given expression
        /// </summary>
        /// <param name="expression">The expression to translate</param>
        /// <returns>Hebrew translation of the expression</returns>
        public static string GetHebrewTranslation(ExpressionEnum expression)
        {
            return HebrewTranslations.TryGetValue(expression, out var translation)
                ? translation
                : expression.ToString().ToUpper(); // Fallback to English if translation not found
        }
    }
}
