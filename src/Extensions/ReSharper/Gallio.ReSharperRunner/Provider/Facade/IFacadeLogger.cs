using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// A facade of the ReSharper logger utilities.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </remarks>
    public interface IFacadeLogger
    {
        void LogVerbose(string message);
        void LogMessage(string message);
        void LogError(string message);
    }
}
