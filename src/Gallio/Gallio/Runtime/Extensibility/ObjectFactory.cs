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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common.Collections;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Builds objects and performs dependency injection via a service locator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The object factory examines the object type to discover required and optional
    /// dependencies that are to be injected.  Required dependencies are specified by
    /// the parameters of the constructor with the most parmaeters.  Optional dependencies
    /// are specified by settable properties.
    /// </para>
    /// <para>
    /// If an optional dependency cannot be resolved then that dependency is ignored.
    /// If any required dependency cannot be satisifed then the object construction fails and a
    /// <see cref="RuntimeException" /> is thrown.
    /// </para>
    /// <para>
    /// To resolve each dependency, the factory consults an <see cref="IObjectDependencyResolver" />
    /// given the name of the parameter, its type, and the optional configuration property value specified
    /// in the object property set whose key matches the dependency parameter name case insensitively.
    /// </para>
    /// </remarks>
    public class ObjectFactory
    {
        private readonly IObjectDependencyResolver dependencyResolver;
        private readonly Type objectType;
        private readonly PropertySet properties;

        /// <summary>
        /// Creates an object factory.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <param name="objectType">The object type.</param>
        /// <param name="properties">The object properties.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dependencyResolver"/>,
        /// <paramref name="objectType"/> or <paramref name="properties"/> is null.</exception>
        public ObjectFactory(IObjectDependencyResolver dependencyResolver, Type objectType, PropertySet properties)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException("dependencyResolver");
            if (objectType == null)
                throw new ArgumentNullException("objectType");
            if (properties == null)
                throw new ArgumentNullException("properties");

            this.dependencyResolver = dependencyResolver;
            this.objectType = objectType;
            this.properties = CanonicalizePropertiesToLowerCase(properties);
        }

        /// <summary>
        /// Creates an instance of the object described by this factory.
        /// </summary>
        /// <returns>The instance, not null.</returns>
        public object CreateInstance()
        {
            if (objectType.IsAbstract)
                throw new RuntimeException(string.Format("Type '{0}' is abstract and cannot be instantiated.", objectType));

            ConstructorInfo constructor = FindDependencyInjectionConstructor();
            object[] requiredDependencies = ResolveRequiredDependencies(constructor);
            Dictionary<PropertyInfo, object> optionalDependencies = ResolveOptionalDependencies();

            object instance = constructor.Invoke(requiredDependencies);

            foreach (KeyValuePair<PropertyInfo, object> pair in optionalDependencies)
                pair.Key.SetValue(instance, pair.Value, null);

            return instance;
        }

        private static PropertySet CanonicalizePropertiesToLowerCase(PropertySet properties)
        {
            PropertySet result = new PropertySet();
            foreach (KeyValuePair<string, string> property in properties)
                result.Add(CanonicalizePropertyKey(property.Key), property.Value);

            return result;
        }

        private static string CanonicalizePropertyKey(string propertyKey)
        {
            return propertyKey.ToLowerInvariant();
        }

        // TODO: Consider satisfiability of constructors successively by decreasing number of parameters
        private ConstructorInfo FindDependencyInjectionConstructor()
        {
            ConstructorInfo[] constructors = objectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length == 0)
                throw new RuntimeException(string.Format("Type '{0}' does not have any public constructors.", objectType));

            Array.Sort(constructors, (x, y) => y.GetParameters().Length.CompareTo(x.GetParameters().Length));
            return constructors[0];
        }

        private object[] ResolveRequiredDependencies(ConstructorInfo constructor)
        {
            object[] values = GenericCollectionUtils.ConvertAllToArray(constructor.GetParameters(),
                parameter => ResolveDependency(parameter.Name, parameter.ParameterType, false).Value);
            return values;
        }

        private Dictionary<PropertyInfo, object> ResolveOptionalDependencies()
        {
            var optionalDependencies = new Dictionary<PropertyInfo, object>();

            foreach (PropertyInfo property in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsOptionalDependency(property))
                {
                    DependencyResolution dependencyResolution = ResolveDependency(property.Name, property.PropertyType, true);
                    if (dependencyResolution.IsSatisfied)
                        optionalDependencies.Add(property, dependencyResolution.Value);
                }
            }

            return optionalDependencies;
        }

        private static bool IsOptionalDependency(PropertyInfo property)
        {
            // Ensure there is a public set method.
            // We do this instead of using property.CanWrite because the latter also considers
            // properties with private set methods to be writeable.
            return property.GetSetMethod() != null;
        }

        private DependencyResolution ResolveDependency(string parameterName, Type parameterType, bool isOptional)
        {
            DependencyResolution resolution;
            try
            {
                string propertyValue;
                properties.TryGetValue(CanonicalizePropertyKey(parameterName), out propertyValue);

                resolution = dependencyResolver.ResolveDependency(parameterName, parameterType, propertyValue);
            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Could not resolve {2} dependency '{0}' of type '{1}' due to an exception.", parameterName, parameterType,
                    isOptional ? "optional" : "required"), ex);
            }

            if (! resolution.IsSatisfied && !isOptional)
                throw new RuntimeException(string.Format("Could not resolve required dependency '{0}' of type '{1}'.", parameterName, parameterType));

            return resolution;
        }
    }
}
