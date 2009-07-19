using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gallio.Common.Normalization
{
    /// <summary>
    /// Normalization utilities.
    /// </summary>
    public static class NormalizationUtils
    {
        /// <summary>
        /// Normalizes a collection of normalizable values.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="T">The type of the values in the collection.</typeparam>
        /// <param name="collection">The collection of values to normalize, or null if none.</param>
        /// <param name="collectionFactory">The factory to use to create a new collection if needed.</param>
        /// <param name="normalize">The normalization function to apply to each value in the collection.</param>
        /// <param name="compare">The comparer to compare normalized values to determine if a change occurred during normalization.</param>
        /// <returns>The normalized collection, or null if none.  The result will
        /// be the same instance as <paramref name="collection"/> if it was already normalized.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="collectionFactory"/>,
        /// <paramref name="normalize"/> or <paramref name="compare"/> is null.</exception>
        public static TCollection NormalizeCollection<TCollection, T>(TCollection collection,
            Func<TCollection> collectionFactory, Func<T, T> normalize, EqualityComparison<T> compare)
            where TCollection : class, ICollection<T>
        {
            if (collectionFactory == null)
                throw new ArgumentNullException("collectionFactory");
            if (normalize == null)
                throw new ArgumentNullException("normalize");
            if (compare == null)
                throw new ArgumentNullException("compare");

            if (collection == null)
                return null;

            TCollection normalizedCollection = null;
            int itemIndex = 0;
            foreach (T item in collection)
            {
                T normalizedItem = normalize(item);
                if (normalizedCollection == null)
                {
                    if (! compare(item, normalizedItem))
                    {
                        normalizedCollection = collectionFactory();
                        if (itemIndex > 0)
                        {
                            int oldItemIndex = 0;
                            foreach (T oldItem in collection)
                            {
                                normalizedCollection.Add(oldItem);

                                oldItemIndex += 1;
                                if (oldItemIndex == itemIndex)
                                    break;
                            }
                        }

                        normalizedCollection.Add(normalizedItem);
                    }
                }
                else
                {
                    normalizedCollection.Add(normalizedItem);
                }

                itemIndex += 1;
            }


            return normalizedCollection ?? collection;
        }

        /// <summary>
        /// Normalizes a string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Preserves all valid characters in the string and replaces others as requested.
        /// </para>
        /// </remarks>
        /// <param name="str">The string to normalize, or null if none.</param>
        /// <param name="valid">The predicate to determine whether a character is valid.
        /// If the character consists of a surrogate pair, then the parameter will be
        /// its UTF32 value.</param>
        /// <param name="replace">The converter to provide a replacement for an invalid character.
        /// If the character consists of a surrogate pair, then the parameters will be
        /// its UTF32 value.</param>
        /// <returns>The normalized text, or null if <paramref name="str"/> was null.  The result will
        /// be the same instance as <paramref name="str"/> if it was already normalized.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="valid"/>
        /// or <paramref name="replace"/> is null.</exception>
        public static string NormalizeString(string str, Predicate<int> valid, Converter<int, string> replace)
        {
            if (valid == null)
                throw new ArgumentNullException("valid");
            if (replace == null)
                throw new ArgumentNullException("replace");

            if (str == null)
                return null;

            int length = str.Length;
            if (length == 0)
                return str;

            StringBuilder result = null;
            for (int i = 0; i < length; i++)
            {
                char c = str[i];
                if (valid(c))
                {
                    if (result != null)
                        result.Append(c);
                    continue;
                }

                int pos = i;
                int codePoint = c;
                if (char.IsHighSurrogate(c) && i + 1 < length)
                {
                    char c2 = str[i + 1];
                    if (char.IsLowSurrogate(c2))
                    {
                        i += 1;

                        codePoint = char.ConvertToUtf32(c, c2);
                        if (valid(codePoint))
                        {
                            if (result != null)
                                result.Append(c).Append(c2);
                            continue;
                        }
                    }
                }

                if (result == null)
                {
                    result = new StringBuilder(length);
                    if (pos > 0)
                        result.Append(str, 0, pos);
                }

                result.Append(replace(codePoint));
            }

            return result != null ? result.ToString() : str;
        }

        /// <summary>
        /// Normalizes a name by replacing characters that are not printable with a '?'.
        /// </summary>
        /// <param name="name">The name to normalize, or null if none.</param>
        /// <returns>The normalized name, or null if <paramref name="name"/> was null.  The result will
        /// be the same instance as <paramref name="name"/> if it was already normalized.</returns>
        public static string NormalizeName(string name)
        {
            return NormalizeString(name, IsValidNameCharacter, ReplaceInvalidCharacter);
        }

        private static bool IsValidNameCharacter(int c)
        {
            if (c >= 0x10000)
                return false;

            switch (char.GetUnicodeCategory((char) c))
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.OtherNumber:
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.OpenPunctuation:
                case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.InitialQuotePunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.MathSymbol:
                case UnicodeCategory.CurrencySymbol:
                case UnicodeCategory.ModifierSymbol:
                case UnicodeCategory.OtherSymbol:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.EnclosingMark:
                    return true;
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.Control:
                case UnicodeCategory.Format:
                case UnicodeCategory.Surrogate:
                case UnicodeCategory.PrivateUse:
                case UnicodeCategory.OtherNotAssigned:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Normalizes a name by replacing characters that are not printable ASCII with '?'.
        /// </summary>
        /// <param name="str">The string to normalize, or null if none.</param>
        /// <returns>The normalized string, or null if <paramref name="str"/> was null.  The result will
        /// be the same instance as <paramref name="str"/> if it was already normalized.</returns>
        public static string NormalizePrintableASCII(string str)
        {
            return NormalizeString(str, IsValidPrintableASCIICharacter, ReplaceInvalidCharacter);
        }

        private static bool IsValidPrintableASCIICharacter(int c)
        {
            return c >= '\u0020' && c <= '\u007e';
        }

        /// <summary>
        /// Normalizes Xml text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Preserves all characters in the following ranges: \t, \n, \r, \u0020 - \uD7FF, 
        /// \uE000 - \uFFFD, and \U00010000 - \U0010FFFF.  All other characters are replaced
        /// with '?' to indicate that they cannot be represented in Xml.
        /// </para>
        /// </remarks>
        /// <param name="text">The Xml text to normalize, or null if none.</param>
        /// <returns>The normalized Xml text, or null if <paramref name="text"/> was null.  The result will
        /// be the same instance as <paramref name="text"/> if it was already normalized.</returns>
        public static string NormalizeXmlText(string text)
        {
            return NormalizeString(text, IsValidXmlCharacter, ReplaceInvalidCharacter);
        }

        private static bool IsValidXmlCharacter(int c)
        {
            return c >= '\u0020' && c <= '\uD7FF'
                || c == '\n'
                || c == '\r'
                || c == '\t'
                || c >= '\uE000' && c <= '\uFFFD'
                || c >= 0x10000 && c <= 0x10FFFF;
        }

        private static string ReplaceInvalidCharacter(int c)
        {
            return "?";
        }
    }
}