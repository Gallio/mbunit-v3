using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Services.Assert
{
    /// <summary>
    /// Provides some utility functions for manipulating assertions.
    /// </summary>
    public static class AssertionUtils
    {
        /// <summary>
        /// Returns true if the assertion result type is one of the valid
        /// enum values.
        /// </summary>
        /// <param name="assertionResultType">The assertion result type to validate</param>
        /// <returns>True if the result type is valid</returns>
        public static bool IsValid(AssertionResultType assertionResultType)
        {
            return assertionResultType >= AssertionResultType.Skip && assertionResultType <= AssertionResultType.Failure;
        }

        /// <summary>
        /// Returns true if the <paramref name="assertionResultType"/> is <see cref="AssertionResultType.Success" />,
        /// <see cref="AssertionResultType.Inconclusive" />, <see cref="AssertionResultType.Ignore" />,
        /// or <see cref="AssertionResultType.Skip" />.
        /// </summary>
        /// <param name="assertionResultType">The assertion result type</param>
        /// <returns>True if the result type signifies a condition that is not a failure or error</returns>
        public static bool IsOk(AssertionResultType assertionResultType)
        {
            return assertionResultType <= AssertionResultType.Success;
        }

        /// <summary>
        /// Ensures that an assertion result has all of its requisite properties such
        /// as an associated assertion and sets them if not.
        /// </summary>
        /// <param name="result">The assertion result</param>
        internal static void EnsureAssertionResultIsWellFormed(AssertionResult result)
        {
            if (result.Assertion == null)
                result.Assertion = new Assertion("Unknown.MissingAssertion",
                    "The AssertionResult was missing its associated Assertion object when verified.", "", null);
        }
    }
}
