// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model;
using Gallio.Model.Reflection;
using MbUnit.Model;

namespace MbUnit.Model.Builder
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
        public override bool Consume(ITestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = (IAssemblyInfo)codeElement;

            MbUnitTest test = CreateAssemblyTest(containingTestBuilder, assembly);
            containingTestBuilder.Test.AddChild(test);

            ITestBuilder testBuilder = containingTestBuilder.TestModelBuilder.CreateTestBuilder(test);
            InitializeAssemblyTest(testBuilder, assembly);

            testBuilder.ApplyDecorators();
            return true;
        }

        /// <summary>
        /// Creates a test for an assembly.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="assembly">The assembly</param>
        /// <returns>The test</returns>
        protected virtual MbUnitTest CreateAssemblyTest(ITestBuilder containingTestBuilder, IAssemblyInfo assembly)
        {
            MbUnitTest test = new MbUnitTest(assembly.Name, assembly);
            test.Kind = ComponentKind.Assembly;
            return test;
        }

        /// <summary>
        /// Initializes a test for an assembly after it has been added to the test model.
        /// </summary>
        /// <param name="assemblyTestBuilder">The test builder for the assembly</param>
        /// <param name="assembly">The assembly</param>
        protected virtual void InitializeAssemblyTest(ITestBuilder assemblyTestBuilder, IAssemblyInfo assembly)
        {
            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTestBuilder.Test.Metadata);

            foreach (IPattern pattern in assemblyTestBuilder.TestModelBuilder.ReflectionPolicy.GetPatterns(assembly))
                pattern.ProcessTest(assemblyTestBuilder, assembly);

            foreach (ITypeInfo type in assembly.GetExportedTypes())
                ProcessType(assemblyTestBuilder, type);
        }

        /// <summary>
        /// Processes a type within the assembly.
        /// </summary>
        /// <param name="assemblyTestBuilder">The test builder for the assembly</param>
        /// <param name="type">The type</param>
        /// <returns>True if the type was consumed</returns>
        protected virtual bool ProcessType(ITestBuilder assemblyTestBuilder, ITypeInfo type)
        {
            return BuilderUtils.ConsumeWithFallback(assemblyTestBuilder, type, ProcessTypeFallback);
        }

        /// <summary>
        /// Processes a type using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="assemblyTestBuilder">The test builder for the assembly</param>
        /// <param name="type">The type</param>
        /// <returns>True if the type was consumed</returns>
        protected virtual bool ProcessTypeFallback(ITestBuilder assemblyTestBuilder, ITypeInfo type)
        {
            return false;
        }

        private sealed class DefaultImpl : AssemblyPatternAttribute
        {
        }
    }
}
