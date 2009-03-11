using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Debugging
{
    /// <summary>
    /// Default implementation of a debugger manager.
    /// </summary>
    public class DefaultDebuggerManager : IDebuggerManager
    {
        private const string VisualStudioDebuggerTypeName = "Gallio.VisualStudio.Interop.VisualStudioDebugger, Gallio.VisualStudio.Interop";

        /// <inheritdoc />
        public IDebugger GetDefaultDebugger()
        {
            // FIXME: Should resolve service using IoC but runtime start-up time is too crappy in v3.0
            //        for some cases where we would like to use the debugger.  (Like in the host process.)
            //        Replace when runtime has been rewritten.  -- Jeff.
            Type debuggerType = Type.GetType(VisualStudioDebuggerTypeName);
            if (debuggerType == null)
                return new NullDebugger();

            return (IDebugger)Activator.CreateInstance(debuggerType);
        }
    }
}
