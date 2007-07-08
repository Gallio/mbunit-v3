using System;
using System.Globalization;
using System.Reflection;

namespace MbUnit.Core.Utilities
{
    /// <summary>
    /// Provides helpers for performing reflection.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Gets the attribute with the specified type.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="provider">The attribute provider</param>
        /// <returns>The attribute or null if none</returns>
        /// <exception cref="InvalidOperationException">Thrown if the attribute provider
        /// contains multiple attributes of the specified type</exception>
        public static T GetAttribute<T>(ICustomAttributeProvider provider) where T : class
        {
            object[] attribs = provider.GetCustomAttributes(typeof(T), true);

            if (attribs.Length == 0)
                return null;
            else if (attribs.Length == 1)
                return (T)attribs[0];
            else
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                    "There are multiple instances of attribute '{0}'.", typeof(T).FullName)); 
        }

        /// <summary>
        /// Finds the referenced assembly name with the specified display name.
        /// Recursively searches referenced assemblies beginning with the one specified as a parameter.
        /// Loads referenced assemblies on demand to resolve names.
        /// </summary>
        /// <param name="assemblyName">The name of the first assembly to check</param>
        /// <param name="displayName">The display name of the assembly to search for</param>
        /// <returns>The referenced assembly name or null if none</returns>
        public static AssemblyName FindReferencedAssembly(AssemblyName assemblyName, string displayName)
        {
            if (assemblyName.Name == displayName)
                return assemblyName;

            Assembly assembly;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                // Ignore failures to load the referenced assembly.
                // Obvious the referenced assembly wasn't found there...
                return null;
            }

            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                AssemblyName result = FindReferencedAssembly(reference, displayName);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Determines if the type can be instantiated using a public constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type can be instantiated</returns>
        public static bool CanInstantiate(Type type)
        {
            return !type.IsAbstract && type.IsClass && type.IsPublic
                && !type.HasElementType && type.GetConstructors().Length != 0;
        }

        /// <summary>
        /// Determines if the method is public, non-static and is non-abstract so it can be invoked.
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>True if the method can be invoked</returns>
        public static bool CanInvokeNonStatic(MethodInfo method)
        {
            return !method.IsAbstract && method.IsPublic && !method.IsStatic;
        }

        /// <summary>
        /// Determines if the property has public, non-static and non-abstract getter
        /// and setter functions.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>True if the property can be get and set</returns>
        public static bool CanGetAndSetNonStatic(PropertyInfo property)
        {
            return property.CanRead && property.CanWrite
                && CanInvokeNonStatic(property.GetGetMethod())
                && CanInvokeNonStatic(property.GetSetMethod());
        }
    }
}
