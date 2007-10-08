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
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Model.Serialization;
using MbUnit.Properties;

namespace MbUnit.Model
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

        [NonSerialized]
        private Assembly cachedAssembly;
        [NonSerialized]
        private Type cachedType;
        [NonSerialized]
        private MemberInfo cachedMember;
        [NonSerialized]
        private ParameterInfo cachedParameter;

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
            cachedAssembly = assembly;
            cachedType = type;
            cachedMember = member;
            cachedParameter = parameter;
        }

        /// <summary>
        /// Gets the kind of code element specified by the code reference.
        /// </summary>
        public CodeReferenceKind Kind
        {
            get
            {
                if (assemblyName == null)
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
        /// Gets or sets the assembly name, or null if none.
        /// </summary>
        [XmlAttribute("assembly")]
        public string AssemblyName
        {
            get { return assemblyName; }
            set
            {
                CheckReadOnlyAndClearCache();
                assemblyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the namespace name, or null if none.
        /// </summary>
        [XmlAttribute("namespace")]
        public string NamespaceName
        {
            get { return namespaceName; }
            set
            {
                CheckReadOnlyAndClearCache();
                namespaceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the fully-qualified type name, or null if none.
        /// </summary>
        [XmlAttribute("type")]
        public string TypeName
        {
            get { return typeName; }
            set
            {
                CheckReadOnlyAndClearCache();
                typeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the member name, or null if none.
        /// </summary>
        [XmlAttribute("member")]
        public string MemberName
        {
            get { return memberName; }
            set
            {
                CheckReadOnlyAndClearCache();
                memberName = value;
            }
        }

        /// <summary>
        /// Gets or sets the parameter name, or null if none.
        /// </summary>
        [XmlAttribute("parameter")]
        public string ParameterName
        {
            get { return parameterName; }
            set
            {
                CheckReadOnlyAndClearCache();
                parameterName = value;
            }
        }

        /// <summary>
        /// Creates a copy of the code reference.
        /// </summary>
        /// <returns>The copy</returns>
        public CodeReference Copy()
        {
            return new CodeReference().InitializeFrom(this);
        }

        /// <summary>
        /// Creates a read-only copy of the code reference.
        /// </summary>
        /// <returns>The read-only copy</returns>
        public CodeReference ReadOnlyCopy()
        {
            return new ReadOnlyCodeReference().InitializeFrom(this);
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
        /// Resolves the code reference's <see cref="Assembly" />.
        /// </summary>
        /// <returns>The resolved assembly</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="AssemblyName"/> is null</exception>
        public Assembly ResolveAssembly()
        {
            if (cachedAssembly == null)
            {
                if (assemblyName == null)
                    throw new InvalidOperationException("The code reference does not contain assembly information.");

                cachedAssembly = Assembly.Load(assemblyName);
            }

            return cachedAssembly;
        }

        /// <summary>
        /// Resolves the code reference's <see cref="Type" />.
        /// </summary>
        /// <returns>The resolved type</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="TypeName"/> is null</exception>
        public Type ResolveType()
        {
            if (cachedType == null)
            {
                if (typeName == null)
                    throw new InvalidOperationException("The code reference does not contain type information.");

                cachedType = ResolveAssembly().GetType(typeName, true);
            }

            return cachedType;
        }

        /// <summary>
        /// Resolves the code reference's <see cref="MemberInfo" />.
        /// </summary>
        /// <returns>The resolved member</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="MemberName"/> is null</exception>
        public MemberInfo ResolveMember()
        {
            if (cachedMember == null)
            {
                if (memberName == null)
                    throw new InvalidOperationException("The code reference does not contain member information.");
                MemberInfo[] members = ResolveType().GetMember(memberName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                // TODO: Handle overloading by signature.
                if (members.Length == 0)
                    throw new InvalidOperationException("The named member could not be found.");
                if (members.Length != 1)
                    throw new NotImplementedException("Cannot resolve overloaded members because the complete signature is not available.");

                cachedMember = members[0];
            }

            return cachedMember;
        }

        /// <summary>
        /// Resolves the code reference's <see cref="ParameterInfo" />.
        /// </summary>
        /// <returns>The resolved parameter</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ParameterName"/> is null</exception>
        public ParameterInfo ResolveParameter()
        {
            if (cachedParameter == null)
            {
                if (parameterName == null)
                    throw new InvalidOperationException("The code reference does not contain parameter information.");

                MethodBase method = ResolveMember() as MethodBase;
                if (method == null)
                    throw new InvalidOperationException("The code reference refers to a member that is not a method or constructor, consequently it does not have parameters.");

                cachedParameter = Array.Find(method.GetParameters(), delegate(ParameterInfo parameter)
                {
                    return parameter.Name == parameterName;
                });
                if (cachedParameter == null)
                    throw new InvalidOperationException("The named parameter could not be found.");
            }

            return cachedParameter;
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
                throw new ArgumentNullException(@"parameter");

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
                throw new ArgumentNullException(@"member");

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
                throw new ArgumentNullException(@"type");

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
                throw new ArgumentNullException(@"assembly");

            return new CodeReference(assembly, null, null, null, null);
        }

        /// <summary>
        /// Creates a code reference from the executing method.
        /// </summary>
        /// <returns>The code reference</returns>
        public static CodeReference CreateFromExecutingMethod()
        {
            return CreateFromStackFrame(1);
        }

        /// <summary>
        /// Creates a code reference from the caller of the executing method.
        /// </summary>
        /// <returns>The code reference</returns>
        public static CodeReference CreateFromCallingMethod()
        {
            return CreateFromStackFrame(2);
        }

        /// <summary>
        /// Creates a code reference from a stack frame.
        /// </summary>
        /// <param name="stackFrame">The stack frame</param>
        /// <returns>The code reference</returns>
        public static CodeReference CreateFromStackFrame(StackFrame stackFrame)
        {
            if (stackFrame == null)
                throw new ArgumentNullException(@"stackFrame");

            return CreateFromMember(stackFrame.GetMethod());
        }

        /// <summary>
        /// Creates a code reference from a particular frame on the current stack.
        /// </summary>
        /// <param name="framesToSkip">The number of frames to skip.  If this number is 0,
        /// the code reference will refer to the direct caller of this method;
        /// if it is 1, it will refer to the caller's caller, and so on.</param>
        /// <returns>The code reference</returns>
        public static CodeReference CreateFromStackFrame(int framesToSkip)
        {
            StackTrace stackTrace = new StackTrace(framesToSkip + 1, false);
            return CreateFromStackFrame(stackTrace.GetFrame(0));
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

        protected internal virtual void CheckReadOnlyAndClearCache()
        {
            cachedAssembly = null;
            cachedType = null;
            cachedMember = null;
            cachedParameter = null;
        }

        private CodeReference InitializeFrom(CodeReference other)
        {
            assemblyName = other.assemblyName;
            namespaceName = other.namespaceName;
            typeName = other.typeName;
            memberName = other.memberName;
            parameterName = other.parameterName;

            cachedAssembly = other.cachedAssembly;
            cachedType = other.cachedType;
            cachedMember = other.cachedMember;
            cachedParameter = other.cachedParameter;
            return this;
        }

        private sealed class ReadOnlyCodeReference : CodeReference
        {
            protected internal override void CheckReadOnlyAndClearCache()
            {
                throw new InvalidOperationException("The code reference is read-only.");
            }
        }
    }
}
