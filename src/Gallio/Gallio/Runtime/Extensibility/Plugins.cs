using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    internal sealed class Plugins : IPlugins
    {
        private readonly Registry registry;

        public Plugins(Registry registry)
        {
            this.registry = registry;
        }

        public IPluginDescriptor this[string pluginId]
        {
            get
            {
                if (pluginId == null)
                    throw new ArgumentNullException("pluginId");

                return registry.DataBox.Read(data => data.GetPluginById(pluginId));
            }
        }

        public IEnumerator<IPluginDescriptor> GetEnumerator()
        {
            return registry.DataBox.Read(data => data.GetPlugins()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
