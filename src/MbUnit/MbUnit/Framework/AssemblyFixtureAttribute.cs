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
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The assembly fixture attribute is applied to a class that contains setup and
    /// teardown methods that are to be applied at the assembly level.  Conceptually,
    /// the <see cref="AssemblyFixtureAttribute" /> adds new behavior to an assembly-level
    /// test fixture that contains all of the test fixtures within the assembly.
    /// </para>
    /// <para>
    /// The following attributes are typically used within an assembly fixture:
    /// <list type="bullet">
    /// <item><see cref="FixtureSetUpAttribute" />: Performs setup activities before any
    /// test fixtures within the assembly are executed.</item>
    /// <item><see cref="FixtureTearDownAttribute" />: Performs teardown activities after all
    /// test fixtures within the assembly are executed.</item>
    /// <item><see cref="SetUpAttribute" />: Performs setup activities before each
    /// test fixture within the assembly is executed.</item>
    /// <item><see cref="TearDownAttribute" />: Performs teardown activities after eacj
    /// test fixture within the assembly is executed.</item>
    /// </list>
    /// </para>
    /// <para>
    /// It is also possible to use other attributes as with an ordinary <see cref="TestFixtureAttribute" />.
    /// An assembly fixture also supports data binding.  When data binding is used on an assembly
    /// fixture, it will cause all test fixtures within the assembly to run once for each combination
    /// of data values used.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The class must have a public default constructor.  The class may not be static.
    /// </para>
    /// <para>
    /// There must only be at most one class with an <see cref="AssemblyFixtureAttribute" />
    /// within any given assembly.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class AssemblyFixtureAttribute : TestTypePatternAttribute
    {
        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            ITypeInfo type = codeElement as ITypeInfo;
            Validate(containingScope, type);

            PatternTest assemblyTest = containingScope.Test;
            InitializeTest(containingScope, type);
            SetTestSemantics(assemblyTest, type);
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope containingScope, ITypeInfo type)
        {
            base.Validate(containingScope, type);

            if (containingScope.Test.Kind != TestKinds.Assembly)
                ThrowUsageErrorException("This attribute can only be used on a non-nested class.");
        }

        /// <inheritdoc />
        protected override void SetTestSemantics(PatternTest test, ITypeInfo type)
        {
            test.TestActions.TestInstanceActions.BeforeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    if (testInstanceState.FixtureType != null)
                        ThrowUsageErrorException("There appears to already be a fixture defined for the assembly.");
                });

            base.SetTestSemantics(test, type);
        }
    }
}
