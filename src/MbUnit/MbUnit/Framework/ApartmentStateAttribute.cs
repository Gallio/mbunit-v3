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
using System.Threading;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Sets the apartment state to be used to run the decorated test.
    /// </para>
    /// <para>
    /// If no apartment state is specified or if it is <see cref="System.Threading.ApartmentState.Unknown" />
    /// the test will inherit the apartment state of its parent.  Otherwise
    /// it will run in a thread with the specified apartment state.
    /// </para>
    /// <para>
    /// The test runner guarantees that the root test runs with the <see cref="System.Threading.ApartmentState.STA" />
    /// apartment state.  Consequently the apartment state only needs to be overridden to run 
    /// a test in some mode that may differ from that which it would ordinarily inherit.
    /// </para>
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class ApartmentStateAttribute : TestDecoratorPatternAttribute
    {
        private readonly ApartmentState apartmentState;

        /// <summary>
        /// <para>
        /// Sets the apartment state to be used to run the decorated test.
        /// </para>
        /// <para>
        /// If no apartment state is specified or if it is <see cref="System.Threading.ApartmentState.Unknown" />
        /// the test will inherit the apartment state of its parent.  Otherwise
        /// it will run in a thread with the specified apartment state.
        /// </para>
        /// <para>
        /// The test runner guarantees that the root test runs with the <see cref="System.Threading.ApartmentState.STA" />
        /// apartment state.  Consequently the apartment state only needs to be overridden to run 
        /// a test in some mode that may differ from that which it would ordinarily inherit.
        /// </para>
        /// </summary>
        /// <param name="apartmentState">The apartment state to use</param>
        public ApartmentStateAttribute(ApartmentState apartmentState)
        {
            this.apartmentState = apartmentState;
        }

        /// <summary>
        /// Gets the apartment state to be used to run the decorated test.
        /// </summary>
        public ApartmentState ApartmentState
        {
            get { return apartmentState; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            scope.Test.ApartmentState = apartmentState;
        }
    }
}
