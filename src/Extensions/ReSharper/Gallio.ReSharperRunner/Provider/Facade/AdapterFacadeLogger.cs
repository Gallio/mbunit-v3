using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Util;
using Gallio.Loader;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// A facade and remote proxy for the ReSharper logger utilities.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </remarks>
    public class AdapterFacadeLogger : MarshalByRefObject, IFacadeLogger
    {
        /// <inheritdoc />
        public void LogVerbose(string message)
        {
            try
            {
                Logger.LogMessage(LoggingLevel.VERBOSE, message);
            }
            catch (Exception ex)
            {
                throw SafeException.Wrap(ex);
            }
        }

        /// <inheritdoc />
        public void LogMessage(string message)
        {
            try
            {
                Logger.LogMessage(LoggingLevel.NORMAL, message);
            }
            catch (Exception ex)
            {
                throw SafeException.Wrap(ex);
            }
        }

        /// <inheritdoc />
        public void LogError(string message)
        {
            try
            {
                Logger.LogError(message);
            }
            catch (Exception ex)
            {
                throw SafeException.Wrap(ex);
            }
        }

        /// <inheritdoc />
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
