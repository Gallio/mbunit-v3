// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model.Diagnostics;
using Gallio.Reflection;
using Gallio.Model;
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Verifies that a contract has been satisfied.  The contract is described by a value
    /// assigned to a field of the test fixture.
    /// </summary>
    /// <example>
    /// <code>
    /// [TestFixture]
    /// public class MyExceptionTests
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract Contract = new ExceptionContract&lt;MyException&gt;();
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ComparisonContract{TTarget}"/>
    /// <seealso cref="EqualityContract{TTarget}"/>
    /// <seealso cref="ExceptionContract{TException}"/>
    /// <seealso cref="ImmutabilityContract{TTarget}"/>
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
        public override bool IsTest(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            IFieldInfo field = codeElement as IFieldInfo;
            Validate(containingScope, field);

            IPatternScope fieldScope = containingScope.CreateChildTestScope(field.Name, field);
            fieldScope.TestBuilder.Kind = TestKinds.Suite;
            fieldScope.TestBuilder.IsTestCase = false;

            InitializeTest(fieldScope, field);
            SetTestSemantics(fieldScope.TestBuilder, field);

            fieldScope.TestBuilder.ApplyDeferredActions();
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="field">The field</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(IPatternScope containingScope, IFieldInfo field)
        {
            if (!containingScope.CanAddChildTest || field == null || ! field.IsInitOnly)
                ThrowUsageErrorException("This attribute can only be used on a read-only field within a test type.");
        }

        /// <summary>
        /// Initializes a test for a contract verifier field after it has been added to the test model.
        /// </summary>
        /// <param name="fieldScope">The field scope</param>
        /// <param name="field">The field</param>
        protected virtual void InitializeTest(IPatternScope fieldScope, IFieldInfo field)
        {
            string xmlDocumentation = field.GetXmlDocumentation();
            if (xmlDocumentation != null)
                fieldScope.TestBuilder.AddMetadata(MetadataKeys.XmlDocumentation, xmlDocumentation);

            fieldScope.Process(field);
        }

        /// <summary>
        /// Establishes the semantics of the contract verifier.
        /// </summary>
        /// <param name="testBuilder">The test builder</param>
        /// <param name="field">The field</param>
        protected virtual void SetTestSemantics(ITestBuilder testBuilder, IFieldInfo field)
        {
            testBuilder.TestInstanceActions.ExecuteTestInstanceChain.After(state =>
            {
                FieldInfo fieldInfo = field.Resolve(true);
                IContract contract = fieldInfo.GetValue(fieldInfo.IsStatic ? null : state.FixtureInstance) as IContract;
                if (contract == null)
                    throw new TestFailedException(String.Format("The field '{0}' must contain an instance of type IContract that describes a contract to be verified.", field.Name));

                TestOutcome outcome = Test.RunDynamicTests(contract.GetContractVerificationTests(), field, null, null);
                if (outcome != TestOutcome.Passed)
                    throw new SilentTestException(outcome);
            });
        }
    }
}
