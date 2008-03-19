using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using Gallio;
using Gallio.Collections;
using Gallio.Framework;
using Gallio.Utilities;
using NBehave.Spec.Framework.Constraints;

namespace NBehave.Spec.Framework
{
    /// <summary>
    /// <para>
    /// Specifies constraints on the actual behavior of a system component
    /// and validates that those constraints have been satisfied.
    /// </para>
    /// </summary>
    public static class Specify
    {
        private const string ConstraintsKey = "SpecificationConstraints";

        /// <summary>
        /// Specifies that a value satisfies a given constraint.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The constraint that is being constructed</returns>
        [NonInlined(SecurityAction.Demand)]
        public static SpecificationConstraintBuilder That(object value)
        {
            return RegisterNewConstraintBuilder(value);
        }

        /// <summary>
        /// Specifies that an action throws an exception that satisfies a given constraint.
        /// The exception that was thrown is used as the actual value in the specification
        /// constraint.  If no exception was thrown then the actual value will be null.
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <returns>The constraint that is being constructed</returns>
        [NonInlined(SecurityAction.Demand)]
        public static SpecificationConstraintBuilder ThrownBy(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            try
            {
                action.DynamicInvoke();
                return RegisterNewConstraintBuilder(null);
            }
            catch (TargetInvocationException outerException)
            {
                return RegisterNewConstraintBuilder(outerException.InnerException);
            }
        }

        internal static IEnumerable<ISpecificationConstraint> GetConstraints()
        {
            List<ISpecificationConstraint> constraints = Context.CurrentContext.Data.GetValue<List<ISpecificationConstraint>>(ConstraintsKey);
            if (constraints == null)
                return EmptyArray<ISpecificationConstraint>.Instance;

            return constraints.ToArray();
        }

        internal static void AddConstraint(ISpecificationConstraint constraint)
        {
            UserDataCollection data = Context.CurrentContext.Data;
            lock (data)
            {
                List<ISpecificationConstraint> constraints = data.GetValue<List<ISpecificationConstraint>>(ConstraintsKey);
                if (constraints == null)
                {
                    constraints = new List<ISpecificationConstraint>();
                    data.SetValue(ConstraintsKey, constraints);
                }

                constraints.Add(constraint);
            }
        }

        [NonInlined(SecurityAction.Demand)]
        private static SpecificationConstraintBuilder RegisterNewConstraintBuilder(object actualValue)
        {
            SpecificationConstraintBuilder constraint = new SpecificationConstraintBuilder(actualValue);
            AddConstraint(constraint);
            constraint.SetStackTrace(new StackTrace(2, true));
            return constraint;
        }

        #region Prevent accidental usage of these members
        /// <summary>
        /// Do not use.
        /// </summary>
        public static new void Equals(object a, object b)
        {
            throw new NotSupportedException("Use Specify.That(x).ShouldEqual(y) instead.");
        }

        /// <summary>
        /// Do not use.
        /// </summary>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new NotSupportedException("Use Specify.That(x).ShouldBeTheSameAs(y) instead.");
        }
        #endregion
    }
}