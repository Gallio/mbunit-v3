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
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Framework.Data;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Declares that a type represents a test.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subclasses of this attribute can control what happens with the type.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given class.
    /// </para>
    /// <para>
    /// A test type has no timeout by default.
    /// </para>
    /// </remarks>
    /// <seealso cref="TestTypeDecoratorPatternAttribute"/>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class TestTypePatternAttribute : PatternAttribute
    {
        private static readonly Key<ObjectCreationSpec> FixtureObjectCreationSpecKey = new Key<ObjectCreationSpec>("FixtureObjectCreationSpec");

        private const BindingFlags ConstructorBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags NestedTypeBindingFlags = BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Gets an instance of the test type pattern attribute to use when no
        /// other pattern consumes the type. 
        /// </summary>
        /// <remarks>
        /// If the type can be inferred to be a test type then the pattern will behave as if the type has a 
        /// test type pattern attribute applied to it. Otherwise it will simply recurse into nested types.
        /// </remarks>
        /// <seealso cref="InferTestType"/>
        public static readonly TestTypePatternAttribute AutomaticInstance = new AutomaticImpl();

        /// <summary>
        /// Gets or sets a number that defines an ordering for the test with respect to its siblings.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Unless compelled otherwise by test dependencies, tests with a lower order number than
        /// their siblings will run before those siblings and tests with the same order number
        /// as their siblings with run in an arbitrary sequence with respect to those siblings.
        /// </para>
        /// </remarks>
        /// <value>The test execution order with respect to siblings, initially zero.</value>
        public int Order { get; set; }

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override IList<TestPart> GetTestParts(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return new[] { new TestPart() { IsTestContainer = true } };
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            var type = codeElement as ITypeInfo;
            Validate(containingScope, type);

            IPatternScope typeScope = containingScope.CreateChildTestScope(type.Name, type);
            typeScope.TestBuilder.Kind = TestKinds.Fixture;
            typeScope.TestBuilder.Order = Order;
                
            InitializeTest(typeScope, type);
            SetTestSemantics(typeScope.TestBuilder, type);

            typeScope.TestBuilder.ApplyDeferredActions();
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly.</exception>
        protected virtual void Validate(IPatternScope containingScope, ITypeInfo type)
        {
            if (!containingScope.CanAddChildTest || type == null)
                ThrowUsageErrorException("This attribute can only be used on a test type within a test assembly.");
            if (!type.IsClass || type.ElementType != null)
                ThrowUsageErrorException("This attribute can only be used on a class.");
        }

        /// <summary>
        /// Initializes a test for a type after it has been added to the test model.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The members of base types are processed before those of subtypes.
        /// </para>
        /// <para>
        /// The default implementation processes all public members of the type including
        /// the first constructor found, then recurses to process all public and non-public
        /// nested types.  Non-public members other than nested types are ignored.
        /// </para>
        /// </remarks>
        /// <param name="typeScope">The type scope.</param>
        /// <param name="type">The type.</param>
        protected virtual void InitializeTest(IPatternScope typeScope, ITypeInfo type)
        {
            string xmlDocumentation = type.GetXmlDocumentation();
            if (xmlDocumentation != null)
                typeScope.TestBuilder.AddMetadata(MetadataKeys.XmlDocumentation, xmlDocumentation);

            typeScope.Process(type);

            if (type.IsGenericTypeDefinition)
            {
                foreach (IGenericParameterInfo parameter in type.GenericArguments)
                    typeScope.Consume(parameter, false, DefaultGenericParameterPattern);
            }

            ConsumeMembers(typeScope, type);
            ConsumeConstructors(typeScope, type);
            ConsumeNestedTypes(typeScope, type);
        }

        /// <summary>
        /// Consumes type members including fields, properties, methods and events.
        /// </summary>
        /// <param name="typeScope">The scope to be used as the containing scope.</param>
        /// <param name="type">The type whose members are to be consumed.</param>
        protected void ConsumeMembers(IPatternScope typeScope, ITypeInfo type)
        {
            BindingFlags bindingFlags = GetMemberBindingFlags(type);

            // TODO: We should probably process groups of members in sorted order working outwards
            //       from the base type, like an onion.
            foreach (IFieldInfo field in CodeElementSorter.SortMembersByDeclaringType(type.GetFields(bindingFlags)))
                typeScope.Consume(field, false, DefaultFieldPattern);

            foreach (IPropertyInfo property in CodeElementSorter.SortMembersByDeclaringType(type.GetProperties(bindingFlags)))
                typeScope.Consume(property, false, DefaultPropertyPattern);

            foreach (IMethodInfo method in CodeElementSorter.SortMembersByDeclaringType(type.GetMethods(bindingFlags)))
                typeScope.Consume(method, false, DefaultMethodPattern);

            foreach (IEventInfo @event in CodeElementSorter.SortMembersByDeclaringType(type.GetEvents(bindingFlags)))
                typeScope.Consume(@event, false, DefaultEventPattern);
        }

        /// <summary>
        /// Consumes type constructors.
        /// </summary>
        /// <param name="typeScope">The scope to be used as the containing scope.</param>
        /// <param name="type">The type whose constructors are to be consumed.</param>
        protected void ConsumeConstructors(IPatternScope typeScope, ITypeInfo type)
        {
            if (ShouldConsumeConstructors(type))
            {
                // FIXME: Currently we arbitrarily choose the first constructor and throw away the rest.
                //        This should be replaced by a more intelligent mechanism that supports a constructor
                //        selection policy based on some criterion.
                IConstructorInfo constructor = GetFirstConstructorWithPreferenceForPublicConsructor(type);
                if (constructor != null)
                    typeScope.Consume(constructor, false, DefaultConstructorPattern);
            }
        }

        private static IConstructorInfo GetFirstConstructorWithPreferenceForPublicConsructor(ITypeInfo type)
        {
            IConstructorInfo result = null;

            foreach (IConstructorInfo constructor in type.GetConstructors(ConstructorBindingFlags))
            {
                if (constructor.IsPublic)
                    return constructor;

                if (result == null)
                    result = constructor;
            }

            return result;
        }

        private static bool ShouldConsumeConstructors(ITypeInfo type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        /// <summary>
        /// Consumes nested types.
        /// </summary>
        /// <param name="typeScope">The scope to be used as the containing scope.</param>
        /// <param name="type">The type whose nested types are to be consumed.</param>
        protected void ConsumeNestedTypes(IPatternScope typeScope, ITypeInfo type)
        {
            foreach (ITypeInfo nestedType in type.GetNestedTypes(NestedTypeBindingFlags))
                typeScope.Consume(nestedType, false, DefaultNestedTypePattern);
        }

        /// <summary>
        /// Applies semantic actions to a test to estalish its runtime behavior.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called after <see cref="InitializeTest" />.
        /// </para>
        /// <para>
        /// The default behavior for a <see cref="TestTypePatternAttribute" />
        /// is to configure the test actions as follows:
        /// <list type="bullet">
        /// <item><see cref="PatternTestInstanceActions.BeforeTestInstanceChain" />: Set the
        /// fixture instance name and <see cref="PatternTestInstanceState.FixtureType" />.</item>
        /// <item><see cref="PatternTestInstanceActions.InitializeTestInstanceChain" />: Create
        /// the fixture instance and set the <see cref="PatternTestInstanceState.FixtureInstance" />
        /// property accordingly.</item>
        /// <item><see cref="PatternTestInstanceActions.DisposeTestInstanceChain" />: If the fixture type
        /// implements <see cref="IDisposable" />, disposes the fixture instance.</item>
        /// <item><see cref="PatternTestInstanceActions.DecorateChildTestChain" />: Decorates the child's
        /// <see cref="PatternTestInstanceActions.BeforeTestInstanceChain" /> to set its <see cref="PatternTestInstanceState.FixtureInstance" />
        /// and <see cref="PatternTestInstanceState.FixtureType" /> properties to those
        /// of the fixture.  The child test may override these values later on but this
        /// is a reasonable default setting for test methods within a fixture.</item>
        /// </list>
        /// </para>
        /// <para>
        /// You can override this method to change the semantics as required.
        /// </para>
        /// </remarks>
        /// <param name="testBuilder">The test builder.</param>
        /// <param name="type">The test type.</param>
        protected virtual void SetTestSemantics(ITestBuilder testBuilder, ITypeInfo type)
        {
            testBuilder.TestInstanceActions.BeforeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    ObjectCreationSpec spec = testInstanceState.GetFixtureObjectCreationSpec(type);
                    testInstanceState.Data.SetValue(FixtureObjectCreationSpecKey, spec);

                    testInstanceState.FixtureType = spec.ResolvedType;

                    if (!testInstanceState.IsReusingPrimaryTestStep)
                        testInstanceState.NameBase = spec.Format(testInstanceState.NameBase, testInstanceState.Formatter);
                });

            testBuilder.TestInstanceActions.InitializeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    if (!type.IsAbstract && !type.IsInterface)
                    {
                        ObjectCreationSpec spec = testInstanceState.Data.GetValue(FixtureObjectCreationSpecKey);

                        testInstanceState.FixtureInstance = spec.CreateInstance();
                    }
                });

            testBuilder.TestInstanceActions.DisposeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    IDisposable dispose = testInstanceState.FixtureInstance as IDisposable;
                    if (dispose != null)
                    {
                        dispose.Dispose();
                    }
                });

            testBuilder.TestInstanceActions.DecorateChildTestChain.After(
                delegate(PatternTestInstanceState testInstanceState, PatternTestActions decoratedTestActions)
                {
                    decoratedTestActions.TestInstanceActions.BeforeTestInstanceChain.Before(delegate(PatternTestInstanceState childTestInstanceState)
                    {
                        IMemberInfo member = childTestInstanceState.Test.CodeElement as IMemberInfo;
                        if (member != null)
                        {
                            ITypeInfo memberDeclaringType = member.DeclaringType;
                            if (memberDeclaringType != null)
                            {
                                if (type.Equals(memberDeclaringType) || type.IsSubclassOf(memberDeclaringType))
                                {
                                    childTestInstanceState.FixtureType = testInstanceState.FixtureType;
                                    childTestInstanceState.FixtureInstance = testInstanceState.FixtureInstance;
                                }
                            }
                        }
                    });
                });
        }

        /// <summary>
        /// Gets the default pattern to apply to generic parameters that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.DefaultInstance" />.
        /// </para>
        /// </remarks>
        protected virtual IPattern DefaultGenericParameterPattern
        {
            get { return TestParameterPatternAttribute.DefaultInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to methods that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns <c>null</c>.
        /// </para>
        /// </remarks>
        protected virtual IPattern DefaultMethodPattern
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the default pattern to apply to events that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns <c>null</c>.
        /// </para>
        /// </remarks>
        protected virtual IPattern DefaultEventPattern
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the default pattern to apply to fields that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.AutomaticInstance" />.
        /// </para>
        /// </remarks>
        protected virtual IPattern DefaultFieldPattern
        {
            get { return TestParameterPatternAttribute.AutomaticInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to properties that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.AutomaticInstance" />.
        /// </para>
        /// </remarks>
        protected virtual IPattern DefaultPropertyPattern
        {
            get { return TestParameterPatternAttribute.AutomaticInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to constructors that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns <see cref="TestConstructorPatternAttribute.DefaultInstance" />.
        /// </para>
        /// </remarks>
        protected virtual IPattern DefaultConstructorPattern
        {
            get { return TestConstructorPatternAttribute.DefaultInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to nested types that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns <see cref="TestTypePatternAttribute.AutomaticInstance"/>.
        /// </para>
        /// </remarks>
        protected virtual IPattern DefaultNestedTypePattern
        {
            get { return AutomaticInstance; }
        }

        /// <summary>
        /// Gets the binding flags that should be used to enumerate non-nested type members
        /// of the type for determining their contribution to the test fixture.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance members are only included if the type is not abstract.
        /// </para>
        /// </remarks>
        /// <param name="type">The type.</param>
        /// <returns>The binding flags for enumerating members.</returns>
        protected virtual BindingFlags GetMemberBindingFlags(ITypeInfo type)
        {
            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            if (!type.IsAbstract)
                bindingFlags |= BindingFlags.Instance;
            return bindingFlags;
        }

        /// <summary>
        /// Infers whether the type is a test type based on its structure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns true if the type any associated patterns, if it has
        /// non-nested type members (subject to <see cref="GetMemberBindingFlags" />)
        /// with patterns, if it has generic parameters with patterns, or if any
        /// of its nested types satisfy the preceding rules.
        /// </para>
        /// </remarks>
        /// <param name="evaluator">The pattern evaluator.</param>
        /// <param name="type">The type.</param>
        /// <returns>True if the type is likely a test type.</returns>
        protected virtual bool InferTestType(IPatternEvaluator evaluator, ITypeInfo type)
        {
            if (evaluator.HasPatterns(type))
                return true;

            BindingFlags bindingFlags = GetMemberBindingFlags(type);
            if (HasCodeElementWithPattern(evaluator, type.GetMethods(bindingFlags))
                || HasCodeElementWithPattern(evaluator, type.GetProperties(bindingFlags))
                || HasCodeElementWithPattern(evaluator, type.GetFields(bindingFlags))
                || HasCodeElementWithPattern(evaluator, type.GetEvents(bindingFlags)))
                return true;

            if (type.IsGenericTypeDefinition && HasCodeElementWithPattern(evaluator, type.GenericArguments))
                return true;

            if (ShouldConsumeConstructors(type)
                && HasCodeElementWithPattern(evaluator, type.GetConstructors(ConstructorBindingFlags)))
                return true;

            foreach (ITypeInfo nestedType in type.GetNestedTypes(NestedTypeBindingFlags))
                if (InferTestType(evaluator, nestedType))
                    return true;

            return false;
        }

        private static bool HasCodeElementWithPattern<T>(IPatternEvaluator evaluator, IEnumerable<T> elements)
            where T : ICodeElementInfo
        {
            foreach (T element in elements)
                if (evaluator.HasPatterns(element))
                    return true;
            return false;
        }

        private sealed class AutomaticImpl : TestTypePatternAttribute
        {
            public override IList<TestPart> GetTestParts(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
            {
                if (IsTest(evaluator, codeElement))
                    return base.GetTestParts(evaluator, codeElement);
                return EmptyArray<TestPart>.Instance;
            }

            public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
            {
                if (IsTest(containingScope.Evaluator, codeElement))
                    base.Consume(containingScope, codeElement, skipChildren);
            }

            private bool IsTest(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
            {
                ITypeInfo type = codeElement as ITypeInfo;
                return type != null && InferTestType(evaluator, type);
            }
        }
    }
}