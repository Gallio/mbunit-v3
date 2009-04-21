using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Resolves object dependencies that are discovered during dependency injection
    /// with the help of a service locator, a resource locator, and a built in set of conversions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To resolve dependencies, the object dependency resolver applies the following rules:
    /// <list type="bullet">
    /// <item>If a configuration argument value was provided then it is converted to an instance
    /// of the parameter type (see below).</item>
    /// <item>If the service locator can resolve a service of the parameter type, or if the parameter type
    /// is an array and the service locator can resolve one or more services of the array's element type,
    /// then those services will be resolved and injected.</item>
    /// </list>
    /// </para>
    /// <para>
    /// To convert dtring configuration arguments to the parameter type, the object factory applies the following rules:
    /// <list type="bullet">
    /// <item>If the parameter type is an array type, then the configuration argument is split on semicolons
    /// (';') and each part is independently converted to a value of the array's element type and then
    /// packaged into an array.</item>
    /// <item>If the parameter type is a string, then no conversion is required so the value is used as-is.</item>
    /// <item>If the value looks like "${component.id}" and there is a component registered with the specified
    /// component id that satisfies the parameter type then that component is used.</item>
    /// <item>If the parameter type is an enum then the value is parsed to a 
    /// value of that enum type, case-insensitively.</item>
    /// <item>If the parameter type is <see cref="Image" /> then the value is treated as a relative file
    /// path to a resource and the image is loaded from the resource locator.</item>
    /// <item>If the parameter type is <see cref="Icon" /> then the value is treated as a relative file
    /// path to a resource and the icon is loaded from the resource locator.</item>
    /// <item>If the parameter type is <see cref="FileInfo" /> or <see cref="DirectoryInfo"/> then the
    /// value is treated as a relative file or directory path to a resource and an instance of the
    /// appropriate file/directory info type is injected using the full path obtained from the resource locator.</item>
    /// <item>Otherwise, the value is converted using <see cref="Convert.ChangeType(object, Type)" /> if possible.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class DefaultObjectDependencyResolver : IObjectDependencyResolver
    {
        private readonly IServiceLocator serviceLocator;
        private readonly IResourceLocator resourceLocator;

        /// <summary>
        /// Creates an object dependency resolver.
        /// </summary>
        /// <param name="serviceLocator">The service locator</param>
        /// <param name="resourceLocator">The resource locator</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceLocator"/>,
        /// <paramref name="resourceLocator"/> is null</exception>
        public DefaultObjectDependencyResolver(IServiceLocator serviceLocator, IResourceLocator resourceLocator)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            if (resourceLocator == null)
                throw new ArgumentNullException("resourceLocator");

            this.serviceLocator = serviceLocator;
            this.resourceLocator = resourceLocator;
        }

        /// <inheritdoc />
        public DependencyResolution ResolveDependency(string parameterName, Type parameterType, string configurationArgument)
        {
            if (parameterName == null)
                throw new ArgumentNullException("parameterName");
            if (parameterType == null)
                throw new ArgumentNullException("parameterType");

            // Resolve by property
            if (configurationArgument != null)
            {
                if (parameterType.IsArray)
                {
                    // TODO: Should really provide a better way to extract structural information.
                    Type type = parameterType.GetElementType();
                    object[] values = Array.ConvertAll(configurationArgument.Split(';'),
                        value => ConvertConfigurationArgumentToType(type, value));
                    return DependencyResolution.Satisfied(CreateArray(type, values));
                }
                else
                {
                    object value = ConvertConfigurationArgumentToType(parameterType, configurationArgument);
                    return DependencyResolution.Satisfied(value);
                }
            }

            if (parameterType.IsArray)
            {
                // Resolve component array
                Type componentType = parameterType.GetElementType();
                if (serviceLocator.CanResolve(componentType))
                {
                    IList<object> components = serviceLocator.ResolveAll(componentType);
                    return DependencyResolution.Satisfied(CreateArray(componentType, components));
                }
            }
            else
            {
                // Resolve component
                if (serviceLocator.CanResolve(parameterType))
                {
                    object component = serviceLocator.Resolve(parameterType);
                    return DependencyResolution.Satisfied(component);
                }
            }

            return DependencyResolution.Unsatisfied();
        }

        private object ConvertConfigurationArgumentToType(Type type, string value)
        {
            if (type == typeof(string))
                return value;

            if (value.StartsWith("${") && value.EndsWith("}"))
            {
                string componentId = value.Substring(2, value.Length - 3);
                object component = serviceLocator.ResolveByComponentId(componentId);
                if (!type.IsInstanceOfType(component))
                    throw new RuntimeException(string.Format("Could not inject component with id '{0}' into a dependency of type '{1}' because it is of the wrong type even though the component was explicitly specified using the '${{component.id}}' property value syntax.",
                        componentId, type));
                return component;
            }

            if (type.IsEnum)
                return Enum.Parse(type, value, true);

            if (type == typeof(Image))
                return Image.FromFile(resourceLocator.GetFullPath(value));

            if (type == typeof(Icon))
                return new Icon(resourceLocator.GetFullPath(value));

            if (type == typeof(FileInfo))
                return new FileInfo(resourceLocator.GetFullPath(value));

            if (type == typeof(DirectoryInfo))
                return new DirectoryInfo(resourceLocator.GetFullPath(value));

            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        private static Array CreateArray(Type elementType, IList<object> values)
        {
            Array array = Array.CreateInstance(elementType, values.Count);

            for (int i = 0; i < values.Count; i++)
                array.SetValue(values[i], i);

            return array;
        }
    }
}
