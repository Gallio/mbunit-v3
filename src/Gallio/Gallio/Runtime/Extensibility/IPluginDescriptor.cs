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
using System.IO;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Describes a plugin.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The descriptor is used to query declarative information about the plugin
    /// and to load associated code and resources.
    /// </para>
    /// </remarks>
    public interface IPluginDescriptor
    {
        /// <summary>
        /// Gets the plugin's id.
        /// </summary>
        string PluginId { get; }

        /// <summary>
        /// Gets the plugin type name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The plugin type should be a class that implements <see cref="IPlugin" />.
        /// </para>
        /// </remarks>
        TypeName PluginTypeName { get; }

        /// <summary>
        /// Gets the plugin handler factory.
        /// </summary>
        IHandlerFactory PluginHandlerFactory { get; }

        /// <summary>
        /// Gets the base directory that contains the plugin's definition and associated resources.
        /// </summary>
        DirectoryInfo BaseDirectory { get; }

        /// <summary>
        /// Gets the list of plugin assembly references.
        /// </summary>
        IList<AssemblyReference> AssemblyReferences { get; }

        /// <summary>
        /// Gets the plugin properties.
        /// </summary>
        PropertySet PluginProperties { get; }

        /// <summary>
        /// Gets the traits properties.
        /// </summary>
        PropertySet TraitsProperties { get; }

        /// <summary>
        /// Gets the plugin's resource locator.
        /// </summary>
        IResourceLocator ResourceLocator { get; }

        /// <summary>
        /// Returns true if the plugin is disabled.
        /// </summary>
        bool IsDisabled { get; }

        /// <summary>
        /// Gets the reason the plugin was disabled.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsDisabled" /> is false</exception>
        string DisabledReason { get; }

        /// <summary>
        /// Gets the list of plugins upon which this plugin depends directly or indirectly.
        /// </summary>
        IList<IPluginDescriptor> PluginDependencies { get; }

        /// <summary>
        /// Gets the list of additional relative or absolute probing paths in which to attempt to locate
        /// referenced assemblies.
        /// </summary>
        IList<string> ProbingPaths { get; }

        /// <summary>
        /// Gets an enumeration of all absolute paths to be searched in order to find a given resource.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The enumeration consists of all combinations of the base directory and probing paths, including
        /// the use of an optional "bin" directory.  For example, if the base directory is "C:\Base", and the
        /// probing paths list contains "Probing", and we are looking for "resource.dll" then this method will
        /// return the following paths: "C:\Base\resource.dll", "C:\Base\bin\resource.dll", "C:\Base\Probing\resource.dll"
        /// and "C:\Base\Probing\bin\resource.dll".
        /// </para>
        /// <para>
        /// If the resource path is absolute then the returned search paths will consist of just that absolute path.
        /// </para>
        /// </remarks>
        /// <param name="resourcePath">The relative or absolute path of the resource to find or null to return
        /// the paths of the search directories themselves</param>
        /// <returns>An enumeration of search paths</returns>
        IEnumerable<string> GetSearchPaths(string resourcePath);

        /*
        /// <summary>
        /// Gets a view of all components provided by the plugin.
        /// </summary>
        IComponents Components { get; }

        /// <summary>
        /// Gets a view of all services provided by the plugin.
        /// </summary>
        IServices Services { get; }
         */

        /// <summary>
        /// Resolves the plugin type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The plugin type should be a class that implements <see cref="IPlugin" />.
        /// </para>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The plugin type</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        Type ResolvePluginType();

        /// <summary>
        /// Resolves the plugin handler.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The plugin handler</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        IHandler ResolvePluginHandler();

        /// <summary>
        /// Resolves the plugin instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The plugin instance</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        IPlugin ResolvePlugin();

        /// <summary>
        /// Resolves the traits handler.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The traits handler</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        IHandler ResolveTraitsHandler();

        /// <summary>
        /// Resolves the plugin traits.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The plugin traits</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        PluginTraits ResolveTraits();

        /// <summary>
        /// Disables the plugin.
        /// </summary>
        /// <param name="reason">The reason the plugin was disabled</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reason"/> is null</exception>
        void Disable(string reason);
    }
}
