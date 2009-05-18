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
    public class ClearCurrentUserPluginCacheCommand : BaseUtilityCommand<object>
    {
        /// <inheritdoc />
        public override int Execute(UtilityCommandContext context, object arguments)
        {
            context.Logger.Log(LogSeverity.Important, "Clearing the current user's plugin cache.");

            CachingPluginLoader.ClearCurrentUserPluginCache();
            return 0;
        }
    }
}
