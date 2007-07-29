using System;
using System.Reflection;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="IModelComponent.CodeReference" />
    /// contains the specified assembly.
    /// </summary>
    [Serializable]
    public class AssemblyFilter<T> : Filter<T> where T : IModelComponent
    {
        private string assemblyName;

        /// <summary>
        /// Creates an assembly filter.
        /// </summary>
        /// <param name="assemblyName">The type name specified in one of the following ways.
        /// In each case, the assembly name must exactly match the value obtained via reflection
        /// on the assembly.
        /// <list type="bullet">
        /// <item>Full name as returned by <see cref="Assembly.FullName" /></item>
        /// <item>Display name (aka. partial name) as returned by <see cref="AssemblyName.Name" /></item>
        /// </list>
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null</exception>
        public AssemblyFilter(string assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            this.assemblyName = assemblyName;
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            Assembly assembly = value.CodeReference.Assembly;
            if (assembly == null)
                return false;

            return assembly.FullName == assemblyName
                || assembly.GetName().Name == assemblyName;
        }
    }
}