using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Performs one primitive action of a test and returns its outcome.
    /// </summary>
    /// <returns>The outcome of the action</returns>
    public delegate TestOutcome TestAction();
}
