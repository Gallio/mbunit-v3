using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// A code reference is a pointer into the structure of a .Net program for use in
    /// describing the location of a certain code construct to the user.  It is
    /// typically used to identify the point of definition of a test component.
    /// </summary>
    public class CodeReference
    {
        /// <summary>
        /// Gets an empty code reference used to indicate that the actual
        /// reference is unknown.
        /// </summary>
        public static readonly CodeReference Unknown = new CodeReference(null, null, null, null, null);

        private Assembly assembly;
        private string @namespace;
        private Type type;
        private MemberInfo member;
        private ParameterInfo parameter;

        /// <summary>
        /// Creates a code reference.
        /// </summary>
        /// <param name="assembly">The referenced assembly, or null if none</param>
        /// <param name="namespace">The referenced namespace, or null if none</param>
        /// <param name="type">The referenced type, or null if none</param>
        /// <param name="member">The referenced member, or null if none</param>
        /// <param name="parameter">The referenced parameter, or null if none</param>
        protected CodeReference(Assembly assembly, string @namespace, Type type, MemberInfo member, ParameterInfo parameter)
        {
            this.assembly = assembly;
            this.@namespace = @namespace;
            this.type = type;
            this.member = member;
            this.parameter = parameter;
        }

        /// <summary>
        /// Gets the referenced assembly, or null if none.
        /// </summary>
        public Assembly Assembly
        {
            get { return assembly; }
        }

        /// <summary>
        /// Gets the referenced namespace, or null if none.
        /// </summary>
        public string Namespace
        {
            get { return @namespace; }
        }

        /// <summary>
        /// Gets the referenced type, or null if none.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the referenced type member, or null if none.
        /// </summary>
        public MemberInfo Member
        {
            get { return member; }
        }

        /// <summary>
        /// Gets the referenced method parameter, or null if none.
        /// </summary>
        public ParameterInfo Parameter
        {
            get { return parameter; }
        }

        /// <summary>
        /// Produces a human-readable description of the code reference.
        /// </summary>
        /// <returns>A description of the code reference</returns>
        public override string ToString()
        {
            StringBuilder description = new StringBuilder();

            if (parameter != null)
                description.AppendFormat(CultureInfo.CurrentCulture,
                    "Parameter '{0}' of ", parameter.Name);

            if (type != null)
            {
                description.Append(type.FullName);

                if (member != null)
                    description.Append('.').Append(member.Name);
            }
            else if (@namespace != null)
            {
                description.Append(@namespace);
            }

            if (assembly != null)
            {
                if (description.Length != 0)
                    description.Append(", ");

                description.Append(assembly.GetName().Name);
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
                && assembly == other.Assembly
                && @namespace == other.@namespace
                && type == other.type
                && member == other.member
                && parameter == other.parameter;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (assembly != null ? assembly.GetHashCode() : 0)
                ^ (@namespace != null ? @namespace.GetHashCode() : 0)
                ^ (type != null ? type.GetHashCode() : 0)
                ^ (member != null ? member.GetHashCode() : 0)
                ^ (parameter != null ? parameter.GetHashCode() : 0);
        }
    }
}
