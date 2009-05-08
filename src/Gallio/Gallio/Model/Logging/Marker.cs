// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;

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
        private readonly AttributeDictionary attributes;

        /// <summary>
        /// Standard marker class for assertion failures.
        /// </summary>
        public const string AssertionFailureClass = "AssertionFailure";

        /// <summary>
        /// Standard marker class for labels and headings.
        /// </summary>
        public const string LabelClass = "Label";

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
        /// Standard marker class for content that has been elided and may be made available
        /// instead as an attribute.
        /// </summary>
        public const string EllipsisClass = "Ellipsis";

        /// <summary>
        /// Standard marker class for code location references.
        /// </summary>
        public const string CodeLocationClass = "CodeLocation";

        /// <summary>
        /// Path attribute for code location references.
        /// </summary>
        public const string CodeLocationPathAttrib = "path";

        /// <summary>
        /// Line attribute for code location references.
        /// </summary>
        public const string CodeLocationLineNumberAttrib = "line";

        /// <summary>
        /// Column attribute for code location references.
        /// </summary>
        public const string CodeLocationColumnNumberAttrib = "column";

        /// <summary>
        /// Standard marker class for link to a url.
        /// </summary>
        public const string LinkClass = "Link";

        /// <summary>
        /// Url attribute for links.
        /// </summary>
        public const string LinkUrlAttrib = "url";

        /// <summary>
        /// Standard marker for assertion failures.
        /// </summary>
        public static Marker AssertionFailure
        {
            get { return new Marker(AssertionFailureClass); }
        }

        /// <summary>
        /// Standard marker for labels and headings.
        /// </summary>
        public static Marker Label
        {
            get { return new Marker(LabelClass); }
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
        /// Standard marker for text that is elided and replaced by an ellipsis.
        /// </summary>
        public static Marker Ellipsis
        {
            get { return new Marker(EllipsisClass); }
        }

        /// <summary>
        /// Creates a standard marker for a code location.
        /// </summary>
        public static Marker CodeLocation(CodeLocation location)
        {
            var marker = new Marker(CodeLocationClass);
            if (location.Path != null)
                marker = marker.WithAttribute(CodeLocationPathAttrib, location.Path);
            if (location.Line != 0)
                marker = marker.WithAttribute(CodeLocationLineNumberAttrib, location.Line.ToString(CultureInfo.InvariantCulture));
            if (location.Column != 0)
                marker = marker.WithAttribute(CodeLocationPathAttrib, location.Column.ToString(CultureInfo.InvariantCulture));
            return marker;
        }

        /// <summary>
        /// Creates a standard marker for a link to a Url.
        /// </summary>
        public static Marker Link(string url)
        {
            var marker = new Marker(LinkClass);
            if (url != null)
                marker = marker.WithAttribute(LinkUrlAttrib, url);
            return marker;
        }

        /// <summary>
        /// Creates a marker.
        /// </summary>
        /// <param name="class">The marker class</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="class"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="class"/> is not a valid identifier.  <seealso cref="ValidateIdentifier"/></exception>
        public Marker(string @class)
            : this(@class, EmptyDictionary<string, string>.Instance)
        {
        }

        /// <summary>
        /// Creates a marker with attributes.
        /// </summary>
        /// <param name="class">The marker class</param>
        /// <param name="attributes">The attributes</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="class"/> or
        /// <paramref name="attributes"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="class"/> is not a valid identifier.  <seealso cref="ValidateIdentifier"/></exception>
        public Marker(string @class, IDictionary<string, string> attributes)
        {
            ValidateIdentifier(@class);
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            foreach (KeyValuePair<string, string> pair in attributes)
                ValidateAttribute(pair.Key, pair.Value);

            this.@class = @class;
            this.attributes = new AttributeDictionary(attributes);
        }

        // Optimized constructor when arguments are already known to be valid.
        private Marker(string @class, AttributeDictionary attributes)
        {
            this.@class = @class;
            this.attributes = attributes;
        }

        /// <summary>
        /// Gets the marker class.
        /// </summary>
        public string Class
        {
            get { return @class; }
        }

        /// <summary>
        /// Gets the marker's attributes which are optional name/value pairs associated
        /// with a marker to carry additional semantic content.
        /// </summary>
        public IDictionary<string, string> Attributes
        {
            get { return new ReadOnlyDictionary<string, string>(attributes); }
        }

        /// <summary>
        /// Creates a copy of the marker with the specified attribute added.
        /// </summary>
        /// <param name="name">The attribute name</param>
        /// <param name="value">The attribute value</param>
        /// <returns>The marker copy with the attribute</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="name"/> is not a valid identifier.  <seealso cref="ValidateIdentifier"/></exception>
        public Marker WithAttribute(string name, string value)
        {
            ValidateAttribute(name, value);

            var newAttributes = new AttributeDictionary(attributes) { { name, value } };
            return new Marker(@class, newAttributes);
        }

        /// <summary>
        /// Verifies that the parameter is a valid marker class or attribute identifier.
        /// </summary>
        /// <param name="identifier">The identifier</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="identifier"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="identifier"/> is empty or contains characters
        /// other than letters, digits and underscores</exception>
        public static void ValidateIdentifier(string identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (identifier.Length == 0)
                throw new ArgumentException("Marker class or attribute name must not be empty.", "identifier");

            foreach (char c in identifier)
            {
                if (!Char.IsLetterOrDigit(c) && c != '_')
                    throw new ArgumentException("Marker class or attribute name must only consist of letters, digits and underscores.", "identifier");
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(@class);

            bool first = true;
            foreach (KeyValuePair<string, string> pair in attributes)
            {
                if (first)
                {
                    result.Append(": ");
                    first = false;
                }
                else
                    result.Append(", ");

                result.Append(pair.Key).Append(" = \"").Append(pair.Value).Append("\"");
            }

            return result.ToString();
        }

        /// <inheritdoc />
        public bool Equals(Marker other)
        {
            return @class == other.@class
                && GenericCollectionUtils.KeyValuePairsEqual(attributes, other.attributes);
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

        internal static void ValidateAttribute(string name, string value)
        {
            ValidateIdentifier(name);
            if (value == null)
                throw new ArgumentNullException("value");
        }

        // This is a workaround because Mono's implementation of SortedDictionary does not support serialization.
        // https://bugzilla.novell.com/show_bug.cgi?id=349053
        [Serializable]
        private sealed class AttributeDictionary : SortedDictionary<string, string>, ISerializable
        {
            private const string ContentsKey = "Contents";

            public AttributeDictionary(IDictionary<string, string> other)
                : base(other)
            {
            }

            private AttributeDictionary(SerializationInfo info, StreamingContext context)
            {
                foreach (KeyValuePair<string, string> pair in
                    (KeyValuePair<string, string>[]) info.GetValue(ContentsKey, typeof(KeyValuePair<string, string>[])))
                    Add(pair.Key, pair.Value);
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(ContentsKey, GenericCollectionUtils.ToArray(this));
            }
        }
    }
}
