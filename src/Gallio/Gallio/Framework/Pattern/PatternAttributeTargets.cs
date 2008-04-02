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
        public const AttributeTargets Test = TestAssembly | TestType | TestMethod;

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
    }
}
