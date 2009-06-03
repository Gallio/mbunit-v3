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
using System.Reflection;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A handle for a lazily instantiated component and its traits.
    /// </summary>
    /// <seealso cref="ComponentHandle{TService, TTraits}"/>
    public abstract class ComponentHandle
    {
        private readonly IComponentDescriptor componentDescriptor;

        internal ComponentHandle(IComponentDescriptor componentDescriptor)
        {
            this.componentDescriptor = componentDescriptor;
        }

        /// <summary>
        /// Creates an instance of a typed component handle for the specified descriptor.
        /// </summary>
        /// <param name="componentDescriptor">The component descriptor.</param>
        /// <returns>The appropriately typed component handle.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentDescriptor"/> is null.</exception>
        /// <exception cref="RuntimeException">Thrown if the described component's service type or traits type cannot be resolved.</exception>
        public static ComponentHandle CreateInstance(IComponentDescriptor componentDescriptor)
        {
            if (componentDescriptor == null)
                throw new ArgumentNullException("componentDescriptor");

            Type serviceType = componentDescriptor.Service.ResolveServiceType();
            Type traitsType = componentDescriptor.Service.ResolveTraitsType();
            Type handleType = typeof(ComponentHandle<,>).MakeGenericType(serviceType, traitsType);
            ConstructorInfo constructor = handleType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(IComponentDescriptor) }, null);
            return (ComponentHandle) constructor.Invoke(new[] { componentDescriptor });
        }

        /// <summary>
        /// Creates an instance of a typed component handle for the specified descriptor.
        /// </summary>
        /// <param name="componentDescriptor">The component descriptor.</param>
        /// <returns>The appropriately typed component handle.</returns>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TTraits">The traits type.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentDescriptor"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the described component's service or traits are not compatible with a handle of this type.</exception>
        /// <exception cref="RuntimeException">Thrown if the described component's service type or traits type cannot be resolved.</exception>
        public static ComponentHandle<TService, TTraits> CreateInstance<TService, TTraits>(IComponentDescriptor componentDescriptor)
            where TTraits : Traits
        {
            if (componentDescriptor == null)
                throw new ArgumentNullException("componentDescriptor");

            Type serviceType = componentDescriptor.Service.ResolveServiceType();
            Type traitsType = componentDescriptor.Service.ResolveTraitsType();
            if (serviceType != typeof(TService) || traitsType != typeof(TTraits))
            {
                throw new ArgumentException(
                    "The component descriptor is not compatible with the requested component handle type because it has a different service type or traits type.",
                    "componentDescriptor");
            }

            return new ComponentHandle<TService, TTraits>(componentDescriptor);
        }

        /// <summary>
        /// Creates an instance of a typed component handle for the specified component and traits instance.
        /// The component handle will have a stub component descriptor for testing purposes.
        /// </summary>
        /// <param name="componentId">The component id.</param>
        /// <param name="component">The component instance.</param>
        /// <param name="traits">The component traits.</param>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TTraits">The traits type.</typeparam>
        /// <returns>The appropriately typed component handle.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentId"/>, <paramref name="component"/>
        /// or <paramref name="traits"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the described component's service or traits are not compatible with a handle of this type.</exception>
        /// <exception cref="RuntimeException">Thrown if the described component's service type or traits type cannot be resolved.</exception>
        public static ComponentHandle<TService, TTraits> CreateStub<TService, TTraits>(string componentId, TService component, TTraits traits)
            where TTraits : Traits
        {
            if (componentId == null)
                throw new ArgumentNullException("componentId");
            if (component == null)
                throw new ArgumentNullException("component");
            if (traits == null)
                throw new ArgumentNullException("traits");

            IComponentDescriptor componentDescriptor = new ComponentDescriptorStub(componentId, component, traits);
            return new ComponentHandle<TService, TTraits>(componentDescriptor);
        }

        /// <summary>
        /// Returns true if the specified type is a component handle type.
        /// </summary>
        /// <param name="type">The type to examine.</param>
        /// <returns>True if the type is a component handle type.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public static bool IsComponentHandleType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return type == typeof(ComponentHandle)
                || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ComponentHandle<,>);
        }

        /// <summary>
        /// Gets the component descriptor.
        /// </summary>
        public IComponentDescriptor Descriptor
        {
            get { return componentDescriptor; }
        }

        /// <summary>
        /// Gets the component id.
        /// </summary>
        public string Id
        {
            get { return componentDescriptor.ComponentId; }
        }

        /// <summary>
        /// Gets the service type.
        /// </summary>
        /// <returns>The service type.</returns>
        public abstract Type ServiceType { get; }

        /// <summary>
        /// Gets the traits type.
        /// </summary>
        /// <returns>The traits type.</returns>
        public abstract Type TraitsType { get; }

        /// <summary>
        /// Gets the component instance.
        /// </summary>
        /// <returns>The component instance.</returns>
        /// <exception cref="RuntimeException">Thrown if the component cannot be resolved.</exception>
        public object GetComponent()
        {
            return GetComponentImpl();
        }

        /// <summary>
        /// Gets the component traits.
        /// </summary>
        /// <returns>The component traits.</returns>
        /// <exception cref="RuntimeException">Thrown if the traits cannot be resolved.</exception>
        public Traits GetTraits()
        {
            return GetTraitsImpl();
        }

        /// <summary>
        /// Returns the component's id.
        /// </summary>
        /// <returns>The component id.</returns>
        public override string ToString()
        {
            return Id;
        }

        internal abstract object GetComponentImpl();
        internal abstract Traits GetTraitsImpl();

        private sealed class ComponentDescriptorStub : IComponentDescriptor
        {
            private readonly string componentId;
            private readonly object component;
            private readonly Traits traits;

            public ComponentDescriptorStub(string componentId, object component, Traits traits)
            {
                this.componentId = componentId;
                this.component = component;
                this.traits = traits;
            }

            public IPluginDescriptor Plugin
            {
                get { throw new NotSupportedException(); }
            }

            public IServiceDescriptor Service
            {
                get { throw new NotSupportedException(); }
            }

            public string ComponentId
            {
                get { return componentId; }
            }

            public TypeName ComponentTypeName
            {
                get { return new TypeName(component.GetType()); }
            }

            public IHandlerFactory ComponentHandlerFactory
            {
                get { throw new NotSupportedException(); }
            }

            public PropertySet ComponentProperties
            {
                get { throw new NotSupportedException(); }
            }

            public PropertySet TraitsProperties
            {
                get { throw new NotSupportedException(); }
            }

            public bool IsDisabled
            {
                get { return false; }
            }

            public string DisabledReason
            {
                get { throw new InvalidOperationException("The component has not been disabled."); }
            }

            public Type ResolveComponentType()
            {
                return component.GetType();
            }

            public IHandler ResolveComponentHandler()
            {
                throw new NotSupportedException();
            }

            public object ResolveComponent()
            {
                return component;
            }

            public IHandler ResolveTraitsHandler()
            {
                throw new NotSupportedException();
            }

            public Traits ResolveTraits()
            {
                return traits;
            }
        }
    }

    /// <summary>
    /// A typed handle for a lazily instantiated component and its traits.
    /// </summary>
    /// <typeparam name="TService">The type of service implemented by the component.</typeparam>
    /// <typeparam name="TTraits">The type of traits provided by the component.</typeparam>
    public sealed class ComponentHandle<TService, TTraits> : ComponentHandle
        where TTraits : Traits
    {
        private Memoizer<TService> instanceMemoizer = new Memoizer<TService>();
        private Memoizer<TTraits> traitsMemoizer = new Memoizer<TTraits>();

        internal ComponentHandle(IComponentDescriptor componentDescriptor)
            : base(componentDescriptor)
        {
        }

        /// <inheritdoc />
        public override Type ServiceType
        {
            get { return typeof(TService); }
        }

        /// <inheritdoc />
        public override Type TraitsType
        {
            get { return typeof(TTraits); }
        }

        /// <summary>
        /// Gets the component instance.
        /// </summary>
        /// <returns>The component instance.</returns>
        /// <exception cref="RuntimeException">Thrown if the component cannot be resolved.</exception>
        new public TService GetComponent()
        {
            return instanceMemoizer.Memoize(() => (TService)Descriptor.ResolveComponent());
        }

        /// <summary>
        /// Gets the component traits.
        /// </summary>
        /// <returns>The component traits.</returns>
        /// <exception cref="RuntimeException">Thrown if the traits cannot be resolved.</exception>
        new public TTraits GetTraits()
        {
            return traitsMemoizer.Memoize(() => (TTraits)Descriptor.ResolveTraits());
        }

        internal override object GetComponentImpl()
        {
            return GetComponent();
        }

        internal override Traits GetTraitsImpl()
        {
            return GetTraits();
        }
    }
}
