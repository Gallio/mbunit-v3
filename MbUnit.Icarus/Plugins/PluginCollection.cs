using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Icarus.Plugins
{
    /// <summary>
    /// Collection for AvailablePlugin Type
    /// </summary>
    public class PluginCollection : CollectionBase
    {
        /// <summary>
        /// Add a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToAdd">The Plugin to Add</param>
        public void Add(AvailablePlugin pluginToAdd)
        {
            this.List.Add(pluginToAdd);
        }

        /// <summary>
        /// Remove a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToRemove">The Plugin to Remove</param>
        public void Remove(AvailablePlugin pluginToRemove)
        {
            this.List.Remove(pluginToRemove);
        }

        /// <summary>
        /// Finds a plugin in the available Plugins
        /// </summary>
        /// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
        /// <returns>Available Plugin, or null if the plugin is not found</returns>
        public AvailablePlugin Find(string pluginNameOrPath)
        {
            AvailablePlugin toReturn = null;

            //Loop through all the plugins
            foreach (AvailablePlugin pluginOn in this.List)
            {
                //Find the one with the matching name or filename
                if ((pluginOn.Instance.Name.Equals(pluginNameOrPath)) || pluginOn.AssemblyPath.Equals(pluginNameOrPath))
                {
                    toReturn = pluginOn;
                    break;
                }
            }
            return toReturn;
        }
    }
}
