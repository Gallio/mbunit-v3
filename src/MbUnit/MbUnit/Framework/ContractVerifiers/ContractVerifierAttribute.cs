// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using Gallio.Model;
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// The <see cref="ContractVerifierAttribute" /> class qualifies the field-based contract verifiers.
    /// It designates a test fixture field as a contract verifier. 
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ContractVerifierAttribute : PatternAttribute
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
        public override bool IsTest(PatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            var fixtureType = GetFixtureType(containingScope);
            var fixtureInstance = GetFixtureInstance(fixtureType);
            var fieldInfo = GetFieldInfo(codeElement);
            var fieldInstance = GetFieldInstance(fieldInfo, fixtureInstance);
            var contractTest = new PatternTest(codeElement.Name, codeElement, containingScope.TestDataContext.CreateChild());
            contractTest.IsTestCase = false;
            contractTest.Metadata.SetValue(MetadataKeys.TestKind, TestKinds.Group);
            var scope = containingScope.AddChildTest(contractTest);

            foreach (var item in fieldInstance.GetContractPatterns())
            {
                item.Build(scope, codeElement.Name);
            }
        }

        private Type GetFixtureType(PatternEvaluationScope containingScope)
        {
            return ((ITypeInfo)containingScope.CodeElement).Resolve(true);
        }

        private object GetFixtureInstance(Type fixtureType)
        {
            try
            {
                return Activator.CreateInstance(fixtureType);
            }
            catch (MissingMethodException)
            {
                ThrowUsageErrorException("Contract verifier fields require that the parent test fixture have a default parameter-less constructor.");
                return null;
            }
        }

        private FieldInfo GetFieldInfo(ICodeElementInfo codeElement)
        {
            var fieldInfo = ((IFieldInfo)codeElement).Resolve(true);

            if (!fieldInfo.IsInitOnly)
            {
                ThrowUsageErrorException("The contract verifier field must be marked with the keyword readonly (C#) / ReadOnly (VB.NET).");
            }

            return fieldInfo;
        }

        private IContractVerifier GetFieldInstance(FieldInfo fieldInfo, object fixtureInstance)
        {
            object instance = fieldInfo.GetValue(fixtureInstance);

            if (Object.ReferenceEquals(instance, null))
            {
                ThrowUsageErrorException("The contract verifier field cannot be null.");
            }

            if (!typeof(IContractVerifier).IsAssignableFrom(instance.GetType()))
            {
                ThrowUsageErrorException("The contract verifier field must be of a type which implements IContractVerifier.");
            }

            return (IContractVerifier)instance;
        }
    }
}
