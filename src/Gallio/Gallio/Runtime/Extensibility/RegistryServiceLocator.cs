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

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A service locator implementation based on a registry.
    /// </summary>
    public class RegistryServiceLocator : IServiceLocator
    {
        private readonly IRegistry registry;

        /// <summary>
        /// Creates a service locator based on a registry.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registry"/> is null.</exception>
        public RegistryServiceLocator(IRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException("registry");

            this.registry = registry;
        }

        /// <summary>
        /// Gets the registry.
        /// </summary>
        public IRegistry Registry
        {
            get { return registry; }
        }

        /// <inheritdoc />
        public TService Resolve<TService>()
        {
            return (TService)ResolveImpl(typeof(TService));
        }

        /// <inheritdoc />
        public object Resolve(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return ResolveImpl(serviceType);
        }

        /// <inheritdoc />
        public IList<TService> ResolveAll<TService>()
        {
            return ResolveAllImpl<TService>(typeof(TService));
        }

        /// <inheritdoc />
        public IList<object> ResolveAll(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return ResolveAllImpl<object>(serviceType);
        }

        /// <inheritdoc />
        public object ResolveByComponentId(string componentId)
        {
            if (componentId == null)
                throw new ArgumentNullException("componentId");

            IComponentDescriptor descriptor = ResolveNonDisabledDescriptorByComponentId(componentId);
            return descriptor.ResolveComponent();
        }

        /// <inheritdoc />
        public ComponentHandle<TService, TTraits> ResolveHandle<TService, TTraits>() where TTraits : Traits
        {
            IComponentDescriptor descriptor = ResolveNonDisabledDescriptor(typeof(TService));
            return ComponentHandle.CreateInstance<TService, TTraits>(descriptor);
        }

        /// <inheritdoc />
        public ComponentHandle ResolveHandle(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            IComponentDescriptor descriptor = ResolveNonDisabledDescriptor(serviceType);
            return ComponentHandle.CreateInstance(descriptor);
        }

        /// <inheritdoc />
        public IList<ComponentHandle<TService, TTraits>> ResolveAllHandles<TService, TTraits>() where TTraits : Traits
        {
            var result = new List<ComponentHandle<TService, TTraits>>();
            foreach (IComponentDescriptor descriptor in ResolveAllNonDisabledDescriptors(typeof(TService)))
                result.Add(ComponentHandle.CreateInstance<TService, TTraits>(descriptor));

            return result;
        }

        /// <inheritdoc />
        public IList<ComponentHandle> ResolveAllHandles(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            var result = new List<ComponentHandle>();
            foreach (IComponentDescriptor descriptor in ResolveAllNonDisabledDescriptors(serviceType))
                result.Add(ComponentHandle.CreateInstance(descriptor));

            return result;
        }

        /// <inheritdoc />
        public ComponentHandle ResolveHandleByComponentId(string componentId)
        {
            if (componentId == null)
                throw new ArgumentNullException("componentId");

            IComponentDescriptor descriptor = ResolveNonDisabledDescriptorByComponentId(componentId);
            return ComponentHandle.CreateInstance(descriptor);
        }

        /// <inheritdoc />
        public bool HasService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            IServiceDescriptor descriptor = registry.Services.GetByServiceType(serviceType);
            return descriptor != null && !descriptor.IsDisabled;
        }

        /// <inheritdoc />
        public bool HasComponent(string componentId)
        {
            if (componentId == null)
                throw new ArgumentNullException("componentId");

            IComponentDescriptor descriptor = registry.Components[componentId];
            return descriptor != null && !descriptor.IsDisabled;
        }

        private object ResolveImpl(Type serviceType)
        {
            return ResolveNonDisabledDescriptor(serviceType).ResolveComponent();
        }

        private IList<TService> ResolveAllImpl<TService>(Type serviceType)
        {
            var result = new List<TService>();
            foreach (IComponentDescriptor descriptor in ResolveAllNonDisabledDescriptors(serviceType))
                result.Add((TService)descriptor.ResolveComponent());

            return result;
        }

        private IComponentDescriptor ResolveNonDisabledDescriptorByComponentId(string componentId)
        {
            IComponentDescriptor descriptor = registry.Components[componentId];
            if (descriptor == null)
                throw new RuntimeException(string.Format("Could not resolve component with id '{0}' because it does not appear to be registered.", componentId));
            if (descriptor.IsDisabled)
                throw new RuntimeException(string.Format("Could not resolve component with id '{0}' because it has been disabled.  Reason: {1}", componentId, descriptor.DisabledReason));

            return descriptor;
        }

        private IEnumerable<IComponentDescriptor> ResolveAllNonDisabledDescriptors(Type serviceType)
        {
            foreach (IComponentDescriptor descriptor in registry.Components.FindByServiceType(serviceType))
                if (!descriptor.IsDisabled)
                    yield return descriptor;
        }

        private IComponentDescriptor ResolveNonDisabledDescriptor(Type serviceType)
        {
            IEnumerable<IComponentDescriptor> descriptors = ResolveAllNonDisabledDescriptors(serviceType);
            IEnumerator<IComponentDescriptor> descriptorEnumerator = descriptors.GetEnumerator();

            if (!descriptorEnumerator.MoveNext())
                throw new RuntimeException(string.Format("Could not resolve component for service type '{0}' because there do not appear to be any components registered and enabled for that service type.", serviceType));

            IComponentDescriptor descriptor = descriptorEnumerator.Current;

            if (descriptorEnumerator.MoveNext())
                throw new RuntimeException(string.Format("Could not resolve component for service type '{0}' because there are more than one of them registered and enabled so the request is ambiguous.", serviceType));

            return descriptor;
        }
    }
}
