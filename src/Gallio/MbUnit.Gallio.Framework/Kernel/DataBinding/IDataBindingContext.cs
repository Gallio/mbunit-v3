using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// A data binding context is used to track resources produced during
    /// the data binding process.  A new context is created each time data
    /// binding occurs.  It must be disposed when the data binding activity
    /// is finished.
    /// </summary>
    /// <remarks>
    /// Essentially the data binding context consists of a very limited
    /// form of a unit of work.
    /// </remarks>
    public interface IDataBindingContext : IDisposable
    {
        /// <summary>
        /// This event is raised when the data binding context is being
        /// disposed.  Data binding participants that must perform reclamation
        /// of bound objects should register to receive this event and
        /// execute their decommission concerns.
        /// </summary>
        event EventHandler Disposing;

        /// <summary>
        /// Gets the data binder used by the context.
        /// </summary>
        IDataBinder Binder { get; }
    }
}
