using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// A data factory yields a single data value on demand.  A data
    /// factory can register its decommission concerns on the data binding
    /// context when the value is obtained.
    /// </summary>
    public interface IDataFactory
    {
        /// <summary>
        /// Gets the data value.
        /// </summary>
        /// <param name="context">The data binding context, never null</param>
        /// <returns>The data value, may be null</returns>
        object GetValue(IDataBindingContext context);
    }
}
