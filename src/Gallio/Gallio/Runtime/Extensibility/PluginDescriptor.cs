using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Gallio.Collections;
using Gallio.Reflection;

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
        private readonly IResourceLocator resourceLocator;

        private Type pluginType;
        private IHandler pluginHandler;
        private IHandler traitsHandler;

        public PluginDescriptor(Registry registry, PluginRegistration pluginRegistration)
        {
            this.registry = registry;
            pluginId = pluginRegistration.PluginId;
            pluginTypeName = pluginRegistration.PluginTypeName;
            baseDirectory = pluginRegistration.BaseDirectory;
            pluginProperties = pluginRegistration.PluginProperties.Copy().AsReadOnly();
            traitsProperties = pluginRegistration.TraitsProperties.Copy().AsReadOnly();
            pluginHandlerFactory = pluginRegistration.PluginHandlerFactory;
            resourceLocator = new FileSystemResourceLocator(baseDirectory);
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

        public PropertySet PluginProperties
        {
            get { return pluginProperties; }
        }

        public PropertySet TraitsProperties
        {
            get { return traitsProperties; }
        }

        public IResourceLocator ResourceLocator
        {
            get { return resourceLocator; }
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
                            pluginHandler = pluginHandlerFactory.CreateHandler(registry, resourceLocator,
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
                            traitsHandler = traitsHandlerFactory.CreateHandler(registry, resourceLocator,
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
    }
}
