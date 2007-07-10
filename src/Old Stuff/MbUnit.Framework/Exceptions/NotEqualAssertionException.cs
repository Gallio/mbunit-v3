using System;
using System.Collections.Generic;
using System.Text;
using TestDriven.UnitTesting.Exceptions;

namespace MbUnit.Framework
{
    public class NotEqualAssertionException : AssertExceptionBase
    {

        private string expectedMessage = null;

        public NotEqualAssertionException(
                 Object expected,
                 Object actual
                 )
        {
            this.expectedMessage = String.Format("Equal assertion failed: [[{0}]]!=[[{1}]]", expected, actual);
        }

        public NotEqualAssertionException(
            Object expected,
            Object actual,
            String message
            )
            : base(message)
        {
            this.expectedMessage = String.Format("Equal assertion failed: [[{0}]]!=[[{1}]]", expected, actual);
        }

        public override string Message
        {
            get
            {
                return String.Format("{0} {1}", base.Message, this.expectedMessage);
            }
        }

    }
}
