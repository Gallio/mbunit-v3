using System;
using System.Collections.Generic;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Common;
using Gallio.Common.Collections;

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
        private readonly object unexpectedValue;
        private readonly bool hasUnexpectedValue;
        private readonly bool diffing;
        private readonly int line;
        private readonly Pair<string, object>[] extraLabeledValues;

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
        /// Gets the raw unexpected value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Null if no unexpected value is applicable to that assertion failure.
        /// </para>
        /// </remarks>
        public object UnexpectedValue
        {
            get { return unexpectedValue; }
        }

        /// <summary>
        /// Indicates whether the assertion failure provides an unexpected value.
        /// </summary>
        public bool HasUnexpectedValue
        {
            get { return hasUnexpectedValue; }
        }

        /// <summary>
        /// Should diffing be applied when displaying the expected/unexpected and the actual value?
        /// </summary>
        public bool Diffing
        {
            get { return diffing; }
        }

        /// <summary>
        /// Gets the line where the assertion has failed.
        /// </summary>
        public int Line
        {
            get { return line; }
        }

        /// <summary>
        /// Gets an array of extra labeled values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Might be empty.
        /// </para>
        /// </remarks>
        public Pair<string, object>[] ExtraLabeledValues
        {
            get { return extraLabeledValues; }
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
            hasActualValue = native.ActualValue.IsValid;
            hasExpectedValue = native.ExpectedValue.IsValid;
            hasUnexpectedValue = native.UnexpectedValue.IsValid;
            line = native.Line;

            if (hasActualValue)
                actualValue = NativeValueParser.Parse(stringResolver.GetString(native.ActualValue.ValueId), native.ActualValue.ValueType);

            if (hasExpectedValue)
                expectedValue = NativeValueParser.Parse(stringResolver.GetString(native.ExpectedValue.ValueId), native.ExpectedValue.ValueType);

            if (hasUnexpectedValue)
                unexpectedValue = NativeValueParser.Parse(stringResolver.GetString(native.UnexpectedValue.ValueId), native.UnexpectedValue.ValueType);

            diffing = hasActualValue && (native.ActualValue.ValueType == NativeValueType.String) &&
                ((hasExpectedValue && (native.ExpectedValue.ValueType == NativeValueType.String)) ||
                (hasUnexpectedValue && (native.UnexpectedValue.ValueType == NativeValueType.String)));

            extraLabeledValues = GenericCollectionUtils.ToArray(GetExtraLabeledValues(native, stringResolver));
        }

        private static IEnumerable<Pair<string, object>> GetExtraLabeledValues(NativeAssertionFailure native, IStringResolver stringResolver)
        {
            foreach (NativeLabeledValue item in new[] { native.Extra_0, native.Extra_1 })
            {
                if (item.IsValid)
                {
                    yield return new Pair<string, object>(
                        stringResolver.GetString(item.LabelId),
                         NativeValueParser.Parse(stringResolver.GetString(item.ValueId), item.ValueType));
                }
            }
        }
    }
}
