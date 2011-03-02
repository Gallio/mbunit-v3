// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;
using Gallio.Model;
using System.Reflection;
using Gallio.Model.Tree;
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Verifies that a contract has been satisfied.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is applied to a field of type <see cref="IContract" /> to that describes
    /// a contract to be verified.  Each contract object is like a reusable test suite that
    /// can be customized by providing constructor parameters or by setting properties.
    /// MbUnit includes several out-of-the-box contract verifier implementations that
    /// capture common testing patterns for verifying the implementation of equivalence relations, exceptions,
    /// collections, accessors and other code.
    /// </para>
    /// <para>
    /// Contract verifiers can be incorporated into test fixtures in two ways.
    /// <list type="bullet">
    /// <item>Contract stored in a readonly instance field.  The contract verifier tests 
    /// will be generated dynamically at test execution time similarly to a 
    /// <see cref="DynamicTestFactoryAttribute" />.  This mechanism is compatible with data-driven
    /// test fixtures (such as generic test fixtures or fixtures with constructor parameters)
    /// but the user will be unable to individually view and select each test in the test runner
    /// before running them.</item>
    /// <item>Contract stored in a readonly <strong>static</strong> instance field.  The
    /// contract verifier tests will be generated statically at test exploration time similarly to a 
    /// <see cref="StaticTestFactoryAttribute" />.  This mechanism can only be used in non-generic
    /// test fixtures but the user will be able to individually view and select each
    /// test in the test runner before running them.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyExceptionTests
    /// {
    ///     // "dynamic" contract verifier (similar to a DynamicTestFactory)
    ///     [VerifyContract]
    ///     public readonly IContract Contract1 = new ExceptionContract<MyException>();
    ///     
    ///     // "static" contract verifier (similar to a StaticTestFactory)
    ///     [VerifyContract]
    ///     public readonly static IContract Contract2 = new ListContract<MyList<int>, int>();
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="AccessorContract{TTarget, TValue}"/>
    /// <seealso cref="CollectionContract{TList, TItem}"/>
    /// <seealso cref="ComparisonContract{TTarget}"/>
    /// <seealso cref="EqualityContract{TTarget}"/>
    /// <seealso cref="ExceptionContract{TException}"/>
    /// <seealso cref="ImmutabilityContract{TTarget}"/>
    /// <seealso cref="ListContract{TList, TItem}"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class VerifyContractAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public override IList<TestPart> GetTestParts(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return new[] { new TestPart() { IsTestContainer = true } };
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            //TODO: Review: Issue 762: Shouldn't the base method be invoked here?
            //base.Consume(containingScope, codeElement, skipChildren);
            IFieldInfo field = codeElement as IFieldInfo;
            Validate(containingScope, field);

            IPatternScope fieldScope = containingScope.CreateChildTestScope(field.Name, field);
            fieldScope.TestBuilder.Kind = TestKinds.Suite;
            fieldScope.TestBuilder.IsTestCase = false;

            InitializeTest(fieldScope, field);
            GenerateTestsFromContract(fieldScope, field, containingScope);

            fieldScope.TestBuilder.ApplyDeferredActions();
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="field">The field.</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly.</exception>
        protected virtual void Validate(IPatternScope containingScope, IFieldInfo field)
        {
            if (!containingScope.CanAddChildTest || field == null || ! field.IsInitOnly)
                ThrowUsageErrorException("This attribute can only be used on a read-only field within a test type.");

            if (field.IsStatic && field.DeclaringType.ContainsGenericParameters)
                ThrowUsageErrorException("A contract verifier field cannot be static when it is declared on a generic type.  "
                    + "Make the field non-static or make its declaring type non-generic.");
        }

        /// <summary>
        /// Initializes a test for a contract verifier field after it has been added to the test model.
        /// </summary>
        /// <param name="fieldScope">The field scope.</param>
        /// <param name="field">The field.</param>
        protected virtual void InitializeTest(IPatternScope fieldScope, IFieldInfo field)
        {
            string xmlDocumentation = field.GetXmlDocumentation();
            if (xmlDocumentation != null)
                fieldScope.TestBuilder.AddMetadata(MetadataKeys.XmlDocumentation, xmlDocumentation);

            fieldScope.Process(field);
        }

        /// <summary>
        /// Generates static or dynamic tests from the contract.
        /// </summary>
        /// <param name="fieldScope">The field scope.</param>
        /// <param name="field">The field.</param>
        /// <param name="containingScope">The containing scope.</param>
        protected virtual void GenerateTestsFromContract(IPatternScope fieldScope, IFieldInfo field, IPatternScope containingScope)
        {
            if (field.IsStatic)
            {
                FieldInfo resolvedField = field.Resolve(false);
                if (resolvedField == null)
                {
                    fieldScope.TestModelBuilder.AddAnnotation(new Annotation(AnnotationType.Info, field,
                        "This test runner does not fully support static contract verifier methods "
                        + "because the code that defines the contract cannot be executed "
                        + "at test exploration time.  Consider making the contract field non-static instead."));
                    return;
                }

                var contract = resolvedField.GetValue(null) as IContract;
                if (contract == null)
                {
                    fieldScope.TestModelBuilder.AddAnnotation(new Annotation(AnnotationType.Error, field,
                        "Expected the contract field to contain a value that is assignable "
                        + "to type IContract."));
                    return;
                }

                IEnumerable<Test> contractTests = GetContractVerificationTests(contract, field);
                Test.BuildStaticTests(contractTests, fieldScope, field);
            }
            else
            {
                fieldScope.TestBuilder.TestInstanceActions.ExecuteTestInstanceChain.After(state =>
                {
                    var invoker = new FixtureMemberInvoker<IContract>(null, containingScope, field.Name);
                    IContract contract;

                    try
                    {
                        contract = invoker.Invoke(FixtureMemberInvokerTargets.Field);
                    }
                    catch (PatternUsageErrorException exception)
                    {
                        throw new TestFailedException(
                            String.Format(
                                "The field '{0}' must contain an instance of type IContract that describes a contract to be verified.",
                                field.Name), exception);
                    }

                    IEnumerable<Test> contractTests = GetContractVerificationTests(contract, field);

                    TestOutcome outcome = Test.RunDynamicTests(contractTests, field, null, null);
                    if (outcome != TestOutcome.Passed)
                        throw new SilentTestException(outcome);
                });
            }
        }

        private static IEnumerable<Test> GetContractVerificationTests(IContract contract, IFieldInfo field)
        {
            var context = new ContractVerificationContext(field);
            return contract.GetContractVerificationTests(context);
        }
    }
}
