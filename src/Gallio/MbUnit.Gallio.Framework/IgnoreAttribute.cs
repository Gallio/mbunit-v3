using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Attributes;

namespace MbUnit.Framework
{
    public class IgnoreAttribute : DecoratorPatternAttribute
    {
        private readonly string reason;

        // TODO.

        public IgnoreAttribute()
            : this("")
        {
        }

        public IgnoreAttribute(string reason)
        {
            this.reason = reason;
        }

        /// <summary>
        /// Gets the reason that the test has been ignored.
        /// </summary>
        public string Reason
        {
            get { return reason; }
        }
    }
}
