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
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Gallio.Utilities;
using System.Runtime.Serialization;

namespace Gallio.Reflection
{
    /// <summary>
    /// Specifies the location of a code element as a position within a file.
    /// </summary>
    /// <remarks>
    /// Lines and columns are numbered starting from 1.
    /// </remarks>
    [Serializable]
    [XmlRoot("codeLocation", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlSchemaProvider("ProvideXmlSchema")]
    public struct CodeLocation : IEquatable<CodeLocation>, IXmlSerializable, ISerializable
    {
        private string path;
        private int line;
        private int column;

        /// <summary>
        /// Gets an empty code location with a null path and no line or column number information.
        /// </summary>
        public static readonly CodeLocation Unknown = new CodeLocation(null, 0, 0);

        /// <summary>
        /// Creates a code location.
        /// </summary>
        /// <param name="path">The path or Uri of a resource that contains the code element,
        /// such as a source file or assembly, or null if unknown</param>
        /// <param name="line">The line number, or 0 if unknown</param>
        /// <param name="column">The column number, or 0 if unknown</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="line"/> or <paramref name="column"/>
        /// is less than 0</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is null and <paramref name="line"/>
        /// is non-zero, or if <paramref name="line"/> is 0 and <paramref name="column"/> is non-zero</exception>
        public CodeLocation(string path, int line, int column)
        {
            if (line < 0)
                throw new ArgumentOutOfRangeException("line");
            if (column < 0)
                throw new ArgumentOutOfRangeException("column");
            if (path == null && line != 0)
                throw new ArgumentException("No path was specified but line information was provided.", "line");
            if (line == 0 && column != 0)
                throw new ArgumentException("No line was specified but column information was provided.", "column");

            this.path = path;
            this.line = line;
            this.column = column;
        }

        /// <summary>
        /// Gets the path or Uri of a resource that contains the code element, such as
        /// a source file or assembly, or null if unknown.
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Gets the line number, or 0 if unknown.
        /// </summary>
        public int Line
        {
            get { return line; }
        }

        /// <summary>
        /// Gets the column number, or 0 if unknown.
        /// </summary>
        public int Column
        {
            get { return column; }
        }

        /// <summary>
        /// Converts the location to a string of the form "path(line,column)",
        /// "path(line)" or "path" depending on which components are available.
        /// </summary>
        /// <returns>The code location as a string or "(unknown)" if unknown</returns>
        public override string ToString()
        {
            if (line != 0)
            {
                if (column != 0)
                    return String.Format("{0}({1},{2})", path, line, column);
                return String.Format("{0}({1})", path, line);
            }

            return path ?? "(unknown)";
        }

        #region Equality
        /// <summary>
        /// Compares two code locations for equality.
        /// </summary>
        /// <param name="a">The first code location</param>
        /// <param name="b">The second code location</param>
        /// <returns>True if the code locations are equal</returns>
        public static bool operator ==(CodeLocation a, CodeLocation b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Compares two code locations for inequality.
        /// </summary>
        /// <param name="a">The first code location</param>
        /// <param name="b">The second code location</param>
        /// <returns>True if the code references are not equal</returns>
        public static bool operator !=(CodeLocation a, CodeLocation b)
        {
            return !a.Equals(b);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CodeLocation && Equals((CodeLocation)obj);
        }

        /// <inheritdoc />
        public bool Equals(CodeLocation other)
        {
            return path == other.path
                && line == other.line
                && column == other.column;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (path != null ? path.GetHashCode() : 0) ^ (line << 5) ^ column;
        }
        #endregion

        #region Serialization

        // Note: We implement our own Xml serialization so that the code location object can still appear to be immutable.
        // since we don't need any property setters unlike if we were using [XmlAttribute] attributes.

        /// <summary>
        /// Provides the Xml schema for this element.
        /// </summary>
        /// <param name="schemas">The schema set</param>
        /// <returns>The schema type of the element</returns>
        public static XmlQualifiedName ProvideXmlSchema(XmlSchemaSet schemas)
        {
            schemas.Add(new XmlSchema()
            {
                TargetNamespace = XmlSerializationUtils.GallioNamespace,
                Items =
                {
                    new XmlSchemaComplexType()
                    {
                        Name = "CodeLocation",
                        Attributes =
                        {
                            new XmlSchemaAttribute()
                            {
                                Name = "path",
                                Use = XmlSchemaUse.Optional,
                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                            },
                            new XmlSchemaAttribute()
                            {
                                Name = "line",
                                Use = XmlSchemaUse.Optional,
                                SchemaTypeName = new XmlQualifiedName("int", "http://www.w3.org/2001/XMLSchema")
                            },
                            new XmlSchemaAttribute()
                            {
                                Name = "column",
                                Use = XmlSchemaUse.Optional,
                                SchemaTypeName = new XmlQualifiedName("int", "http://www.w3.org/2001/XMLSchema")
                            }
                        }
                    }
                }
            });

            return new XmlQualifiedName("CodeLocation", XmlSerializationUtils.GallioNamespace);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotSupportedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            path = reader.GetAttribute(@"path");
            line = GetIntegerAttributeOrZeroIfAbsent(reader, @"line");
            column = GetIntegerAttributeOrZeroIfAbsent(reader, @"column");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (path != null)
                writer.WriteAttributeString(@"path", path);
            if (line != 0)
                writer.WriteAttributeString(@"line", line.ToString(CultureInfo.InvariantCulture));
            if (column != 0)
                writer.WriteAttributeString(@"column", column.ToString(CultureInfo.InvariantCulture));
        }

        private static int GetIntegerAttributeOrZeroIfAbsent(XmlReader reader, string name)
        {
            string value = reader.GetAttribute(name);
            if (value == null)
                return 0;
            return int.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture);
        }

        private const string PathKey = "Gallio.CodeLocation.Path";
        private const string LineKey = "Gallio.CodeLocation.Line";
        private const string ColumnKey = "Gallio.CodeLocation.Column";

        /// <summary>
        /// Allows the code location structure to control 
        /// its own serialization and deserialization.
        /// </summary>
        /// <param name="info">The serialization info to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(PathKey, path);
            info.AddValue(LineKey, line);
            info.AddValue(ColumnKey, column);
        }

        private CodeLocation(SerializationInfo info, StreamingContext context)
        {
            path = info.GetString(PathKey);
            line = info.GetInt32(LineKey);
            column = info.GetInt32(ColumnKey);
        }

        #endregion
    }
}