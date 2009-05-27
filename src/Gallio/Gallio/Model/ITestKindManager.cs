using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Manages the list of test kinds.
    /// </summary>
    public interface ITestKindManager
    {
        /// <summary>
        /// Gets handles for all registered test kinds.
        /// </summary>
        IList<ComponentHandle<ITestKind, TestKindTraits>> TestKindHandles { get; }
    }
}
