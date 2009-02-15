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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Standard <see cref="AttributeTargets"/> flag combinations for <see cref="PatternAttribute" /> declarations.
    /// </summary>
    public static class PatternAttributeTargets
    {
        /// <summary>
        /// Valid attribute targets for tests.
        /// </summary>
        public const AttributeTargets Test = TestAssembly | TestType | TestMethod | TestContract;

        /// <summary>
        /// Valid attribute targets for test assemblies.
        /// </summary>
        public const AttributeTargets TestAssembly = AttributeTargets.Assembly;

        /// <summary>
        /// Valid attribute targets for test types.
        /// </summary>
        public const AttributeTargets TestType = AttributeTargets.Class;

        /// <summary>
        /// Valid attribute targets for test methods.
        /// </summary>
        public const AttributeTargets TestMethod = AttributeTargets.Method;

        /// <summary>
        /// Valid attribute targets for test parameters.
        /// </summary>
        public const AttributeTargets TestParameter = AttributeTargets.Field | AttributeTargets.Property
            | AttributeTargets.Parameter | AttributeTargets.GenericParameter;

        /// <summary>
        /// Valid attribute targets for tests or test parameters.
        /// </summary>
        public const AttributeTargets TestComponent = Test | TestParameter;

        /// <summary>
        /// Valid attribute targets for test constructors.
        /// </summary>
        public const AttributeTargets TestConstructor = AttributeTargets.Constructor;

        /// <summary>
        /// Valid attribute targets for data contexts.
        /// </summary>
        public const AttributeTargets DataContext = Test | TestParameter | AttributeTargets.Constructor;

        /// <summary>
        /// Valid attribute targets for contribution methods.
        /// </summary>
        public const AttributeTargets ContributionMethod = AttributeTargets.Method;

        /// <summary>
        /// Valid attribute targets for test contracts.
        /// </summary>
        public const AttributeTargets TestContract = AttributeTargets.Field;
    }
}
