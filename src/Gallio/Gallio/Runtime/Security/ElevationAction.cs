using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Security
{
    /// <summary>
    /// Performs an action using an elevation context.
    /// </summary>
    /// <param name="context">The elevation context, not null</param>
    /// <returns>True if the action completed successfully</returns>
    public delegate bool ElevationAction(IElevationContext context);
}
