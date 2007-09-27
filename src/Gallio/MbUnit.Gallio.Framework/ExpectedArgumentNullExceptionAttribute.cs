using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedArgumentNullExceptionAttribute : ExpectedExceptionAttribute
    {
        // TODO.

        public ExpectedArgumentNullExceptionAttribute()
            : base(typeof(ArgumentNullException))
        {
        }

        public ExpectedArgumentNullExceptionAttribute(string message)
            : base(typeof(ArgumentException), message)
        {
        }

    }
}
