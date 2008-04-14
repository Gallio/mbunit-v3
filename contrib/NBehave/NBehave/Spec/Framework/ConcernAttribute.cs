using System;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using NBehave.Core;

namespace NBehave.Spec.Framework
{
    /// <summary>
    /// <para>
    /// When applied to a class, the <see cref="ConcernAttribute" /> expresses a common concern that
    /// may consist of one or more specification contexts expressed as nested classes.
    /// The name of the concern class describes a high-level concern or story that is used
    /// to classify related specification contexts.
    /// </para>
    /// <para>
    /// For example, a good concern class name might be: "Account_balance_management".
    /// Alternately the name might be expressed in Pascal-case: "AccountBalanceManagement" if preferred.
    /// In either case, the framework will transform the name to "Account balance management"
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
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class ConcernAttribute : TestTypePatternAttribute
    {
        /// <inheritdoc />
        protected override PatternTest CreateTest(PatternEvaluationScope constainingScope, ITypeInfo type)
        {
            PatternTest test = base.CreateTest(constainingScope, type);
            test.Name = NameSanitizer.MakeNameFromIdentifier(test.Name);
            test.Kind = NBehaveTestKinds.Concern;
            return test;
        }

        /// <inheritdoc />
        protected override void InitializeTest(PatternEvaluationScope typeScope, ITypeInfo type)
        {
            base.InitializeTest(typeScope, type);

            typeScope.Test.Metadata.Add(NBehaveMetadataKeys.Concern, typeScope.Test.Name);
        }
    }
}