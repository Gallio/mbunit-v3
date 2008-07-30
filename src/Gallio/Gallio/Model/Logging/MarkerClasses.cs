using System;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// Defines common marker classes.
    /// </summary>
    public static class MarkerClasses
    {
        /// <summary>
        /// Marks an assertion failure.
        /// </summary>
        public const string AssertionFailure = "assertionFailure";

        /// <summary>
        /// Marks an exception with all of its details.
        /// </summary>
        public const string Exception = "exception";

        /// <summary>
        /// Marks an exception type.
        /// </summary>
        public const string ExceptionType = "exceptionType";

        /// <summary>
        /// Marks an exception message.
        /// </summary>
        public const string ExceptionMessage = "exceptionMessage";

        /// <summary>
        /// Marks a stack trace.
        /// </summary>
        public const string StackTrace = "stackTrace";

        /// <summary>
        /// Marks a log trace the output from a process that was spawned.
        /// </summary>
        public const string Console = "console";

        /// <summary>
        /// Verifies that the parameter is a valid marker class identifier.
        /// </summary>
        /// <param name="class">The class</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="class"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="class"/> is empty or contains characters
        /// other than letters, digits and underscores</exception>
        public static void Validate(string @class)
        {
            if (@class == null)
                throw new ArgumentNullException("class");
            if (@class.Length == 0)
                throw new ArgumentException("Marker class name must not be empty.", "class");
            
            foreach (char c in @class)
            {
                if (!Char.IsLetterOrDigit(c) && c != '_')
                    throw new ArgumentException("Marker class name must only consist of letters, digits and underscores.", "class");
            }
        }
    }
}