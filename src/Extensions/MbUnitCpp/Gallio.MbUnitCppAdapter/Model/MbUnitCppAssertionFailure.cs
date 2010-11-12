using System;
using System.Collections.Generic;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.Bridge;

namespace Gallio.MbUnitCppAdapter.Model
{
    internal class MbUnitCppAssertionFailure
    {
        private readonly string description;
        private readonly string message;
        private readonly object actualValue;
        private readonly bool hasActualValue;
        private readonly object expectedValue;
        private readonly bool hasExpectedValue;
        private readonly bool diffing;

        public string Description
        {
            get { return description; }
        }

        public string Message
        {
            get { return message; }
        }

        public object ActualValue
        {
            get { return actualValue; }
        }

        public bool HasActualValue
        {
            get { return hasActualValue; }
        }

        public object ExpectedValue
        {
            get { return expectedValue; }
        }

        public bool HasExpectedValue
        {
            get { return hasExpectedValue; }
        }

        public bool Diffing
        {
            get { return diffing; }
        }

        public MbUnitCppAssertionFailure(NativeAssertionFailure native, UnmanagedTestRepository repository)
        {
            description = repository.GetString(native.DescriptionId);
            message = repository.GetString(native.MessageId);
            hasActualValue = native.ActualValueId != 0;
            hasExpectedValue = native.ExpectedValueId != 0;

            if (hasActualValue)
                actualValue = TransformValue(repository.GetString(native.ActualValueId), native.ActualValueType);

            if (hasExpectedValue)
                expectedValue = TransformValue(repository.GetString(native.ExpectedValueId), native.ExpectedValueType);

            diffing = hasActualValue && hasExpectedValue && 
                (native.ActualValueType == NativeValueType.String) && 
                (native.ExpectedValueType == NativeValueType.String);
        }

        private static object TransformValue(string field, NativeValueType valueType)
        {
            switch (valueType)
            {
                case NativeValueType.Raw:
                case NativeValueType.String:
                    return field;

                case NativeValueType.Boolean:
                    return Boolean.Parse(field);

                case NativeValueType.Char:
                    return Char.Parse(field);

                case NativeValueType.Byte:
                    return Byte.Parse(field);

                case NativeValueType.Int16:
                    return Int16.Parse(field);

                case NativeValueType.UInt16:
                    return UInt16.Parse(field);

                case NativeValueType.Int32:
                    return Int32.Parse(field);

                case NativeValueType.UInt32:
                    return UInt32.Parse(field);

                case NativeValueType.Int64:
                    return Int64.Parse(field);

                case NativeValueType.Single:
                    return Single.Parse(field);

                case NativeValueType.Double:
                    return Double.Parse(field);

                default:
                    throw new ArgumentOutOfRangeException("valueType");
            }
        }
    }
}
