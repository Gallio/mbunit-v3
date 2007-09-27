using System;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedArgumentExceptionAttribute : ExpectedExceptionAttribute
    {
        // TODO.

        public ExpectedArgumentExceptionAttribute()
            : base(typeof(ArgumentException))
        {
        }

        public ExpectedArgumentExceptionAttribute(string message)
            : base(typeof(ArgumentException), message)
        {
        }
    }
}