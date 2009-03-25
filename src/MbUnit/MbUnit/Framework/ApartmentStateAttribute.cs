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
using System.Threading;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Sets the apartment state to be used to run the decorated test and its children
    /// unless subsequently overridden.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If no apartment state is specified or if it is <see cref="System.Threading.ApartmentState.Unknown" />
    /// the test will inherit the apartment state of its parent test.  Consequently if the apartment
    /// state is set on the fixture then its tests will use the same apartment state unless overridden.
    /// </para>
    /// <para>
    /// The default apartment state for a test assembly is <see cref="System.Threading.ApartmentState.STA"/> and
    /// may be overridden by setting <see cref="ApartmentStateAttribute" /> attribute on the assembly.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class ApartmentStateAttribute : TestDecoratorPatternAttribute
    {
        private readonly ApartmentState apartmentState;

        /// <summary>
        /// Sets the apartment state to be used to run the decorated test and its children
        /// unless subsequently overridden.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no apartment state is specified or if it is <see cref="System.Threading.ApartmentState.Unknown" />
        /// the test will inherit the apartment state of its parent test.  Consequently if the apartment
        /// state is set on the fixture then its tests will use the same apartment state unless overridden. 
        /// </para>
        /// <para>
        /// The default apartment state for a test assembly is <see cref="System.Threading.ApartmentState.STA"/> and
        /// may be overridden by setting <see cref="ApartmentStateAttribute" /> attribute on the assembly.
        /// </para>
        /// </remarks>
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
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.ApartmentState = apartmentState;
        }
    }
}
