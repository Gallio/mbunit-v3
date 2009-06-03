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
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Gallio.Common.Xml;
using Gallio.Properties;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// A code reference is a pointer into the structure of a .Net program for use in
    /// describing the location of a certain code construct to the user.  It is
    /// typically used to identify the point of definition of a test component.
    /// </summary>
    [Serializable]
    [XmlRoot("codeReference", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlSchemaProvider("ProvideXmlSchema")]
    public struct CodeReference : IEquatable<CodeReference>, IXmlSerializable
    {
        private string assemblyName;
        private string namespaceName;
        private string typeName;
        private string memberName;
        private string parameterName;

        /// <summary>
        /// Gets an empty code reference used to indicate that the actual
        /// reference is unknown.
        /// </summary>
        public static readonly CodeReference Unknown = new CodeReference();

        /// <summary>
        /// Creates a code reference from strings.
        /// </summary>
        /// <param name="assemblyName">The assembly name, or null if none.</param>
        /// <param name="namespaceName">The namespace name, or null if none.</param>
        /// <param name="typeName">The fully-qualified type name, or null if none.</param>
        /// <param name="memberName">The member name, or null if none.</param>
        /// <param name="parameterName">The parameter name, or null if none.</param>
        public CodeReference(string assemblyName, string namespaceName, string typeName,
            string memberName, string parameterName)
        {
            this.assemblyName = assemblyName;
            this.namespaceName = namespaceName;
            this.typeName = typeName;
            this.memberName = memberName;
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Gets the kind of code element specified by the code reference.
        /// </summary>
        [XmlIgnore]
        public CodeReferenceKind Kind
        {
            get
            {
                if (assemblyName == null && namespaceName == null)
                    return CodeReferenceKind.Unknown;
                if (namespaceName == null)
                    return CodeReferenceKind.Assembly;
                if (typeName == null)
                    return CodeReferenceKind.Namespace;
                if (memberName == null)
                    return CodeReferenceKind.Type;
                if (parameterName == null)
                    return CodeReferenceKind.Member;
                return CodeReferenceKind.Parameter;
            }
        }

        /// <summary>
        /// Gets the assembly name, or null if none.
        /// </summary>
        public string AssemblyName
        {
            get { return assemblyName; }
        }

        /// <summary>
        /// Gets the namespace name, or null if none.
        /// </summary>
        public string NamespaceName
        {
            get { return namespaceName; }
        }

        /// <summary>
        /// Gets the fully-qualified type name, or null if none.
        /// </summary>
        public string TypeName
        {
            get { return typeName; }
        }

        /// <summary>
        /// Gets the member name, or null if none.
        /// </summary>
        public string MemberName
        {
            get { return memberName; }
        }

        /// <summary>
        /// Gets the parameter name, or null if none.
        /// </summary>
        public string ParameterName
        {
            get { return parameterName; }
        }

        /// <summary>
        /// Produces a human-readable description of the code reference.
        /// </summary>
        /// <returns>A description of the code reference</returns>
        public override string ToString()
        {
            StringBuilder description = new StringBuilder();

            if (parameterName != null)
                description.AppendFormat(Resources.CodeReference_ToString_ParameterName, parameterName);

            if (typeName != null)
            {
                description.Append(typeName);

                if (memberName != null)
                    description.Append('.').Append(memberName);
            }
            else if (namespaceName != null)
            {
                description.Append(namespaceName);
            }

            if (assemblyName != null)
            {
                if (description.Length != 0)
                    description.Append(@", ");

                description.Append(assemblyName);
            }

            return description.ToString();
        }

        /// <summary>
        /// Creates a code reference from a method parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null.</exception>
        public static CodeReference CreateFromParameter(ParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(@"parameter");

            MemberInfo member = parameter.Member;
            Type type = GetSimpleType(member.ReflectedType);
            return new CodeReference(type.Assembly.FullName, GetTypeNamespace(type), GetTypeName(type), member.Name, parameter.Name);
        }

        /// <summary>
        /// Creates a code reference from a member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="member"/> is null.</exception>
        public static CodeReference CreateFromMember(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            Type type = member as Type;
            if (type != null)
                return CreateFromType(type);

            type = GetSimpleType(member.ReflectedType);
            return new CodeReference(type.Assembly.FullName, GetTypeNamespace(type), GetTypeName(type), member.Name, null);
        }

        /// <summary>
        /// Creates a code reference from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public static CodeReference CreateFromType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(@"type");

            type = GetSimpleType(type);
            return new CodeReference(type.Assembly.FullName, GetTypeNamespace(type), GetTypeName(type), null, null);
        }

        /// <summary>
        /// Creates a code reference from an namespace name.
        /// </summary>
        /// <param name="namespaceName">The namespace name.</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="namespaceName"/> is null.</exception>
        public static CodeReference CreateFromNamespace(string namespaceName)
        {
            if (namespaceName == null)
                throw new ArgumentNullException(@"namespaceName");

            return new CodeReference(null, namespaceName, null, null, null);
        }

        /// <summary>
        /// Creates a code reference from an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static CodeReference CreateFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");

            return new CodeReference(assembly.FullName, null, null, null, null);
        }

        #region Equality
        /// <summary>
        /// Compares two code references for equality.
        /// </summary>
        /// <param name="a">The first code reference.</param>
        /// <param name="b">The second code reference.</param>
        /// <returns>True if the code references are equal</returns>
        public static bool operator ==(CodeReference a, CodeReference b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Compares two code references for inequality.
        /// </summary>
        /// <param name="a">The first code reference.</param>
        /// <param name="b">The second code reference.</param>
        /// <returns>True if the code references are not equal</returns>
        public static bool operator !=(CodeReference a, CodeReference b)
        {
            return !a.Equals(b);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CodeReference && Equals((CodeReference) obj);
        }

        /// <inheritdoc />
        public bool Equals(CodeReference other)
        {
            return assemblyName == other.assemblyName
               && namespaceName == other.namespaceName
               && typeName == other.typeName
               && memberName == other.memberName
               && parameterName == other.parameterName;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (assemblyName != null ? assemblyName.GetHashCode() : 0)
                   ^ (namespaceName != null ? namespaceName.GetHashCode() : 0)
                   ^ (typeName != null ? typeName.GetHashCode() : 0)
                   ^ (memberName != null ? memberName.GetHashCode() : 0)
                   ^ (parameterName != null ? parameterName.GetHashCode() : 0);
        }
        #endregion

        #region Xml Serialization
        /* Note: We implement our own Xml serialization so that the code reference object can still appear to be immutable.
                 since we don't need any property setters unlike if we were using [XmlAttribute] attributes. */

        /// <summary>
        /// Provides the Xml schema for this element.
        /// </summary>
        /// <param name="schemas">The schema set.</param>
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
                        Name = "CodeReference",
                        Attributes =
                        {
                            new XmlSchemaAttribute()
                            {
                                Name = "assembly",
                                Use = XmlSchemaUse.Optional,
                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                            },
                            new XmlSchemaAttribute()
                            {
                                Name = "namespace",
                                Use = XmlSchemaUse.Optional,
                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                            },
                            new XmlSchemaAttribute()
                            {
                                Name = "type",
                                Use = XmlSchemaUse.Optional,
                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                            },
                            new XmlSchemaAttribute()
                            {
                                Name = "member",
                                Use = XmlSchemaUse.Optional,
                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                            },
                            new XmlSchemaAttribute()
                            {
                                Name = "parameter",
                                Use = XmlSchemaUse.Optional,
                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                            }
                        }
                    }
                }
            });

            return new XmlQualifiedName("CodeReference", XmlSerializationUtils.GallioNamespace);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotSupportedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            assemblyName = reader.GetAttribute(@"assembly");
            namespaceName = reader.GetAttribute(@"namespace");
            typeName = reader.GetAttribute(@"type");
            memberName = reader.GetAttribute(@"member");
            parameterName = reader.GetAttribute(@"parameter");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (assemblyName != null)
                writer.WriteAttributeString(@"assembly", assemblyName);
            if (namespaceName != null)
                writer.WriteAttributeString(@"namespace", namespaceName);
            if (typeName != null)
                writer.WriteAttributeString(@"type", typeName);
            if (memberName != null)
                writer.WriteAttributeString(@"member", memberName);
            if (parameterName != null)
                writer.WriteAttributeString(@"parameter", parameterName);
        }
        #endregion

        private static Type GetSimpleType(Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        private static string GetTypeNamespace(Type type)
        {
            return type.Namespace ?? "";
        }

        private static string GetTypeName(Type type)
        {
            return type.FullName ?? type.Name;
        }
    }
}