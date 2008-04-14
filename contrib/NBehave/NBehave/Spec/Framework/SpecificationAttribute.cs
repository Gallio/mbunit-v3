using System;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;
using NBehave.Core;
using NBehave.Spec.Framework.Constraints;

namespace NBehave.Spec.Framework
{
    /// <summary>
    /// <para>
    /// When applied to a method, the <see cref="SpecificationAttribute" /> declares an executable
    /// specification of a particular behavior of a system component within a given context.
    /// The name of a specification should clearly express the anticipated behavior.
    /// </para>
    /// <para>
    /// The body of a specification consists of code that exercises the component under test and
    /// submits results to the <see cref="Specify" /> class for evaluation.  The specified constraints
    /// are evaluated one by one after a method completes with a success or failure result reported
    /// for each one.
    /// </para>
    /// <para>
    /// In addition to constraints specified using the <see cref="Specify" /> class, each specification
    /// method also incorporates the implied constraint that it will run to completion without
    /// throwing an exception.  Consequently any exception thrown by a specification method is
    /// interpreted as a failure.
    /// </para>
    /// <para>
    /// For example, a good context specification name might be: "Should_provide_the_option_to_send_money".
    /// Alternately the name might be expressed in Pascal-case: "ShouldProvideTheOptionToSendMoney" if preferred.
    /// In either case, the framework will transform the name to "Should provide the option to send money"
    /// when it appears in reports.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// [Concern]
    /// public class Account_balance_management
    /// {
    ///     [Context]
    ///     public class When_user_has_an_account_with_a_non_zero_balance
    ///     {
    ///         [Specification]
    ///         public void Should_provide_the_option_to_send_money()
    ///         {
    ///             Specify.That(user.CanSendMoney).IsTrue();
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple = false, Inherited = true)]
    public class SpecificationAttribute : TestMethodPatternAttribute
    {
        /// <inheritdoc />
        protected override PatternTest CreateTest(PatternEvaluationScope containingScope, IMethodInfo method)
        {
            PatternTest test = base.CreateTest(containingScope, method);
            test.Name = NameSanitizer.MakeNameFromIdentifier(test.Name);
            test.Kind = NBehaveTestKinds.Specification;
            return test;
        }

        protected override void SetTestSemantics(PatternTest test, IMethodInfo method)
        {
            base.SetTestSemantics(test, method);

            test.TestInstanceActions.ExecuteTestInstanceChain.Around(
                delegate(PatternTestInstanceState state, Action<PatternTestInstanceState> action)
                {
                    Specify.ThrownBy(delegate { action(state); }).ShouldBeNull();

                    bool failed = false;
                    foreach (ISpecificationConstraint constraint in Specify.GetConstraints())
                    {
                        SpecificationResult result = constraint.Evaluate();
                        if (!result.Success)
                        {
                            failed = true;

                            using (Log.BeginSection(constraint.Description))
                            {
                                Log.Failures.WriteLine(result.Message);

                                if (result.StackTrace != null)
                                    Log.Failures.WriteLine(result.StackTrace);
                            }
                        }
                    }

                    if (failed)
                        throw new SilentTestException(TestOutcome.Failed);
                });
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope containingScope, IMethodInfo method)
        {
            base.Validate(containingScope, method);

            if (!containingScope.IsTestDeclaration
                || containingScope.Test.Kind != NBehaveTestKinds.Context)
                throw new PatternUsageErrorException("The [Specification] attribute can only appear on a method within a context class.");
        }
    }
}
