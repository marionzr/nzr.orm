using System.Text;

namespace Nzr.Orm.Core.Extensions
{
    /// <summary>
    /// Extensions for string values.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Adds a separator before each word in the text.
        /// </summary>
        /// <param name="text">The string to be modified.</param>
        /// <param name="separator">The separator to be placed between words.</param>
        /// <returns>A string with the words separated by the given separator.</returns>
        public static string AddWordSeparator(this string text, char separator)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                char current = text[i];

                if (char.IsUpper(current))
                {
                    char previous = text[i - 1];

                    if (i + 1 < text.Length)
                    {
                        char next = text[i + 1];

                        if ((previous != separator && !char.IsUpper(previous)) ||
                            (char.IsUpper(previous) &&
                             i < text.Length - 1 && !char.IsUpper(next)))
                        {
                            newText.Append(separator);
                        }
                    }
                }

                newText.Append(current);
            }

            return newText.ToString();
        }
    }
}
