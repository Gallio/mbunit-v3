using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.TDNetRunner.Facade
{
    /// <summary>
    /// Describes the mode of the TDNet category filter.
    /// </summary>
    public enum FacadeFilterCategoryMode
    {
        /// <summary>
        /// No category filter.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Include named categories.
        /// </summary>
        Include,

        /// <summary>
        /// Exclude named categories.
        /// </summary>
        Exclude,
    }
}
