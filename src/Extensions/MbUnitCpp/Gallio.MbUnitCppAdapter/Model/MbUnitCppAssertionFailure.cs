using System;
using System.Collections.Generic;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.Bridge;

namespace Gallio.MbUnitCppAdapter.Model
{
    /// <summary>
    /// An assertion failure that occurred while running an MbUnitCpp test case.
    /// </summary>
    public class MbUnitCppAssertionFailure
    {
        private readonly string description;
        private readonly string message;
        private readonly object actualValue;
        private readonly bool hasActualValue;
        private readonly object expectedValue;
        private readonly bool hasExpectedValue;
        private readonly bool diffing;

        /// <summary>
        /// Gets the description of the failure.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets the optional custom user message.
        /// </summary>
        /// <remarks>
        /// <para>
        /// An empty string if no message was specified by the user.
        /// </para>
        /// </remarks>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets the raw actual value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Null if no actual value is applicable to that assertion failure.
        /// </para>
        /// </remarks>
        public object ActualValue
        {
            get { return actualValue; }
        }

        /// <summary>
        /// Indicates whether the assertion failure provides an actual value.
        /// </summary>
        public bool HasActualValue
        {
            get { return hasActualValue; }
        }

        /// <summary>
        /// Gets the raw expected value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Null if no expected value is applicable to that assertion failure.
        /// </para>
        /// </remarks>
        public object ExpectedValue
        {
            get { return expectedValue; }
        }

        /// <summary>
        /// Indicates whether the assertion failure provides an expected value.
        /// </summary>
        public bool HasExpectedValue
        {
            get { return hasExpectedValue; }
        }

        /// <summary>
        /// Should diffing be applied when displaying the expected and the actual value?
        /// </summary>
        public bool Diffing
        {
            get { return diffing; }
        }

        /// <summary>
        /// Constructs an assertion failure.
        /// </summary>
        /// <param name="native">The native unmanged object describing the failure.</param>
        /// <param name="stringResolver">A service to resolve unmanaged unicode strings.</param>
        public MbUnitCppAssertionFailure(NativeAssertionFailure native, IStringResolver stringResolver)
        {
            description = stringResolver.GetString(native.DescriptionId);
            message = stringResolver.GetString(native.MessageId);
            hasActualValue = native.ActualValueId != 0;
            hasExpectedValue = native.ExpectedValueId != 0;

            if (hasActualValue)
                actualValue = NativeValueParser.Parse(stringResolver.GetString(native.ActualValueId), native.ActualValueType);

            if (hasExpectedValue)
                expectedValue = NativeValueParser.Parse(stringResolver.GetString(native.ExpectedValueId), native.ExpectedValueType);

            diffing = hasActualValue && hasExpectedValue && 
                (native.ActualValueType == NativeValueType.String) && 
                (native.ExpectedValueType == NativeValueType.String);
        }
    }
}
