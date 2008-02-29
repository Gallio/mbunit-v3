using System;
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The fixture initializer attribute is applied to a method that is to be
    /// invoked after a fixture instance has been created to complete its
    /// initialization.
    /// </para>
    /// <para>
    /// This attribute provides a mechanism for completing the initialization
    /// of a fixture if the work cannot be completed entirely within the
    /// constructor.  For example, data binding might be used to set fields
    /// and property values of the fixture instance.  Consequently post-construction
    /// initialization may be required.
    /// </para>
    /// <para>
    /// <see cref="FixtureInitializerAttribute" /> allows initialization to occur
    /// earlier in the test lifecycle than <see cref="FixtureSetUpAttribute" />.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The attribute may be applied to multiple methods within a fixture, however
    /// the order in which they are processed is undefined.
    /// </para>
    /// <para>
    /// The method to which this attribute is applied must be declared by the
    /// fixture class and must not have any parameters.  The method may be static.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FixtureInitializerAttribute : ContributionPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateContainingTest(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            IMethodInfo method = (IMethodInfo) codeElement;

            containingTestBuilder.Test.TestInstanceActions.InitializeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    testInstanceState.InvokeFixtureMethod(method, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
                });
        }
    }
}