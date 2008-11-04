using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Framework.Data;
using Gallio.Framework.Assertions;
using Gallio.Reflection;

namespace MbUnit.Framework.NewContractVerifiers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TAttribute"></typeparam>
    public class HasAttributeVerifier<TTarget, TAttribute> : AbstractContractVerifier
        where TTarget : class
        where TAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public override void DeclareChildTests(PatternEvaluationScope scope)
        {
            var test = new PatternTest("Has" + typeof(TAttribute).Name, null, scope.TestDataContext.CreateChild());
            test.Metadata.SetValue(MetadataKeys.TestKind, TestKinds.Test);
            test.IsTestCase = true;
            scope.AddChildTest(test);

            test.TestInstanceActions.BeforeTestInstanceChain.After(
                state =>
                {
                    ObjectCreationSpec spec = state.GetFixtureObjectCreationSpec(scope.Parent.CodeElement as ITypeInfo);
                    state.FixtureType = spec.ResolvedType;
                    state.FixtureInstance = spec.CreateInstance();
                });

            test.TestInstanceActions.ExecuteTestInstanceChain.After(
                state =>
                {
                    AssertionHelper.Verify(() =>
                    {
                        if (typeof(TTarget).IsDefined(typeof(TAttribute), false))
                            return null;

                        return new AssertionFailureBuilder("Expected the exception type to be annotated by a particular attribute.")
                            .AddRawLabeledValue("Exception Type", typeof(TTarget))
                            .AddRawLabeledValue("Expected Attribute", typeof(TAttribute))
                            .ToAssertionFailure();
                    });
                });
        }
    }
}
