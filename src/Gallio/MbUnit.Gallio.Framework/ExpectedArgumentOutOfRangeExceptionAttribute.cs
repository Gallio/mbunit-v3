using System;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedArgumentOutOfRangeExceptionAttribute : ExpectedExceptionAttribute
    {
        // TODO.

        public ExpectedArgumentOutOfRangeExceptionAttribute()
            : base(typeof(ArgumentOutOfRangeException))
        {
        }

        public ExpectedArgumentOutOfRangeExceptionAttribute(string message)
            : base(typeof(ArgumentException), message)
        {
        }
    }
}