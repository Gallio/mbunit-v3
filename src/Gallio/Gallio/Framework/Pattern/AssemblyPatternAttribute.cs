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
    /// <seealso cref="AssemblyDecoratorPatternAttribute"/>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false, Inherited=true)]
    public abstract class AssemblyPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the assembly pattern attribute to use
        /// when no other pattern consumes an assembly.
        /// </summary>
        public static readonly AssemblyPatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(IPatternResolver patternResolver, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement, bool skipChildren)
        {
            IAssemblyInfo assembly = (IAssemblyInfo)codeElement;
            Validate(assembly);

            PatternTest test = CreateAssemblyTest(containingTestBuilder, assembly);
            IPatternTestBuilder testBuilder = containingTestBuilder.AddChild(test);
            InitializeAssemblyTest(testBuilder, assembly);

            if (skipChildren)
                PrepareToPopulateChildrenOnDemand(testBuilder, assembly);
            else
                PopulateChildrenImmediately(testBuilder, assembly);

            testBuilder.ApplyDecorators();
        }

        /// <summary>
        /// Creates a test for an assembly.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="assembly">The assembly</param>
        /// <returns>The test</returns>
        protected virtual PatternTest CreateAssemblyTest(IPatternTestBuilder containingTestBuilder, IAssemblyInfo assembly)
        {
            PatternTest test = new PatternTest(assembly.Name, assembly);
            test.Kind = TestKinds.Assembly;
            return test;
        }

        /// <summary>
        /// Initializes a test for an assembly after it has been added to the test model.
        /// </summary>
        /// <param name="assemblyTestBuilder">The test builder for the assembly</param>
        /// <param name="assembly">The assembly</param>
        protected virtual void InitializeAssemblyTest(IPatternTestBuilder assemblyTestBuilder, IAssemblyInfo assembly)
        {
            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTestBuilder.Test.Metadata);

            foreach (IPattern pattern in assemblyTestBuilder.TestModelBuilder.PatternResolver.GetPatterns(assembly, true))
                pattern.ProcessTest(assemblyTestBuilder, assembly);
        }

        /// <summary>
        /// Populates the children of the assembly test all at once.
        /// </summary>
        /// <param name="assemblyTestBuilder">The assembly test builder</param>
        /// <param name="assembly">The assembly</param>
        protected virtual void PopulateChildrenImmediately(IPatternTestBuilder assemblyTestBuilder, IAssemblyInfo assembly)
        {
            foreach (ITypeInfo type in assembly.GetExportedTypes())
                ProcessType(assemblyTestBuilder, type);
        }

        /// <summary>
        /// Prepares to populate the children of the assembly test on demand by
        /// adding actions to <see cref="IPatternTestBuilder.PopulateChildrenChain" />.
        /// </summary>
        /// <param name="assemblyTestBuilder">The assembly test builder</param>
        /// <param name="assembly">The assembly</param>
        protected virtual void PrepareToPopulateChildrenOnDemand(IPatternTestBuilder assemblyTestBuilder, IAssemblyInfo assembly)
        {
            HashSet<ITypeInfo> populatedTypes = new HashSet<ITypeInfo>();
            assemblyTestBuilder.PopulateChildrenChain.After(delegate(ICodeElementInfo childCodeElement)
            {
                ITypeInfo type = childCodeElement as ITypeInfo;
                if (type != null && !populatedTypes.Contains(type) && assembly.Equals(type.Assembly))
                {
                    populatedTypes.Add(type);
                    PopulateChildrenOnDemand(assemblyTestBuilder, type);
                }
            });
        }

        /// <summary>
        /// Populates the children of the assembly test on demand.
        /// </summary>
        /// <param name="assemblyTestBuilder">The assembly test builder</param>
        /// <param name="type">The type from which to populate children on demand</param>
        protected virtual void PopulateChildrenOnDemand(IPatternTestBuilder assemblyTestBuilder, ITypeInfo type)
        {
            ProcessType(assemblyTestBuilder, type);
        }

        /// <summary>
        /// Processes a type within the assembly.
        /// </summary>
        /// <param name="assemblyTestBuilder">The test builder for the assembly</param>
        /// <param name="type">The type</param>
        protected virtual void ProcessType(IPatternTestBuilder assemblyTestBuilder, ITypeInfo type)
        {
            IPattern pattern = GetPrimaryTypePattern(assemblyTestBuilder.TestModelBuilder.PatternResolver, type);
            if (pattern != null)
                pattern.Consume(assemblyTestBuilder, type, false);
        }

        /// <summary>
        /// Validates whether the attribute has been applied to a valid <see cref="IAssemblyInfo" />.
        /// Called by <see cref="Consume" />.
        /// </summary>
        /// <remarks>
        /// The default implementation does nothing.
        /// </remarks>
        /// <param name="assembly">The assembly</param>
        /// <exception cref="ModelException">Thrown if the attribute is applied to an inappropriate assembly</exception>
        protected virtual void Validate(IAssemblyInfo assembly)
        {
        }

        /// <summary>
        /// Gets the default pattern to apply to types that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <c>null</c>.
        /// </remarks>
        protected virtual IPattern DefaultTypePattern
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the primary pattern of a type, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="type">The type</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryTypePattern(IPatternResolver patternResolver, ITypeInfo type)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, type) ?? DefaultTypePattern;
        }

        private sealed class DefaultImpl : AssemblyPatternAttribute
        {
        }
    }
}