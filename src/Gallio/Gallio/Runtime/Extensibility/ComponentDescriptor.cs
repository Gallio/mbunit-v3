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
using System.Text;
using System.Threading;
using Gallio.Collections;
using Gallio.Reflection;

namespace Gallio.Runtime.Extensibility
{
    internal sealed class ComponentDescriptor : IComponentDescriptor
    {
        private readonly Registry registry;
        private readonly PluginDescriptor pluginDescriptor;
        private readonly ServiceDescriptor serviceDescriptor;
        private readonly string componentId;
        private readonly TypeName componentTypeName;
        private readonly PropertySet componentProperties;
        private readonly PropertySet traitsProperties;
        private readonly IHandlerFactory componentHandlerFactory;

        private Type componentType;
        private IHandler componentHandler;
        private IHandler traitsHandler;

        public ComponentDescriptor(Registry registry, ComponentRegistration componentRegistration)
        {
            this.registry = registry;
            pluginDescriptor = (PluginDescriptor) componentRegistration.Plugin;
            serviceDescriptor = (ServiceDescriptor) componentRegistration.Service;
            componentId = componentRegistration.ComponentId;
            componentTypeName = componentRegistration.ComponentTypeName;
            componentProperties = componentRegistration.ComponentProperties.Copy().AsReadOnly();
            traitsProperties = componentRegistration.TraitsProperties.Copy().AsReadOnly();
            componentHandlerFactory = componentRegistration.ComponentHandlerFactory;
        }

        public IPluginDescriptor Plugin
        {
            get { return pluginDescriptor; }
        }

        public IServiceDescriptor Service
        {
            get { return serviceDescriptor; }
        }

        public string ComponentId
        {
            get { return componentId; }
        }

        public TypeName ComponentTypeName
        {
            get { return componentTypeName; }
        }

        public IHandlerFactory ComponentHandlerFactory
        {
            get { return componentHandlerFactory; }
        }

        public PropertySet ComponentProperties
        {
            get { return componentProperties; }
        }

        public PropertySet TraitsProperties
        {
            get { return traitsProperties; }
        }

        public bool IsDisabled
        {
            get { return pluginDescriptor.IsDisabled || serviceDescriptor.IsDisabled; }
        }

        public string DisabledReason
        {
            get
            {
                if (pluginDescriptor.IsDisabled)
                    return string.Format("The plugin that provides this component was disabled.  Reason: {0}", pluginDescriptor.DisabledReason);
                if (serviceDescriptor.IsDisabled)
                    return string.Format("The service implemented by this component was disabled.  Reason: {0}", serviceDescriptor.DisabledReason);

                throw new InvalidOperationException("The component has not been disabled.");
            }
        }

        public Type ResolveComponentType()
        {
            if (componentType == null)
            {
                try
                {
                    componentType = componentTypeName.Resolve();
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not resolve the component type of component '{0}'.", componentId), ex);
                }
            }

            return componentType;
        }

        public IHandler ResolveComponentHandler()
        {
            if (componentHandler == null)
            {
                try
                {
                    Type contractType = serviceDescriptor.ResolveServiceType();
                    Type objectType = ResolveComponentType();
                    registry.DataBox.Write(data =>
                    {
                        if (componentHandler == null)
                            componentHandler = componentHandlerFactory.CreateHandler(registry.ServiceLocator,
                                pluginDescriptor.ResourceLocator,
                                contractType, objectType, componentProperties);
                    });
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not resolve the component handler of component '{0}'.", componentId), ex);
                }
            }

            return componentHandler;
        }

        public object ResolveComponent()
        {
            try
            {
                return ResolveComponentHandler().Activate();
            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Could not resolve instance of component '{0}'.", componentId), ex);
            }
        }

        public IHandler ResolveTraitsHandler()
        {
            if (traitsHandler == null)
            {
                try
                {
                    Type contractType = typeof(Traits);
                    Type objectType = serviceDescriptor.ResolveTraitsType();
                    registry.DataBox.Write(data =>
                    {
                        if (traitsHandler == null)
                            traitsHandler = serviceDescriptor.TraitsHandlerFactory.CreateHandler(registry.ServiceLocator,
                                pluginDescriptor.ResourceLocator,
                                contractType, objectType, traitsProperties);
                    });
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not resolve the traits handler of component '{0}'.", componentId), ex);
                }
            }

            return traitsHandler;
        }

        public Traits ResolveTraits()
        {
            try
            {
                return (Traits) ResolveTraitsHandler().Activate();
            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Could not resolve traits of component '{0}'.", componentId), ex);
            }
        }
    }
}
