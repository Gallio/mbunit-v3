// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Collections;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Declares that an assembly generates an assembly-level test.
    /// Subclasses of this attribute can customize how test enumeration takes place within
    /// the assembly.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given assembly.
    /// </para>
    /// </summary>
    /// <seealso cref="TestAssemblyDecoratorPatternAttribute"/>
    [AttributeUsage(PatternAttributeTargets.TestAssembly, AllowMultiple = false, Inherited = true)]
    public abstract class TestAssemblyPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the assembly pattern attribute to use
        /// when no other pattern consumes an assembly.
        /// </summary>
        public static readonly TestAssemblyPatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(PatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            Validate(containingScope, assembly);

            PatternTest assemblyTest = CreateAssemblyTest(containingScope, assembly);
            PatternEvaluationScope assemblyScope = containingScope.AddChildTest(assemblyTest);

            InitializeAssemblyTest(assemblyScope, assembly);

            if (skipChildren)
                PrepareToPopulateChildrenOnDemand(assemblyScope, assembly);
            else
                PopulateChildrenImmediately(assemblyScope, assembly);

            assemblyScope.ApplyDecorators();
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="assembly">The assembly</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(PatternEvaluationScope containingScope, IAssemblyInfo assembly)
        {
            if (!containingScope.CanAddChildTest || assembly == null)
                ThrowUsageErrorException("This attribute can only be used on a test assembly.");
        }

        /// <summary>
        /// Creates a test for an assembly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="assembly">The assembly</param>
        /// <returns>The test</returns>
        protected virtual PatternTest CreateAssemblyTest(PatternEvaluationScope containingScope, IAssemblyInfo assembly)
        {
            PatternTest test = new PatternTest(assembly.Name, assembly, containingScope.TestDataContext.CreateChild());
            test.Kind = TestKinds.Assembly;
            return test;
        }

        /// <summary>
        /// Initializes a test for an assembly after it has been added to the test model.
        /// </summary>
        /// <param name="assemblyScope">The assembly scope</param>
        /// <param name="assembly">The assembly</param>
        protected virtual void InitializeAssemblyTest(PatternEvaluationScope assemblyScope, IAssemblyInfo assembly)
        {
            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyScope.Test.Metadata);

            assemblyScope.Process(assembly);
        }

        /// <summary>
        /// Populates the children of the assembly test all at once.
        /// </summary>
        /// <remarks>
        /// The default implementation processes all public and non-public types within the assembly.
        /// </remarks>
        /// <param name="assemblyScope">The assembly scope</param>
        /// <param name="assembly">The assembly</param>
        protected virtual void PopulateChildrenImmediately(PatternEvaluationScope assemblyScope, IAssemblyInfo assembly)
        {
            foreach (ITypeInfo type in assembly.GetTypes())
            {
                if (!type.IsNested)
                    assemblyScope.Consume(type, false, DefaultTypePattern);
            }
        }

        /// <summary>
        /// Prepares to populate the children of the assembly test on demand by
        /// adding actions to <see cref="PatternEvaluationScope.PopulateChildrenChain" />.
        /// </summary>
        /// <param name="assemblyScope">The assembly scope</param>
        /// <param name="assembly">The assembly</param>
        protected virtual void PrepareToPopulateChildrenOnDemand(PatternEvaluationScope assemblyScope, IAssemblyInfo assembly)
        {
            HashSet<ITypeInfo> populatedTypes = new HashSet<ITypeInfo>();
            assemblyScope.PopulateChildrenChain.After(delegate(ICodeElementInfo childCodeElement)
            {
                ITypeInfo type = childCodeElement as ITypeInfo;
                if (type != null && ! type.IsNested && !populatedTypes.Contains(type) && assembly.Equals(type.Assembly))
                {
                    populatedTypes.Add(type);
                    assemblyScope.Consume(type, false, DefaultTypePattern);
                }
            });
        }

        /// <summary>
        /// Gets the default pattern to apply to types that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="RecursiveTypePattern.Instance"/>.
        /// </remarks>
        protected virtual IPattern DefaultTypePattern
        {
            get { return RecursiveTypePattern.Instance; }
        }

        private sealed class DefaultImpl : TestAssemblyPatternAttribute
        {
        }
    }
}