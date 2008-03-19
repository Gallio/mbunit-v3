using System;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using NBehave.Core;

namespace NBehave.Spec.Framework
{
    /// <summary>
    /// <para>
    /// When applied to a class, the <see cref="ContextAttribute" /> declares a common context
    /// for interpreting specifications.  The name of the context class describes preconditions or
    /// assumptions about the environment or the use case to which the specifications apply.
    /// </para>
    /// <para>
    /// For example, a good context class name might be: "When_user_has_an_account_with_a_non_zero_balance".
    /// Alternately the name might be expressed in Pascal-case: "WhenUserHasAnAccountWithANonZeroBalance" if preferred.
    /// In either case, the framework will transform the name to "When user has an account with a non zero balance"
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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class ContextAttribute : TestTypePatternAttribute
    {
        /// <inheritdoc />
        protected override PatternTest CreateTest(IPatternTestBuilder containingTestBuilder, ITypeInfo type)
        {
            PatternTest test = base.CreateTest(containingTestBuilder, type);
            test.Name = NameSanitizer.MakeNameFromIdentifier(test.Name);
            test.Kind = NBehaveTestKinds.Context;
            return test;
        }
    }
}
