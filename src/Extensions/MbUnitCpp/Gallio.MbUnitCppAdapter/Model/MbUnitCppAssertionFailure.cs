using System;
using System.Collections.Generic;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.Bridge;

namespace Gallio.MbUnitCppAdapter.Model
{
    internal class MbUnitCppAssertionFailure
    {
        private string description;
        private string message;
        private string actualValue;
        private string expectedValue;

        public string Description
        {
            get { return description; }
        }

        public string Message
        {
            get { return message; }
        }

        public string ActualValue
        {
            get { return actualValue; }
        }

        public string ExpectedValue
        {
            get { return expectedValue; }
        }

        public MbUnitCppAssertionFailure(NativeAssertionFailure native, UnmanagedTestRepository repository)
        {
            description = repository.GetString(native.DescriptionId);
            message = repository.GetString(native.MessageId);
            actualValue = repository.GetString(native.ActualValueId);
            expectedValue = repository.GetString(native.ExpectedValueId);
        }
    }
}
