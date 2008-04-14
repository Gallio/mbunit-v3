using System;
using System.Diagnostics;
using Gallio.Framework.Data.Formatters;
using Gallio.Runtime;
using Gallio.Utilities;

namespace NBehave.Spec.Framework.Constraints
{
    /// <summary>
    /// Describes the result of the evaluation of a specification.
    /// </summary>
    public class SpecificationResult
    {
        private readonly bool success;
        private readonly string message;
        private StackTrace stackTrace;

        private SpecificationResult(bool success, string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            this.success = success;
            this.message = message;
        }

        public static SpecificationResult CreateSuccess()
        {
            return new SpecificationResult(true, "The specification was evaluated successfully.");
        }

        public static SpecificationResult CreateFailure(string message)
        {
            return new SpecificationResult(false, message);
        }

        public static SpecificationResult CreateExceptionFailure(Exception ex)
        {
            return new SpecificationResult(false, ExceptionUtils.SafeToString(ex));
        }

        public static SpecificationResult CreateCriteriaFailure(object actualValue)
        {
            IFormatter formatter = RuntimeAccessor.Instance.Resolve<IFormatter>();
            return new SpecificationResult(false, String.Format(
                "Actual: {0}", formatter.Format(actualValue)));
        }

        public static SpecificationResult CreateComparisonFailure(object expectedValue, object actualValue)
        {
            IFormatter formatter = RuntimeAccessor.Instance.Resolve<IFormatter>();
            return new SpecificationResult(false, String.Format(
                "Expected: {0}\n\tActual: {1}", formatter.Format(expectedValue), formatter.Format(actualValue)));
        }

        /// <summary>
        /// Returns true if the specification was evaluated successfully.
        /// </summary>
        public bool Success
        {
            get { return success; }
        }

        /// <summary>
        /// Gets a description of the specification result.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets or sets the associated stack trace.
        /// </summary>
        public StackTrace StackTrace
        {
            get { return stackTrace; }
            set { stackTrace = value; }
        }
    }
}
