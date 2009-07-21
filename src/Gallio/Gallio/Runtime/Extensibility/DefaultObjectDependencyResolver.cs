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
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;

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
    /// <item>If the parameter type is <see cref="ComponentHandle{TService,TTraits}" /> or an array
    /// of that type and the service locator can resolve an a service of the requested type,
    /// then the associated services will be resolve and injected as component handles.</item>
    /// </list>
    /// </para>
    /// <para>
    /// To convert string configuration arguments to the parameter type, the object factory applies the following rules:
    /// <list type="bullet">
    /// <item>If the parameter type is an array type, then the configuration argument is split on semicolons
    /// (';') and each part is independently converted to a value of the array's element type and then
    /// packaged into an array.</item>
    /// <item>If the parameter type is a string, then no conversion is required so the value is used as-is.</item>
    /// <item>If the value looks like "${component.id}" and there is a component registered with the specified
    /// component id that satisfies the service described by the parameter type then that component is injected.
    /// Similarly if the parameter is of type <see cref="ComponentHandle{TService,TTraits}"/> and the component
    /// implements the requested service type then a handle of that component is injected.</item>
    /// <item>If the parameter type is an enum then the value is parsed to a 
    /// value of that enum type, case-insensitively.</item>
    /// <item>If the parameter type is <see cref="Version" /> then the value is parsed into a version.</item>
    /// <item>If the parameter type is <see cref="Guid" /> then the value is parsed into a guid.</item>
    /// <item>If the parameter type is <see cref="Condition" /> then the value is parsed into a condition.</item>
    /// <item>If the parameter type is <see cref="Image" /> then the value is treated as a Uri
    /// to a resource and the image is loaded from the resource locator.</item>
    /// <item>If the parameter type is <see cref="Icon" /> then the value is treated as a Uri
    /// to a resource and the icon is loaded from the resource locator.</item>
    /// <item>If the parameter type is <see cref="FileInfo" /> or <see cref="DirectoryInfo"/> then the
    /// value is treated as a Uri to a file or directory resource and an instance of the
    /// appropriate file/directory info type is injected using the full path obtained from the resource locator.</item>
    /// <item>If the parameter type is <see cref="Assembly" /> then the value is interpreted the name of an assembly which is loaded using <see cref="Assembly.Load(string)" />.</item>
    /// <item>If the parameter type is <see cref="Type" /> then the value is interpereted as the assembly-qualified name of a type which is obtained using <see cref="Type.GetType(string)" />.</item>
    /// <item>If the parameter type is <see cref="AssemblyName" /> then the value is parsed into an assembly name.</item>
    /// <item>If the parameter type is <see cref="AssemblySignature" /> then the value is parsed into an assembly signature.</item>
    /// <item>Otherwise, the value is converted using <see cref="Convert.ChangeType(object, Type)" /> if possible.</item>
    /// </list>
    /// </para>
    /// <para>
    /// For information about how the Uri is resolved to a path, see <see cref="IResourceLocator.ResolveResourcePath"/>.
    /// </para>
    /// </remarks>
    public class DefaultObjectDependencyResolver : IObjectDependencyResolver
    {
        private readonly IServiceLocator serviceLocator;
        private readonly IResourceLocator resourceLocator;

        /// <summary>
        /// Creates an object dependency resolver.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="resourceLocator">The resource locator.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceLocator"/>,
        /// <paramref name="resourceLocator"/> is null.</exception>
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
        public virtual DependencyResolution ResolveDependency(string parameterName, Type parameterType, string configurationArgument)
        {
            if (parameterName == null)
                throw new ArgumentNullException("parameterName");
            if (parameterType == null)
                throw new ArgumentNullException("parameterType");

            if (configurationArgument != null)
                return ResolveDependencyByConfiguration(parameterName, parameterType, configurationArgument);

            return ResolveDependencyByServiceLocation(parameterName, parameterType);
        }

        private DependencyResolution ResolveDependencyByConfiguration(string parameterName, Type parameterType, string configurationArgument)
        {
            configurationArgument = configurationArgument.Trim();

            if (parameterType.IsArray)
            {
                // TODO: Should really provide a better way to extract structural information from the configuration argument.
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

        private DependencyResolution ResolveDependencyByServiceLocation(string parameterName, Type parameterType)
        {
            if (parameterType.IsArray)
            {
                Type elementType = parameterType.GetElementType();

                if (ComponentHandle.IsComponentHandleType(elementType))
                {
                    Type serviceType = GetServiceTypeFromComponentHandleType(elementType);
                    if (serviceLocator.HasService(serviceType))
                    {
                        IList<ComponentHandle> componentHandles = serviceLocator.ResolveAllHandles(serviceType);
                        return DependencyResolution.Satisfied(CreateArray(elementType, componentHandles));
                    }
                }
                else
                {
                    Type serviceType = elementType;
                    if (serviceLocator.HasService(serviceType))
                    {
                        IList<object> components = serviceLocator.ResolveAll(serviceType);
                        return DependencyResolution.Satisfied(CreateArray(serviceType, components));
                    }
                }
            }
            else
            {
                if (ComponentHandle.IsComponentHandleType(parameterType))
                {
                    Type serviceType = GetServiceTypeFromComponentHandleType(parameterType);
                    if (serviceLocator.HasService(serviceType))
                    {
                        ComponentHandle componentHandle = serviceLocator.ResolveHandle(serviceType);
                        return DependencyResolution.Satisfied(componentHandle);
                    }
                }
                else
                {
                    Type serviceType = parameterType;
                    if (serviceLocator.HasService(serviceType))
                    {
                        object component = serviceLocator.Resolve(parameterType);
                        return DependencyResolution.Satisfied(component);
                    }
                }
            }

            return DependencyResolution.Unsatisfied();
        }

        private static Type GetServiceTypeFromComponentHandleType(Type componentHandleType)
        {
            if (!componentHandleType.IsGenericType)
                throw new RuntimeException("Could not detect service type from non-generic component handle.");

            return componentHandleType.GetGenericArguments()[0];
        }

        // TODO: Refactor me.
        private object ConvertConfigurationArgumentToType(Type type, string value)
        {
            if (type == typeof(string))
                return value;

            if (type.IsEnum)
                return Enum.Parse(type, value, true);

            if (type == typeof(Version))
                return new Version(value);

            if (type == typeof(Guid))
                return new Guid(value);

            if (type == typeof(Condition))
                return Condition.Parse(value);

            if (type == typeof(Image))
                return Image.FromFile(ResolveResourcePath(value));

            if (type == typeof(Icon))
                return new Icon(ResolveResourcePath(value));

            if (type == typeof(FileInfo))
                return new FileInfo(ResolveResourcePath(value));

            if (type == typeof(DirectoryInfo))
                return new DirectoryInfo(ResolveResourcePath(value));

            if (type == typeof(AssemblyName))
                return new AssemblyName(value);

            if (type == typeof(AssemblySignature))
                return AssemblySignature.Parse(value);

            if (type == typeof(Assembly))
                return Assembly.Load(value);

            if (type == typeof(Type))
                return Type.GetType(value);

            if (value.StartsWith("${") && value.EndsWith("}"))
            {
                string componentId = value.Substring(2, value.Length - 3);

                object componentOrHandle;
                if (ComponentHandle.IsComponentHandleType(type))
                    componentOrHandle = serviceLocator.ResolveHandleByComponentId(componentId);
                else
                    componentOrHandle = serviceLocator.ResolveByComponentId(componentId);

                if (!type.IsInstanceOfType(componentOrHandle))
                    throw new RuntimeException(string.Format("Could not inject component with id '{0}' into a dependency of type '{1}' because it is of the wrong type even though the component was explicitly specified using the '${{component.id}}' property value syntax.",
                        componentId, type));

                return componentOrHandle;
            }

            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        private string ResolveResourcePath(string uri)
        {
            return resourceLocator.ResolveResourcePath(new Uri(uri));
        }

        private static Array CreateArray<T>(Type elementType, IList<T> values)
        {
            Array array = Array.CreateInstance(elementType, values.Count);

            for (int i = 0; i < values.Count; i++)
                array.SetValue(values[i], i);

            return array;
        }
    }
}
