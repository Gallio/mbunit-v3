using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Reflection
{
    /// <summary>
    /// Describes the name of a type and allows partial type names to be compared with one another.
    /// </summary>
    [Serializable]
    public sealed class TypeName : IEquatable<TypeName>
    {
        private readonly string fullName;
        private readonly AssemblyName assemblyName;

        /// <summary>
        /// Creates a type name from a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/> does not have a valid FullName property
        /// (for example, it represents a generic type parameter).</exception>
        public TypeName(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            fullName = type.FullName;
            if (fullName == null)
                throw new ArgumentException("The type must have a valid FullName.", "type");

            assemblyName = type.Assembly.GetName();
        }

        /// <summary>
        /// Creates a type name from type info.
        /// </summary>
        /// <param name="typeInfo">The type info</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="typeInfo"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="typeInfo"/> does not have a valid FullName property
        /// (for example, it represents a generic type parameter).</exception>
        public TypeName(ITypeInfo typeInfo)
        {
            if (typeInfo == null)
                throw new ArgumentNullException("typeInfo");

            fullName = typeInfo.FullName;
            if (fullName == null)
                throw new ArgumentException("The type must have a valid FullName.", "typeInfo");

            assemblyName = typeInfo.Assembly.GetName();
        }

        /// <summary>
        /// Creates a type name from its assembly-qualified name.
        /// </summary>
        /// <param name="assemblyQualifiedName">The assembly-qualified name of the type, including
        /// its namespace and assembly</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyQualifiedName"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="assemblyQualifiedName"/> is malformed or
        /// does not contain a valid assembly name</exception>
        public TypeName(string assemblyQualifiedName)
        {
            if (assemblyQualifiedName == null)
                throw new ArgumentNullException("assemblyQualifiedName");

            int lastBracketPos = assemblyQualifiedName.LastIndexOf(']');
            int commaPos = assemblyQualifiedName.IndexOf(',', lastBracketPos + 1);
            if (commaPos < 0)
                throw new ArgumentException("The assembly qualified name must include the assembly name.", "assemblyQualifiedName");

            fullName = assemblyQualifiedName.Substring(0, commaPos);
            assemblyName = new AssemblyName(assemblyQualifiedName.Substring(commaPos + 1));
        }

        /// <summary>
        /// Creates a type name from its full name and assembly name.
        /// </summary>
        /// <param name="fullName">The full name of the type, including its namespace</param>
        /// <param name="assemblyName">The full or partial name of the assembly that contains the type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fullName"/> or <paramref name="assemblyName"/> is null</exception>
        public TypeName(string fullName, AssemblyName assemblyName)
        {
            if (fullName == null)
                throw new ArgumentNullException("fullName");
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            this.fullName = fullName;
            this.assemblyName = assemblyName;
        }

        /// <summary>
        /// Gets the assembly-qualified name of the type, including its namespace and assembly.
        /// </summary>
        public string AssemblyQualifiedName
        {
            get { return string.Concat(fullName, ", ", assemblyName.FullName); }
        }

        /// <summary>
        /// Gets the full name of the type, including its namespace.
        /// </summary>
        public string FullName
        {
            get { return fullName; }
        }

        /// <summary>
        /// Gets the full or partial name of the assembly that contains the type.
        /// </summary>
        public AssemblyName AssemblyName
        {
            get { return assemblyName; }
        }

        /// <inheritdoc />
        public bool Equals(TypeName other)
        {
            return other != null
                && fullName == other.fullName
                && assemblyName.FullName == other.assemblyName.FullName;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as TypeName);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return fullName.GetHashCode() ^ assemblyName.FullName.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return AssemblyQualifiedName;
        }

        /// <summary>
        /// Resolves the type named by this instance.
        /// </summary>
        /// <returns>The type</returns>
        /// <exception cref="ReflectionResolveException">Thrown if the type could not be resolved</exception>
        public Type Resolve()
        {
            string assemblyQualifiedName = AssemblyQualifiedName;
            try
            {
                Type type = Type.GetType(assemblyQualifiedName);
                if (type != null)
                    return type;
            }
            catch (Exception ex)
            {
                throw new ReflectionResolveException(string.Format("Could not resolve type '{0}'.", assemblyQualifiedName), ex);
            }

            throw new ReflectionResolveException(string.Format("Could not resolve type '{0}'.", assemblyQualifiedName));
        }

        /// <summary>
        /// Returns true if the associated <see cref="AssemblyName"/> is a partial name only.
        /// </summary>
        public bool HasPartialAssemblyName
        {
            get { return assemblyName.FullName == assemblyName.Name; }
        }

        /// <summary>
        /// Returns a type name that has a partial assembly name instead of the full assembly name.
        /// </summary>
        /// <returns>The type name</returns>
        public TypeName ConvertToPartialAssemblyName()
        {
            return HasPartialAssemblyName ? this : new TypeName(fullName, new AssemblyName(assemblyName.Name));
        }
    }
}