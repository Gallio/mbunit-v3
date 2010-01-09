// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Represents an activated plugin.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The runtime provides a default implementation of this interface for plugins.
    /// However, plugins authors may choose to provide a custom implementation
    /// to control the plugin lifecycle and provide additional services to the
    /// runtime.  Initialization concerns should be performed by the constructor
    /// and disposal should be performed by implementing <see cref="IDisposable" />.
    /// </para>
    /// <example>
    /// <code><![CDATA[
    /// public class MyPlugin : IPlugin, IDisposable
    /// {
    ///     // The constructor may specify dependencies on other services
    ///     // that are required for plugin activation.  The services will
    ///     // be injected as with other components.
    ///     public MyPlugin(IService1 service1, IService2 service2)
    ///     {
    ///         // custom initialization logic
    ///     }
    ///     
    ///     public void Dispose()
    ///     {
    ///         // custom dispose logic
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <para>
    /// </para>
    /// </remarks>
    [Traits(typeof(PluginTraits))]
    public interface IPlugin
    {
    }
}
