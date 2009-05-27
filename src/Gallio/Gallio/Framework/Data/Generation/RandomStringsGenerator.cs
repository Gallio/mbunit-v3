using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Gallio.Common.Text.RegularExpression;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generator of random <see cref="String"/> objects based on a regular expression filter mask.
    /// </summary>
    public class RandomStringsGenerator : Generator<string>
    {
        private RegexLite regex;

        /// <summary>
        /// Gets or sets regular expression pattern that serves as a filter mask.
        /// </summary>
        public string RegularExpressionPattern
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the length of the sequence of strings
        /// created by the generator.
        /// </summary>
        public int? Count
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a generator of random <see cref="String"/> objects.
        /// </summary>
        public RandomStringsGenerator()
        {
        }

        /// <inheritdoc/>
        public override IEnumerable Run()
        {
            if (RegularExpressionPattern == null)
                throw new GenerationException("The 'RegularExpressionPattern' property must be initialized.");

            if (!Count.HasValue)
                throw new GenerationException("The 'Count' property must be initialized.");

            if (Count.Value < 0)
                throw new GenerationException("The 'Count' property wich specifies the length of the sequence must be strictly positive.");

            try
            {
                regex = new RegexLite(RegularExpressionPattern);
            }
            catch (RegexLiteException exception)
            {
                throw new GenerationException(String.Format(
                    "The specified regular expression cannot be parsed ({0}).", exception.Message), exception);
            }

            return GetSequence();
        }

        private IEnumerable GetSequence()
        {
            for (int i = 0; i < Count.Value; i++)
            {
                yield return regex.GetRandomString();
            }
        }
    }
}
