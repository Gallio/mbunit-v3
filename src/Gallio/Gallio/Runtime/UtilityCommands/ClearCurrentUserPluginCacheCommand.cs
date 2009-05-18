using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command to clear the current user's plugin metadata cache.
    /// </summary>
    public class ClearCurrentUserPluginCacheCommand : IUtilityCommand
    {
        /// <inheritdoc />
        public int Execute(UtilityCommandContext context)
        {
            context.Logger.Log(LogSeverity.Important, "Clearing the current user's plugin cache.");

            CachingPluginLoader.ClearCurrentUserPluginCache();
            return 0;
        }

        /// <inheritdoc />
        public Type GetArgumentClass()
        {
            return typeof(Arguments);
        }

        /// <summary>
        /// The arguments for the command.
        /// </summary>
        public class Arguments
        {
        }
    }
}
