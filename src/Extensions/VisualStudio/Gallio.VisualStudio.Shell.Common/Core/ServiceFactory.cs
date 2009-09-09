using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Provides an instance of a service.
    /// </summary>
    /// <returns>The service instance.</returns>
    public delegate object ServiceFactory();
}
