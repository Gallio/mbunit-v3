using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common;

namespace Gallio.Runner.Extensions
{
    internal class ActivationConditionContext : ConditionContext
    {
        protected override bool HasPropertyImpl(string @namespace, string identifier)
        {
            if (string.Compare("env", @namespace, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return ! string.IsNullOrEmpty(Environment.GetEnvironmentVariable(identifier));
            }

            return false;
        }
    }
}
