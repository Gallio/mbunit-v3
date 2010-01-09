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
using System.Collections.Generic;
using Gallio.Common.Collections;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies a method that is to be invoked after a fixture instance has been
    /// created to complete its initialization before test fixture setup runs.
    /// </summary>
    /// <remarks>
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
    /// <para>
    /// The attribute may be applied to multiple methods within a fixture, however
    /// the order in which they are processed is undefined.
    /// </para>
    /// <para>
    /// The method to which this attribute is applied must be declared by the
    /// fixture class and must not have any parameters.  The method may be static.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public class FixtureInitializerAttribute : ContributionMethodPatternAttribute
    {
        /// <inheritdoc />
        protected override void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            base.Validate(containingScope, method);

            if (method.Parameters.Count != 0)
                ThrowUsageErrorException("A fixture initializer method must not have any parameters.");
        }

        /// <inheritdoc />
        protected override void DecorateContainingScope(IPatternScope containingScope, IMethodInfo method)
        {
            containingScope.TestBuilder.TestInstanceActions.InitializeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    testInstanceState.InvokeFixtureMethod(method, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
                });
        }
    }
}