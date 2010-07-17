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
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies that a class represents an abstract test fixture used as a base class for concrete fixture types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The attribute hides the test methods in the current test fixture, so they can be consumed properly in the
    /// desired context from any derived concrete test fixtures.
    /// </para>
    /// <para>
    /// This attribute is not inherited.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = false)]
    public class AbstractTestFixtureAttribute : TestFixtureAttribute
    {
        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            // Nothing here!
            // Just eat up the consumption of the test methods.
        }
    }
}
