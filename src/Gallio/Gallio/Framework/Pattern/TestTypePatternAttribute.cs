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
using System.Reflection;
using Gallio.Collections;
using Gallio.Framework.Data;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Declares that a type represents an <see cref="PatternTest" />.
    /// Subclasses of this attribute can control what happens with the type.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given class.
    /// </para>
    /// </summary>
    /// <seealso cref="TestTypeDecoratorPatternAttribute"/>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class TestTypePatternAttribute : PatternAttribute
    {
        private static readonly Key<ObjectCreationSpec> FixtureObjectCreationSpecKey = new Key<ObjectCreationSpec>("FixtureObjectCreationSpec");

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(PatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            ITypeInfo type = codeElement as ITypeInfo;
            Validate(containingScope, type);

            PatternTest typeTest = CreateTest(containingScope, type);
            PatternEvaluationScope typeScope = containingScope.AddChildTest(typeTest);
            InitializeTest(typeScope, type);
            SetTestSemantics(typeTest, type);

            typeScope.ApplyDecorators();
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="type">The type</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(PatternEvaluationScope containingScope, ITypeInfo type)
        {
            if (!containingScope.CanAddChildTest || type == null)
                ThrowUsageErrorException("This attribute can only be used on a test type within a test assembly.");
            if (!type.IsClass || type.ElementType != null)
                ThrowUsageErrorException("This attribute can only be used on a class.");
        }

        /// <summary>
        /// Creates a test for a type.
        /// </summary>
        /// <param name="constainingScope">The containing scope</param>
        /// <param name="type">The type</param>
        /// <returns>The test</returns>
        protected virtual PatternTest CreateTest(PatternEvaluationScope constainingScope, ITypeInfo type)
        {
            PatternTest test = new PatternTest(type.Name, type, constainingScope.TestDataContext.CreateChild());
            test.Kind = TestKinds.Fixture;
            return test;
        }

        /// <summary>
        /// <para>
        /// Initializes a test for a type after it has been added to the test model.
        /// </para>
        /// <para>
        /// The members of base types are processed before those of subtypes.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The default implementation processes all public members of the type including
        /// the first constructor found, then recurses to process all public and non-public
        /// nested types.  Non-public members other than nested types are ignored.
        /// </remarks>
        /// <param name="typeScope">The type scope</param>
        /// <param name="type">The type</param>
        protected virtual void InitializeTest(PatternEvaluationScope typeScope, ITypeInfo type)
        {
            string xmlDocumentation = type.GetXmlDocumentation();
            if (xmlDocumentation != null)
                typeScope.Test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            typeScope.Process(type);

            if (type.IsGenericTypeDefinition)
            {
                foreach (IGenericParameterInfo parameter in type.GenericArguments)
                    typeScope.Consume(parameter, false, DefaultGenericParameterPattern);
            }

            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;
            if (! type.IsAbstract)
                bindingFlags |= BindingFlags.Instance;

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

            // Note: We only consider instance members of concrete types because abstract types
            //       cannot be instantiated so the members cannot be accessed.  An abstract type
            //       might yet be a static test fixture so we still consider its static members.
            if (!type.IsAbstract && !type.IsInterface)
            {
                foreach (IConstructorInfo constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
                {
                    typeScope.Consume(constructor, false, DefaultConstructorPattern);

                    // FIXME: Currently we arbitrarily choose the first constructor and throw away the rest.
                    //        This should be replaced by a more intelligent mechanism that supports a constructor
                    //        selection policy based on some criterion.
                    break;
                }
            }

            foreach (ITypeInfo nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
                typeScope.Consume(nestedType, false, DefaultNestedTypePattern);
        }

        /// <summary>
        /// <para>
        /// Applies semantic actions to the <see cref="PatternTest.TestActions" /> member of a 
        /// test to set the test's runtime behavior.
        /// </para>
        /// <para>
        /// This method is called after <see cref="InitializeTest" />.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default behavior for a <see cref="TestTypePatternAttribute" />
        /// is to configure the test actions as follows:
        /// <list type="bullet">
        /// <item><see cref="IPatternTestInstanceHandler.BeforeTestInstance" />: Set the
        /// fixture instance name and <see cref="PatternTestInstanceState.FixtureType" />.</item>
        /// <item><see cref="IPatternTestInstanceHandler.InitializeTestInstance" />: Create
        /// the fixture instance and set the <see cref="PatternTestInstanceState.FixtureInstance" />
        /// property accordingly.</item>
        /// <item><see cref="IPatternTestInstanceHandler.DisposeTestInstance" />: If the fixture type
        /// implements <see cref="IDisposable" />, disposes the fixture instance.</item>
        /// <item><see cref="IPatternTestInstanceHandler.DecorateChildTest" />: Decorates the child's
        /// <see cref="IPatternTestInstanceHandler.BeforeTestInstance" /> to set its <see cref="PatternTestInstanceState.FixtureInstance" />
        /// and <see cref="PatternTestInstanceState.FixtureType" /> properties to those
        /// of the fixture.  The child test may override these values later on but this
        /// is a reasonable default setting for test methods within a fixture.</item>
        /// </list>
        /// </para>
        /// <para>
        /// You can override this method to change the semantics as required.
        /// </para>
        /// </remarks>
        /// <param name="test">The test</param>
        /// <param name="type">The test type</param>
        protected virtual void SetTestSemantics(PatternTest test, ITypeInfo type)
        {
            test.TestInstanceActions.BeforeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    ObjectCreationSpec spec = testInstanceState.GetFixtureObjectCreationSpec(type);
                    testInstanceState.Data.SetValue(FixtureObjectCreationSpecKey, spec);

                    testInstanceState.FixtureType = spec.ResolvedType;

                    if (!testInstanceState.IsReusingPrimaryTestStep)
                        testInstanceState.TestStep.Name = spec.Format(testInstanceState.TestStep.Name, testInstanceState.Formatter);
                });

            test.TestInstanceActions.InitializeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    if (!type.IsAbstract && !type.IsInterface)
                    {
                        ObjectCreationSpec spec = testInstanceState.Data.GetValue(FixtureObjectCreationSpecKey);

                        testInstanceState.FixtureInstance = spec.CreateInstance();
                    }
                });

            test.TestInstanceActions.DisposeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    IDisposable dispose = testInstanceState.FixtureInstance as IDisposable;
                    if (dispose != null)
                    {
                        Context.CurrentContext.Sandbox.Run(delegate { dispose.Dispose(); }, "Dispose Fixture");
                    }
                });

            test.TestInstanceActions.DecorateChildTestChain.After(
                delegate(PatternTestInstanceState testInstanceState, PatternTestActions decoratedTestActions)
                {
                    decoratedTestActions.TestInstanceActions.BeforeTestInstanceChain.Before(delegate(PatternTestInstanceState childTestInstanceState)
                    {
                        IMethodInfo method = childTestInstanceState.Test.CodeElement as IMethodInfo;
                        if (method != null && (type.Equals(method.DeclaringType) || type.IsSubclassOf(method.DeclaringType)))
                        {
                            childTestInstanceState.FixtureType = testInstanceState.FixtureType;
                            childTestInstanceState.FixtureInstance = testInstanceState.FixtureInstance;
                        }
                    });
                });
        }

        /// <summary>
        /// Gets the default pattern to apply to generic parameters that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultGenericParameterPattern
        {
            get { return TestParameterPatternAttribute.DefaultInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to methods that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <c>null</c>.
        /// </remarks>
        protected virtual IPattern DefaultMethodPattern
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the default pattern to apply to events that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <c>null</c>.
        /// </remarks>
        protected virtual IPattern DefaultEventPattern
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the default pattern to apply to fields that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.AutomaticInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultFieldPattern
        {
            get { return TestParameterPatternAttribute.AutomaticInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to properties that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.AutomaticInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultPropertyPattern
        {
            get { return TestParameterPatternAttribute.AutomaticInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to constructors that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestConstructorPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultConstructorPattern
        {
            get { return TestConstructorPatternAttribute.DefaultInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to nested types that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="RecursiveTypePattern.Instance"/>.
        /// </remarks>
        protected virtual IPattern DefaultNestedTypePattern
        {
            get { return RecursiveTypePattern.Instance; }
        }
    }
}