// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
        public override bool Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            ITypeInfo type = (ITypeInfo)codeElement;
            if (!ShouldConsume(type))
                return false;

            PatternTest test = CreateTest(containingTestBuilder, type);
            IPatternTestBuilder testBuilder = containingTestBuilder.AddChild(test);
            InitializeTest(testBuilder, type);
            SetTestSemantics(test, type);

            testBuilder.ApplyDecorators();
            return true;
        }

        /// <summary>
        /// Returns true if the <see cref="Consume" /> method should proceed
        /// to call <see cref="CreateTest" /> for the specified <see cref="ITypeInfo" />.
        /// </summary>
        /// <remarks>
        /// The default implementation returns true if <paramref name="type"/> is a
        /// concrete class.  Returns false for interfaces, abstract classes, open
        /// generic type definitions, generic type parameters, generic method parameters,
        /// arrays, pointers and references.
        /// </remarks>
        /// <param name="type">The type</param>
        /// <returns>True if the type should be consumed</returns>
        protected virtual bool ShouldConsume(ITypeInfo type)
        {
            return !type.IsArray && !type.IsByRef && !type.IsPointer && type.IsClass;
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
                    ProcessSlot(typeTestBuilder, parameter);
            }

            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;
            if (! type.IsAbstract)
                bindingFlags |= BindingFlags.Instance;

            // TODO: We should probably process groups of members in sorted order working outwards
            //       from the base type, like an onion.
            foreach (IFieldInfo field in CodeElementSorter.SortMembersByDeclaringType(type.GetFields(bindingFlags)))
                ProcessSlot(typeTestBuilder, field);

            foreach (IPropertyInfo property in CodeElementSorter.SortMembersByDeclaringType(type.GetProperties(bindingFlags)))
                ProcessSlot(typeTestBuilder, property);

            foreach (IMethodInfo method in CodeElementSorter.SortMembersByDeclaringType(type.GetMethods(bindingFlags)))
                ProcessMethod(typeTestBuilder, method);

            foreach (IEventInfo @event in CodeElementSorter.SortMembersByDeclaringType(type.GetEvents(bindingFlags)))
                ProcessEvent(typeTestBuilder, @event);

            // Note: We only consider instance members of concrete types because abstract types
            //       cannot be instantiated so the members cannot be accessed.  An abstract type
            //       might yet be a static test fixture so we still consider its static members.
            if (!type.IsAbstract)
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
        /// <item><see cref="IPatternTestInstanceHandler.InitializeTestInstance" />: Create
        /// the fixture instance and set the <see cref="PatternTestInstanceState.FixtureInstance" />
        /// and <see cref="PatternTestInstanceState.FixtureType" /> properties accordingly.</item>
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
                    if (!type.IsAbstract)
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
                        TestInvoker.Run(delegate { dispose.Dispose(); }, "Dispose Fixture", null);
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
        /// Processes a field, property, constructor parameter or generic parameter slot.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="slot">The slot</param>
        /// <returns>True if the slot was consumed</returns>
        protected virtual bool ProcessSlot(IPatternTestBuilder typeTestBuilder, ISlotInfo slot)
        {
            return PatternUtils.ConsumeWithFallback(typeTestBuilder, slot, ProcessSlotFallback);
        }

        /// <summary>
        /// Processes a slot using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="slot">The slot</param>
        /// <returns>True if the slot was consumed</returns>
        protected virtual bool ProcessSlotFallback(IPatternTestBuilder typeTestBuilder, ISlotInfo slot)
        {
            if (slot is IFieldInfo || slot is IPropertyInfo)
                return false;

            return TestParameterPatternAttribute.DefaultInstance.Consume(typeTestBuilder, slot);
        }

        /// <summary>
        /// Processes a constructor.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="constructor">The constructor</param>
        /// <returns>True if the constructor was consumed</returns>
        protected virtual bool ProcessConstructor(IPatternTestBuilder typeTestBuilder, IConstructorInfo constructor)
        {
            return PatternUtils.ConsumeWithFallback(typeTestBuilder, constructor, ProcessConstructorFallback);
        }

        /// <summary>
        /// Processes a constructor using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="constructor">The constructor</param>
        /// <returns>True if the constructor was consumed</returns>
        protected virtual bool ProcessConstructorFallback(IPatternTestBuilder typeTestBuilder, IConstructorInfo constructor)
        {
            foreach (IParameterInfo parameter in constructor.Parameters)
                ProcessSlot(typeTestBuilder, parameter);

            return true;
        }

        /// <summary>
        /// Processes an event.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="event">The event</param>
        /// <returns>True if the event was consumed</returns>
        protected virtual bool ProcessEvent(IPatternTestBuilder typeTestBuilder, IEventInfo @event)
        {
            return PatternUtils.ConsumeWithFallback(typeTestBuilder, @event, ProcessEventFallback);
        }

        /// <summary>
        /// Processes an event using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="event">The event</param>
        /// <returns>True if the event was consumed</returns>
        protected virtual bool ProcessEventFallback(IPatternTestBuilder typeTestBuilder, IEventInfo @event)
        {
            return false;
        }

        /// <summary>
        /// Processes a method.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="method">The method</param>
        /// <returns>True if the method was consumed</returns>
        protected virtual bool ProcessMethod(IPatternTestBuilder typeTestBuilder, IMethodInfo method)
        {
            return PatternUtils.ConsumeWithFallback(typeTestBuilder, method, ProcessMethodFallback);
        }

        /// <summary>
        /// Processes a method using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="method">The method</param>
        /// <returns>True if the method was consumed</returns>
        protected virtual bool ProcessMethodFallback(IPatternTestBuilder typeTestBuilder, IMethodInfo method)
        {
            return false;
        }

        private sealed class DefaultImpl : TestTypePatternAttribute
        {
        }
    }
}