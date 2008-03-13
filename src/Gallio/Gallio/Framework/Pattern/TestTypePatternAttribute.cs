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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class TestTypePatternAttribute : PatternAttribute
    {
        private const string FixtureObjectCreationSpecKey = "FixtureObjectCreationSpec";

        /// <summary>
        /// Gets a default instance of the type pattern attribute to use
        /// when no other pattern consumes a type.
        /// </summary>
        public static readonly TestTypePatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(IPatternResolver patternResolver, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement, bool skipChildren)
        {
            ITypeInfo type = (ITypeInfo)codeElement;
            Validate(type);

            PatternTest test = CreateTest(containingTestBuilder, type);
            IPatternTestBuilder testBuilder = containingTestBuilder.AddChild(test);
            InitializeTest(testBuilder, type);
            SetTestSemantics(test, type);

            testBuilder.ApplyDecorators();
        }

        /// <summary>
        /// Validates whether the attribute has been applied to a valid <see cref="ITypeInfo" />.
        /// Called by <see cref="Consume" />.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an exception if <paramref name="type"/> is an interface,
        /// abstract class, open generic type definition, generic type parameter, generic method parameter,
        /// array, pointer or reference.
        /// </remarks>
        /// <param name="type">The type</param>
        /// <exception cref="ModelException">Thrown if the attribute is applied to an inappropriate type</exception>
        protected virtual void Validate(ITypeInfo type)
        {
            if (! type.IsClass || type.IsArray || type.IsByRef || type.IsPointer || type.ContainsGenericParameters)
                throw new ModelException(String.Format("The {0} attribute is not valid for use on type '{1}'.  The type must be a concrete class.", GetType().Name, type));
        }

        /// <summary>
        /// Creates a test for a type.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="type">The type</param>
        /// <returns>The test</returns>
        protected virtual PatternTest CreateTest(IPatternTestBuilder containingTestBuilder, ITypeInfo type)
        {
            PatternTest test = new PatternTest(type.Name, type);
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
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="type">The type</param>
        protected virtual void InitializeTest(IPatternTestBuilder typeTestBuilder, ITypeInfo type)
        {
            string xmlDocumentation = type.GetXmlDocumentation();
            if (xmlDocumentation != null)
                typeTestBuilder.Test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            foreach (IPattern pattern in typeTestBuilder.TestModelBuilder.PatternResolver.GetPatterns(type, true))
                pattern.ProcessTest(typeTestBuilder, type);

            if (type.IsGenericTypeDefinition)
            {
                foreach (IGenericParameterInfo parameter in type.GenericArguments)
                    ProcessGenericParameter(typeTestBuilder, parameter);
            }

            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;
            if (! type.IsAbstract)
                bindingFlags |= BindingFlags.Instance;

            // TODO: We should probably process groups of members in sorted order working outwards
            //       from the base type, like an onion.
            foreach (IFieldInfo field in CodeElementSorter.SortMembersByDeclaringType(type.GetFields(bindingFlags)))
                ProcessField(typeTestBuilder, field);

            foreach (IPropertyInfo property in CodeElementSorter.SortMembersByDeclaringType(type.GetProperties(bindingFlags)))
                ProcessProperty(typeTestBuilder, property);

            foreach (IMethodInfo method in CodeElementSorter.SortMembersByDeclaringType(type.GetMethods(bindingFlags)))
                ProcessMethod(typeTestBuilder, method);

            foreach (IEventInfo @event in CodeElementSorter.SortMembersByDeclaringType(type.GetEvents(bindingFlags)))
                ProcessEvent(typeTestBuilder, @event);

            // Note: We only consider instance members of concrete types because abstract types
            //       cannot be instantiated so the members cannot be accessed.  An abstract type
            //       might yet be a static test fixture so we still consider its static members.
            if (!type.IsAbstract && !type.IsInterface)
            {
                foreach (IConstructorInfo constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
                {
                    ProcessConstructor(typeTestBuilder, constructor);

                    // FIXME: Currently we arbitrarily choose the first constructor and throw away the rest.
                    //        This should be replaced by a more intelligent mechanism that supports a constructor
                    //        selection policy based on some criterion.
                    break;
                }
            }
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

                    testInstanceState.TestInstance.Name += spec.Format(testInstanceState.Formatter);
                    testInstanceState.FixtureType = spec.ResolvedType;
                });

            test.TestInstanceActions.InitializeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    if (!type.IsAbstract && !type.IsInterface)
                    {
                        ObjectCreationSpec spec = testInstanceState.Data.GetValue<ObjectCreationSpec>(FixtureObjectCreationSpecKey);

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
        /// The default implementation returns <see cref="TestParameterPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultFieldPattern
        {
            get { return TestParameterPatternAttribute.DefaultInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to properties that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultPropertyPattern
        {
            get { return TestParameterPatternAttribute.DefaultInstance; }
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
        /// Gets the primary pattern of a generic parameter, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="genericParameter">The generic parameter</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryGenericParameterPattern(IPatternResolver patternResolver, IGenericParameterInfo genericParameter)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, genericParameter) ?? DefaultGenericParameterPattern;
        }

        /// <summary>
        /// Gets the primary pattern of a method, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="method">The method</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryMethodPattern(IPatternResolver patternResolver, IMethodInfo method)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, method) ?? DefaultMethodPattern;
        }

        /// <summary>
        /// Gets the primary pattern of an event, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="event">The event</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryEventPattern(IPatternResolver patternResolver, IEventInfo @event)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, @event) ?? DefaultEventPattern;
        }

        /// <summary>
        /// Gets the primary pattern of a field, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="field">The field</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryFieldPattern(IPatternResolver patternResolver, IFieldInfo field)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, field) ?? DefaultFieldPattern;
        }

        /// <summary>
        /// Gets the primary pattern of a property, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="property">The property</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryPropertyPattern(IPatternResolver patternResolver, IPropertyInfo property)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, property) ?? DefaultPropertyPattern;
        }

        /// <summary>
        /// Gets the primary pattern of a constructor, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="constructor">The constructor</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryConstructorPattern(IPatternResolver patternResolver, IConstructorInfo constructor)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, constructor) ?? DefaultConstructorPattern;
        }

        /// <summary>
        /// Processes a generic parameter.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="genericParameter">The generic parameter</param>
        protected virtual void ProcessGenericParameter(IPatternTestBuilder typeTestBuilder, IGenericParameterInfo genericParameter)
        {
            IPattern pattern = GetPrimaryGenericParameterPattern(typeTestBuilder.TestModelBuilder.PatternResolver, genericParameter);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, genericParameter, false);
        }

        /// <summary>
        /// Processes a method.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="method">The method</param>
        protected virtual void ProcessMethod(IPatternTestBuilder typeTestBuilder, IMethodInfo method)
        {
            IPattern pattern = GetPrimaryMethodPattern(typeTestBuilder.TestModelBuilder.PatternResolver, method);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, method, false);
        }

        /// <summary>
        /// Processes an event.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="event">The event</param>
        protected virtual void ProcessEvent(IPatternTestBuilder typeTestBuilder, IEventInfo @event)
        {
            IPattern pattern = GetPrimaryEventPattern(typeTestBuilder.TestModelBuilder.PatternResolver, @event);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, @event, false);
        }

        /// <summary>
        /// Processes a field.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="field">The field</param>
        protected virtual void ProcessField(IPatternTestBuilder typeTestBuilder, IFieldInfo field)
        {
            IPattern pattern = GetPrimaryFieldPattern(typeTestBuilder.TestModelBuilder.PatternResolver, field);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, field, false);
        }

        /// <summary>
        /// Processes a property.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="property">The property</param>
        protected virtual void ProcessProperty(IPatternTestBuilder typeTestBuilder, IPropertyInfo property)
        {
            IPattern pattern = GetPrimaryPropertyPattern(typeTestBuilder.TestModelBuilder.PatternResolver, property);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, property, false);
        }

        /// <summary>
        /// Processes a constructor.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="constructor">The constructor</param>
        protected virtual void ProcessConstructor(IPatternTestBuilder typeTestBuilder, IConstructorInfo constructor)
        {
            IPattern pattern = GetPrimaryConstructorPattern(typeTestBuilder.TestModelBuilder.PatternResolver, constructor);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, constructor, false);
        }

        private sealed class DefaultImpl : TestTypePatternAttribute
        {
        }
    }
}