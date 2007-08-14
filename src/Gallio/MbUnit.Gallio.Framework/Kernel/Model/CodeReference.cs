// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A code reference is a pointer into the structure of a .Net program for use in
    /// describing the location of a certain code construct to the user.  It is
    /// typically used to identify the point of definition of a test component.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class CodeReference
    {
        /// <summary>
        /// Gets an empty code reference used to indicate that the actual
        /// reference is unknown.
        /// </summary>
        public static CodeReference Unknown
        {
            get { return new CodeReference(); }
        }

        private string assemblyName;
        private string namespaceName;
        private string typeName;
        private string memberName;
        private string parameterName;

        /// <summary>
        /// Creates an empty code reference.
        /// </summary>
        public CodeReference()
        {
        }

        /// <summary>
        /// Creates a code reference from strings.
        /// </summary>
        /// <param name="assemblyName">The assembly name, or null if none</param>
        /// <param name="namespaceName">The namespace name, or null if none</param>
        /// <param name="typeName">The fully-qualified type name, or null if none</param>
        /// <param name="memberName">The member name, or null if none</param>
        /// <param name="parameterName">The parameter name, or null if none</param>
        public CodeReference(string assemblyName, string namespaceName, string typeName, string memberName, string parameterName)
        {
            this.assemblyName = assemblyName;
            this.namespaceName = namespaceName;
            this.typeName = typeName;
            this.memberName = memberName;
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Creates a code reference.
        /// </summary>
        /// <param name="assembly">The assembly, or null if none</param>
        /// <param name="namespace">The namespace, or null if none</param>
        /// <param name="type">The type, or null if none</param>
        /// <param name="member">The member, or null if none</param>
        /// <param name="parameter">The parameter, or null if none</param>
        public CodeReference(Assembly assembly, string @namespace, Type type, MemberInfo member, ParameterInfo parameter)
            : this(assembly != null ? assembly.FullName : null,
                @namespace,
                type != null ? type.FullName : null,
                member != null ? member.Name : null,
                parameter != null ? parameter.Name : null)
        {
        }

        /// <summary>
        /// Gets or sets the assembly name, or null if none.
        /// </summary>
        [XmlAttribute("assembly")]
        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        /// <summary>
        /// Gets or sets the namespace name, or null if none.
        /// </summary>
        [XmlAttribute("namespace")]
        public string NamespaceName
        {
            get { return namespaceName; }
            set { namespaceName = value; }
        }

        /// <summary>
        /// Gets or sets the fully-qualified type name, or null if none.
        /// </summary>
        [XmlAttribute("type")]
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        /// <summary>
        /// Gets or sets the member name, or null if none.
        /// </summary>
        [XmlAttribute("member")]
        public string MemberName
        {
            get { return memberName; }
            set { memberName = value; }
        }

        /// <summary>
        /// Gets or sets the parameter name, or null if none.
        /// </summary>
        [XmlAttribute("parameter")]
        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }

        /// <summary>
        /// Produces a human-readable description of the code reference.
        /// </summary>
        /// <returns>A description of the code reference</returns>
        public override string ToString()
        {
            StringBuilder description = new StringBuilder();

            if (parameterName != null)
                description.AppendFormat(CultureInfo.CurrentCulture,
                    "Parameter '{0}' of ", parameterName);

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
                    description.Append(", ");

                description.Append(assemblyName);
            }

            return description.ToString();
        }

        /// <summary>
        /// Creates a code reference from a method parameter.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null</exception>
        public static CodeReference CreateFromParameter(ParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            MemberInfo member = parameter.Member;
            return new CodeReference(member.ReflectedType.Assembly, member.ReflectedType.Namespace,
                member.ReflectedType, member, parameter);
        }

        /// <summary>
        /// Creates a code reference from a member.
        /// </summary>
        /// <param name="member">The member</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="member"/> is null</exception>
        public static CodeReference CreateFromMember(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            return new CodeReference(member.ReflectedType.Assembly, member.ReflectedType.Namespace,
                member.ReflectedType, member, null);
        }

        /// <summary>
        /// Creates a code reference from a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static CodeReference CreateFromType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return new CodeReference(type.Assembly, type.Namespace, type, null, null);
        }

        /// <summary>
        /// Creates a code reference from an assembly.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public static CodeReference CreateFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return new CodeReference(assembly, null, null, null, null);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            CodeReference other = obj as CodeReference;
            return other != null
                && assemblyName == other.assemblyName
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
    }
}
