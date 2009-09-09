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
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Extensibility
{
    internal sealed class PluginDescriptor : IPluginDescriptor
    {
        private static IHandlerFactory traitsHandlerFactory = new SingletonHandlerFactory();

        private readonly Registry registry;
        private readonly string pluginId;
        private readonly TypeName pluginTypeName;
        private readonly DirectoryInfo baseDirectory;
        private readonly PropertySet pluginProperties;
        private readonly PropertySet traitsProperties;
        private readonly IHandlerFactory pluginHandlerFactory;
        private readonly ReadOnlyCollection<AssemblyBinding> assemblyBindings;
        private readonly ReadOnlyCollection<IPluginDescriptor> pluginDependencies;
        private readonly ReadOnlyCollection<string> probingPaths;
        private readonly Condition enableCondition;
        private readonly string recommendedInstallationPath;
        private readonly ReadOnlyCollection<string> filePaths;

        private Type pluginType;
        private IHandler pluginHandler;
        private IHandler traitsHandler;
        private string disabledReason;

        public PluginDescriptor(Registry registry, PluginRegistration pluginRegistration, IList<IPluginDescriptor> completePluginDependenciesCopy)
        {
            this.registry = registry;
            pluginId = pluginRegistration.PluginId;
            pluginTypeName = pluginRegistration.PluginTypeName;
            baseDirectory = pluginRegistration.BaseDirectory;
            pluginProperties = pluginRegistration.PluginProperties.Copy().AsReadOnly();
            traitsProperties = pluginRegistration.TraitsProperties.Copy().AsReadOnly();
            pluginHandlerFactory = pluginRegistration.PluginHandlerFactory;
            assemblyBindings = new ReadOnlyCollection<AssemblyBinding>(GenericCollectionUtils.ToArray(pluginRegistration.AssemblyBindings));
            pluginDependencies = new ReadOnlyCollection<IPluginDescriptor>(completePluginDependenciesCopy);
            probingPaths = new ReadOnlyCollection<string>(GenericCollectionUtils.ToArray(pluginRegistration.ProbingPaths));
            enableCondition = pluginRegistration.EnableCondition;
            recommendedInstallationPath = pluginRegistration.RecommendedInstallationPath;
            filePaths = new ReadOnlyCollection<string>(GenericCollectionUtils.ToArray(pluginRegistration.FilePaths));
        }

        // Used by unit tests.
        internal static void RunWithInjectedTraitsHandlerFactoryMock(IHandlerFactory traitsHandlerFactory, Action action)
        {
            IHandlerFactory oldTraitsHandlerFactory = traitsHandlerFactory;
            try
            {
                PluginDescriptor.traitsHandlerFactory = traitsHandlerFactory;
                action();
            }
            finally
            {
                PluginDescriptor.traitsHandlerFactory = oldTraitsHandlerFactory;
            }
        }

        public string PluginId
        {
            get { return pluginId; }
        }

        public TypeName PluginTypeName
        {
            get { return pluginTypeName; }
        }

        public IHandlerFactory PluginHandlerFactory
        {
            get { return pluginHandlerFactory; }
        }

        public DirectoryInfo BaseDirectory
        {
            get { return baseDirectory; }
        }

        public IList<AssemblyBinding> AssemblyBindings
        {
            get { return assemblyBindings; }
        }

        public PropertySet PluginProperties
        {
            get { return pluginProperties; }
        }

        public PropertySet TraitsProperties
        {
            get { return traitsProperties; }
        }

        public bool IsDisabled
        {
            get
            {
                return FirstDisabledPluginDependency != null || disabledReason != null;
            }
        }

        public string DisabledReason
        {
            get
            {
                var firstDisabledPluginDependency = FirstDisabledPluginDependency;
                if (firstDisabledPluginDependency != null)
                    return string.Format("The plugin depends on another disabled plugin.  Reason: {0}", firstDisabledPluginDependency.DisabledReason);

                if (disabledReason == null)
                    throw new InvalidOperationException("The plugin has not been disabled.");

                return disabledReason;
            }
        }

        private IPluginDescriptor FirstDisabledPluginDependency
        {
            get { return GenericCollectionUtils.Find(pluginDependencies, p => p.IsDisabled); }
        }

        public IList<IPluginDescriptor> PluginDependencies
        {
            get { return pluginDependencies; }
        }

        public IList<string> ProbingPaths
        {
            get { return probingPaths; }
        }

        public Condition EnableCondition
        {
            get { return enableCondition; }
        }

        public string RecommendedInstallationPath
        {
            get { return recommendedInstallationPath; }
        }

        public IList<string> FilePaths
        {
            get { return filePaths; }
        }

        public IEnumerable<string> GetSearchPaths(string resourcePath)
        {
            if (resourcePath != null && resourcePath.Length == 0)
                throw new ArgumentException(
                    "Resource path must not be empty.  Use null to get search paths without a particular resource specified.",
                    "resourcePath");

            return ResourceSearchRules.GetSearchPaths(baseDirectory, probingPaths, resourcePath);
        }

        public Type ResolvePluginType()
        {
            if (pluginType == null)
            {
                try
                {
                    pluginType = pluginTypeName.Resolve();
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not resolve the plugin type of plugin '{0}'.", pluginId), ex);
                }
            }

            return pluginType;
        }

        public IHandler ResolvePluginHandler()
        {
            if (pluginHandler == null)
            {
                try
                {
                    Type contractType = typeof (IPlugin);
                    Type objectType = ResolvePluginType();
                    registry.DataBox.Write(data =>
                    {
                        if (pluginHandler == null)
                            pluginHandler = pluginHandlerFactory.CreateHandler(
                                new PluginDependencyResolver(this),
                                contractType, objectType, pluginProperties);
                    });
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not resolve the plugin handler of plugin '{0}'.", pluginId), ex);
                }
            }

            return pluginHandler;
        }

        public IPlugin ResolvePlugin()
        {
            try
            {
                return (IPlugin) ResolvePluginHandler().Activate();
            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Could not resolve instance of plugin '{0}'.", pluginId), ex);
            }
        }

        public IHandler ResolveTraitsHandler()
        {
            if (traitsHandler == null)
            {
                try
                {
                    Type contractType = typeof(PluginTraits);
                    Type objectType = typeof(PluginTraits);
                    registry.DataBox.Write(data =>
                    {
                        if (traitsHandler == null)
                            traitsHandler = traitsHandlerFactory.CreateHandler(
                                new DefaultObjectDependencyResolver(registry.ServiceLocator, registry.ResourceLocator),
                                contractType, objectType, traitsProperties);
                    });
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not resolve the traits handler of plugin '{0}'.", pluginId), ex);
                }
            }

            return traitsHandler;
        }

        public PluginTraits ResolveTraits()
        {
            try
            {
                return (PluginTraits)ResolveTraitsHandler().Activate();
            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Could not resolve traits of plugin '{0}'.", pluginId), ex);
            }
        }

        public void Disable(string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            disabledReason = reason;
        }

        private sealed class PluginDependencyResolver : DefaultObjectDependencyResolver
        {
            private readonly PluginDescriptor descriptor;

            public PluginDependencyResolver(PluginDescriptor descriptor)
                : base(descriptor.registry.ServiceLocator, descriptor.registry.ResourceLocator)
            {
                this.descriptor = descriptor;
            }

            public override DependencyResolution ResolveDependency(string parameterName, Type parameterType, string configurationArgument)
            {
                if (configurationArgument == null)
                {
                    if (typeof (Traits).IsAssignableFrom(parameterType) // check we want traits
                        && parameterType.IsAssignableFrom(typeof (PluginTraits))) // check we can handle the traits
                        return DependencyResolution.Satisfied(descriptor.ResolveTraits());

                    if (parameterType == typeof (IPluginDescriptor))
                        return DependencyResolution.Satisfied(descriptor);
                }

                return base.ResolveDependency(parameterName, parameterType, configurationArgument);
            }
        }
    }
}
