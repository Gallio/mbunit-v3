namespace MbUnit.Framework
{
    using System;
    using TestDriven.UnitTesting.Exceptions;

    public class AssertionException : AssertFailExceptionBase
    {
        public AssertionException(string message)
            : base(message)
        {
        }
    }
}
