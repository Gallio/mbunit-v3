using System;
using System.IO;
using System.Reflection;

using MbUnit.Icarus.Plugins;

namespace MbUnit.Icarus.Plugins
{
    /// <summary>
    /// Summary description for PluginServices.
    /// </summary>
    public class PluginServices : IMbUnitPluginHost 
    {
        /// <summary>
        /// Constructor of the Class
        /// </summary>
        public PluginServices()
        {

        }

        #region Plugin Management 

        private PluginCollection colAvailablePlugins = new PluginCollection();

        /// <summary>
        /// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
        /// </summary>
        public PluginCollection AvailablePlugins
        {
            get { return colAvailablePlugins; }
            set { colAvailablePlugins = value; }
        }

        /// <summary>
        /// Searches the Application's Startup Directory for Plugins
        /// </summary>
        public void FindPlugins()
        {
            FindPlugins(AppDomain.CurrentDomain.BaseDirectory);
        }

        /// <summary>
        /// Searches the passed Path for Plugins
        /// </summary>
        /// <param name="Path">Directory to search for Plugins in</param>
        public void FindPlugins(string Path)
        {
            // First empty the collection, we're reloading them all
            colAvailablePlugins.Clear();

            if (Directory.Exists(Path))
            {
                // Go through all the files in the plugin directory.
                foreach (string fileOn in Directory.GetFiles(Path, "*.dll"))
                {
                    //FileInfo file = new FileInfo(fileOn);

                    //// Preliminary check, must be .dll
                    //if (file.Extension.Equals(".dll"))
                    //{
                        // Add the 'plugin'
                        this.AddPlugin(fileOn);
                    //}
                }
            }
        }

        /// <summary>
        /// Unloads and Closes all AvailablePlugins
        /// </summary>
        public void ClosePlugins()
        {
            foreach (AvailablePlugin pluginOn in colAvailablePlugins)
            {
                // Call the Dispose method incase the plugin has to do  any cleanup.
                pluginOn.Instance.Dispose();

                // After we give the plugin a chance to tidy up, get rid of it
                pluginOn.Instance = null;
            }

            // Finally, clear our collection of available plugins
            colAvailablePlugins.Clear();
        }

        private void AddPlugin(string FileName)
        {
            // Create a new assembly from the plugin file we're adding..
            Assembly pluginAssembly = Assembly.LoadFrom(FileName);

            // Next loop through all the Types found in the assembly
            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (pluginType.IsPublic && !pluginType.IsAbstract) // Only look at public, non abstract types
                {
                    // Gets a type object of the interface we need the plugins to match
                    Type typeInterface = pluginType.GetInterface("MbUnit.Icarus.Plugins.IMbUnitPlugin", true);

                    // Make sure the interface we want to use actually exists
                    if (typeInterface != null)
                    {
                        // Create a new available plugin since the type implements the IPlugin interface
                        AvailablePlugin newPlugin = new AvailablePlugin();

                        // Set the filename where we found it
                        newPlugin.AssemblyPath = FileName;

                        // Create a new instance and store the instance in the collection for later use
                        newPlugin.Instance = (IMbUnitPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));

                        // Set the Plugin's host to this class which inherits IPluginHost
                        newPlugin.Instance.Host = this;

                        // Call the initialization sub of the plugin
                        newPlugin.Instance.Initialize();

                        // Add the new plugin to our collection here
                        this.colAvailablePlugins.Add(newPlugin);

                        // Cleanup a bit.
                        newPlugin = null;
                    }

                    typeInterface = null;
                }
            }

            pluginAssembly = null;
        }

        #endregion

        /// <summary>
        /// Displays a feedback dialog from the plugin
        /// </summary>
        /// <param name="Feedback">String message for feedback</param>
        /// <param name="Plugin">The plugin that called the feedback</param>
        public void Feedback(string Feedback, IMbUnitPlugin Plugin)
        {
        //    //This sub makes a new feedback form and fills it out
        //    //With the appropriate information
        //    //This method can be called from the actual plugin with its Host Property

        //    System.Windows.Forms.Form newForm = null;
        //    frmFeedback newFeedbackForm = new frmFeedback();

        //    //Here we set the frmFeedback's properties that i made custom
        //    newFeedbackForm.PluginAuthor = "By: " + Plugin.Author;
        //    newFeedbackForm.PluginDesc = Plugin.Description;
        //    newFeedbackForm.PluginName = Plugin.Name;
        //    newFeedbackForm.PluginVersion = Plugin.Version;
        //    newFeedbackForm.Feedback = Feedback;

        //    //We also made a Form object to hold the frmFeedback instance
        //    //If we were to declare if not as  frmFeedback object at first,
        //    //We wouldn't have access to the properties we need on it
        //    newForm = newFeedbackForm;
        //    newForm.ShowDialog();

        //    newFeedbackForm = null;
        //    newForm = null;
        }
    }
}

