// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// <para>
    /// A marker is a hidden tag that labels its contents with a semantic class.
    /// It is roughly equivalent in operation to an HTML "span" tag.  Various tools
    /// may inspect the markers and modify the presentation accordingly.
    /// </para>
    /// <para>
    /// Several standard marker classes are provided but you may also define your own.
    /// </para>
    /// </summary>
    [Serializable]
    public struct Marker : IEquatable<Marker>
    {
        private readonly string @class;

        /// <summary>
        /// Standard marker class for assertion failures.
        /// </summary>
        public const string AssertionFailureClass = "AssertionFailure";

        /// <summary>
        /// Standard marker class for exceptions including their details.
        /// </summary>
        public const string ExceptionClass = "Exception";

        /// <summary>
        /// Standard marker class for exception types reported as part of exception details.
        /// </summary>
        public const string ExceptionTypeClass = "ExceptionType";

        /// <summary>
        /// Standard marker class for exception messages reported as part of exception details.
        /// </summary>
        public const string ExceptionMessageClass = "ExceptionMessage";

        /// <summary>
        /// Standard marker class for stack traces.
        /// </summary>
        public const string StackTraceClass = "StackTrace";

        /// <summary>
        /// Standard marker class for fixed width output such as that from a console or structured table.
        /// </summary>
        public const string MonospaceClass = "Monospace";

        /// <summary>
        /// Standard marker class for content that should be displayed with a highlight.
        /// </summary>
        public const string HighlightClass = "Highlight";

        /// <summary>
        /// Standard marker class for content that represents added content in a diff.
        /// </summary>
        public const string DiffAdditionClass = "DiffAddition";

        /// <summary>
        /// Standard marker class for content that represents deleted content in a diff.
        /// </summary>
        public const string DiffDeletionClass = "DiffDeletion";

        /// <summary>
        /// Standard marker class for content that represents changed content in a diff.
        /// </summary>
        public const string DiffChangeClass = "DiffChange";

        /// <summary>
        /// Standard marker for assertion failures.
        /// </summary>
        public static Marker AssertionFailure
        {
            get { return new Marker(AssertionFailureClass); }
        }

        /// <summary>
        /// Standard marker for exceptions including their details.
        /// </summary>
        public static Marker Exception
        {
            get { return new Marker(ExceptionClass); }
        }

        /// <summary>
        /// Standard marker for exception types reported as part of exception details.
        /// </summary>
        public static Marker ExceptionType
        {
            get { return new Marker(ExceptionTypeClass); }
        }

        /// <summary>
        /// Standard marker for exception messages reported as part of exception details.
        /// </summary>
        public static Marker ExceptionMessage
        {
            get { return new Marker(ExceptionMessageClass); }
        }

        /// <summary>
        /// Standard marker for stack traces.
        /// </summary>
        public static Marker StackTrace
        {
            get { return new Marker(StackTraceClass); }
        }

        /// <summary>
        /// Standard marker for fixed width output such as that from a console or structured table.
        /// </summary>
        public static Marker Monospace
        {
            get { return new Marker(MonospaceClass); }
        }

        /// <summary>
        /// Standard marker for content that should be displayed with a highlight.
        /// </summary>
        public static Marker Highlight
        {
            get { return new Marker(HighlightClass); }
        }

        /// <summary>
        /// Standard marker for content that represents added content in a diff.
        /// </summary>
        public static Marker DiffAddition
        {
            get { return new Marker(DiffAdditionClass); }
        }

        /// <summary>
        /// Standard marker for content that represents deleted content in a diff.
        /// </summary>
        public static Marker DiffDeletion
        {
            get { return new Marker(DiffDeletionClass); }
        }

        /// <summary>
        /// Standard marker for content that represents changed content in a diff.
        /// </summary>
        public static Marker DiffChange
        {
            get { return new Marker(DiffChangeClass); }
        }

        /// <summary>
        /// Creates a marker.
        /// </summary>
        /// <param name="class">The marker class</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="class"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="class"/> is not a valid identifier.  <seealso cref="ValidateClass"/></exception>
        public Marker(string @class)
        {
            ValidateClass(@class);
            this.@class = @class;
        }

        /// <summary>
        /// Gets the marker class.
        /// </summary>
        public string Class
        {
            get { return @class; }
        }

        /// <summary>
        /// Verifies that the parameter is a valid marker class identifier.
        /// </summary>
        /// <param name="class">The class</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="class"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="class"/> is empty or contains characters
        /// other than letters, digits and underscores</exception>
        public static void ValidateClass(string @class)
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

        /// <inheritdoc />
        public override string ToString()
        {
            return @class;
        }

        /// <inheritdoc />
        public bool Equals(Marker other)
        {
            return @class == other.@class;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Marker && Equals((Marker)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return @class.GetHashCode();
        }

        /// <inheritdoc />
        public static bool operator ==(Marker a, Marker b)
        {
            return a.Equals(b);
        }

        /// <inheritdoc />
        public static bool operator !=(Marker a, Marker b)
        {
            return !(a == b);
        }
    }
}
