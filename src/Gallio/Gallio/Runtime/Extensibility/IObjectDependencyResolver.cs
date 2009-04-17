using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Resolves object dependencies and parameters.
    /// </summary>
    public interface IObjectDependencyResolver
    {
        /// <summary>
        /// Resolves a dependency.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterType">The parameter type</param>
        /// <param name="configurationArgument">An optional configuration argument that supplies a value
        /// for the parameter or describes the means by which the value should be obtained, or null if none</param>
        /// <returns>An object that describes whether the dependency was satisfied and the value it was assigned</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameterName"/> or 
        /// <paramref name="parameterType"/> is null</exception>
        DependencyResolution ResolveDependency(string parameterName, Type parameterType, string configurationArgument);
    }
}
