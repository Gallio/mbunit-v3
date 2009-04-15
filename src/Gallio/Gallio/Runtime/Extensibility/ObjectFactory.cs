using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Collections;

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
    /// To resolve dependencies, the object factory applies the following rules:
    /// <list type="bullet">
    /// <item>If the property set contains a property whose name is (case insensitively) equal
    /// to that of the parameters or property into which the dependency is to be injected
    /// then the associated value is converted to an instance of the required type (see below).</item>
    /// <item>If the service locator can resolve a service of the type specified by
    /// the dependency, or if the type is an array and the service locator can resolve one or
    /// more services of the array's element type, then those services will be resolved and injected.</item>
    /// <item>If an optional dependency cannot be resolved then that dependency is ignored.
    /// If any required dependency cannot be satisifed then the object construction fails and a
    /// <see cref="RuntimeException" /> is thrown.</item>
    /// </list>
    /// </para>
    /// <para>
    /// To convert string property values to the dependency type, the object factory applies the following rules:
    /// <list type="bullet">
    /// <item>If the dependency type is a string, then no conversion is required so the value is used as-is.</item>
    /// <item>If the value looks like "${component.id}" and there is a component registered with the specified
    /// component id that satisfies the dependency type then that component is used.</item>
    /// <item>If the dependency type is <see cref="Image" /> then the value is treated as a relative file
    /// path to a resource and the image is loaded from the resource locator.</item>
    /// <item>If the dependency type is <see cref="Icon" /> then the value is treated as a relative file
    /// path to a resource and the icon is loaded from the resource locator.</item>
    /// <item>If the dependency type is <see cref="FileInfo" /> or <see cref="DirectoryInfo"/> then the
    /// value is treated as a relative file or directory path to a resource and an instance of the
    /// appropriate file/directory info type is injected using the full path obtained from the resource locator.</item>
    /// <item>Otherwise, the property value is converted using <see cref="Convert.ChangeType(object, Type)" /> if possible.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class ObjectFactory
    {
        private readonly IServiceLocator serviceLocator;
        private readonly IResourceLocator resourceLocator;
        private readonly Type objectType;
        private readonly PropertySet properties;

        /// <summary>
        /// Creates an object factory.
        /// </summary>
        /// <param name="serviceLocator">The service locator</param>
        /// <param name="resourceLocator">The resource locator</param>
        /// <param name="objectType">The object type</param>
        /// <param name="properties">The object properties</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceLocator"/>,
        /// <paramref name="resourceLocator"/>, <paramref name="objectType"/> or <paramref name="properties"/> is null</exception>
        public ObjectFactory(IServiceLocator serviceLocator, IResourceLocator resourceLocator, Type objectType, PropertySet properties)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            if (resourceLocator == null)
                throw new ArgumentNullException("resourceLocator");
            if (objectType == null)
                throw new ArgumentNullException("objectType");
            if (properties == null)
                throw new ArgumentNullException("properties");

            this.serviceLocator = serviceLocator;
            this.resourceLocator = resourceLocator;
            this.objectType = objectType;
            this.properties = CanonicalizePropertiesToLowerCase(properties);
        }

        /// <summary>
        /// Creates an instance of the object described by this factory.
        /// </summary>
        /// <returns>The instance, not null</returns>
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
            object[] values = GenericUtils.ConvertAllToArray(constructor.GetParameters(),
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
                    DependencyResult dependencyResult = ResolveDependency(property.Name, property.PropertyType, true);
                    if (dependencyResult.IsSatisfied)
                        optionalDependencies.Add(property, dependencyResult.Value);
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

        private DependencyResult ResolveDependency(string name, Type type, bool isOptional)
        {
            try
            {
                // Resolve by property
                string propertyValue;
                if (properties.TryGetValue(CanonicalizePropertyKey(name), out propertyValue))
                {
                    object value = ConvertPropertyValueToType(propertyValue, type);
                    return DependencyResult.Satisfied(value);
                }

                if (type.IsArray)
                {
                    // Resolve component array
                    Type componentType = type.GetElementType();
                    if (serviceLocator.CanResolve(componentType))
                    {
                        IList<object> components = serviceLocator.ResolveAll(componentType);
                        Array componentArray = Array.CreateInstance(componentType, components.Count);

                        for (int i = 0; i < components.Count; i++)
                            componentArray.SetValue(components, i);

                        return DependencyResult.Satisfied(componentArray);
                    }
                }
                else
                {
                    // Resolve component
                    if (serviceLocator.CanResolve(type))
                    {
                        object component = serviceLocator.Resolve(type);
                        return DependencyResult.Satisfied(component);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Could not resolve {2} dependency '{0}' of type '{1}' due to an exception.", name, type,
                    isOptional ? "optional" : "required"), ex);
            }

            if (!isOptional)
                throw new RuntimeException(string.Format("Could not resolve required dependency '{0}' of type '{1}'.", name, type));

            return DependencyResult.Unsatisfied();
        }

        private object ConvertPropertyValueToType(string propertyValue, Type type)
        {
            if (type == typeof(string))
                return propertyValue;

            if (propertyValue.StartsWith("${") && propertyValue.EndsWith("}"))
            {
                string componentId = propertyValue.Substring(2, propertyValue.Length - 3);
                object component = serviceLocator.ResolveByComponentId(componentId);
                if (!type.IsInstanceOfType(component))
                    throw new RuntimeException(string.Format("Could not inject component with id '{0}' into a dependency of type '{1}' because it is of the wrong type even though the component was explicitly specified using the '${{component.id}}' property value syntax.",
                        componentId, type));
                return component;
            }

            if (type == typeof(Image))
                return Image.FromFile(resourceLocator.GetFullPath(propertyValue));

            if (type == typeof(Icon))
                return new Icon(resourceLocator.GetFullPath(propertyValue));

            if (type == typeof(FileInfo))
                return new FileInfo(resourceLocator.GetFullPath(propertyValue));

            if (type == typeof(DirectoryInfo))
                return new DirectoryInfo(resourceLocator.GetFullPath(propertyValue));

            return Convert.ChangeType(propertyValue, type, CultureInfo.InvariantCulture);
        }

        private struct DependencyResult
        {
            private readonly bool isSatisfied;
            private readonly object value;

            private DependencyResult(bool isSatisfied, object value)
            {
                this.isSatisfied = isSatisfied;
                this.value = value;
            }

            public static DependencyResult Satisfied(object value)
            {
                return new DependencyResult(true, value);
            }

            public static DependencyResult Unsatisfied()
            {
                return new DependencyResult(false, null);
            }

            public bool IsSatisfied
            {
                get { return isSatisfied; }
            }

            public object Value
            {
                get
                {
                    if (!isSatisfied)
                        throw new InvalidOperationException("The dependency was not satisfied.");
                    return value;
                }
            }
        }
    }
}
