// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model;
using Gallio.Reflection;
using MbUnit.Model.Builder;
using MbUnit.Model.Patterns;

namespace MbUnit.Model.Patterns
{
    /// <summary>
    /// <para>
    /// Declares that a type represents an <see cref="MbUnitTest" />.
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
        /// <summary>
        /// Gets a default instance of the type pattern attribute to use
        /// when no other pattern consumes a type.
        /// </summary>
        public static readonly TestTypePatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool Consume(ITestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            ITypeInfo type = (ITypeInfo)codeElement;

            MbUnitTest test = CreateTest(containingTestBuilder, type);
            ITestBuilder testBuilder = containingTestBuilder.AddChild(test);
            InitializeTest(testBuilder, type);

            testBuilder.ApplyDecorators();
            return true;
        }

        /// <summary>
        /// Creates a test for a type.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="type">The type</param>
        /// <returns>The test</returns>
        protected virtual MbUnitTest CreateTest(ITestBuilder containingTestBuilder, ITypeInfo type)
        {
            MbUnitTest test = new MbUnitTest(type.Name, type);
            test.Kind = TestKinds.Fixture;
            return test;
        }

        /// <summary>
        /// Initializes a test for a type after it has been added to the test model.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="type">The type</param>
        protected virtual void InitializeTest(ITestBuilder typeTestBuilder, ITypeInfo type)
        {
            string xmlDocumentation = type.GetXmlDocumentation();
            if (xmlDocumentation != null)
                typeTestBuilder.Test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            foreach (IPattern pattern in typeTestBuilder.TestModelBuilder.PatternResolver.GetPatterns(type))
                pattern.ProcessTest(typeTestBuilder, type);

            foreach (IGenericParameterInfo parameter in type.GetGenericParameters())
                ProcessSlot(typeTestBuilder, parameter);

            foreach (IFieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                ProcessSlot(typeTestBuilder, field);

            foreach (IPropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                ProcessSlot(typeTestBuilder, property);

            foreach (IConstructorInfo constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
            {
                ProcessConstructor(typeTestBuilder, constructor);

                // FIXME: Currently we arbitrarily choose the first constructor and throw away the rest.
                //        This should be replaced by a more intelligent mechanism that can
                //        handle optional or alternative dependencies.  We might benefit from
                //        using an existing inversion of control framework like Castle
                //        to handle stuff like this.
                break;
            }

            foreach (IMethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                ProcessMethod(typeTestBuilder, method);

            foreach (IEventInfo @event in type.GetEvents(BindingFlags.Instance | BindingFlags.Public))
                ProcessEvent(typeTestBuilder, @event);
        }

        /// <summary>
        /// Processes a field, property, constructor parameter or generic parameter slot.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="slot">The slot</param>
        /// <returns>True if the slot was consumed</returns>
        protected virtual bool ProcessSlot(ITestBuilder typeTestBuilder, ISlotInfo slot)
        {
            return PatternUtils.ConsumeWithFallback(typeTestBuilder, slot, ProcessSlotFallback);
        }

        /// <summary>
        /// Processes a slot using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="slot">The slot</param>
        /// <returns>True if the slot was consumed</returns>
        protected virtual bool ProcessSlotFallback(ITestBuilder typeTestBuilder, ISlotInfo slot)
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
        protected virtual bool ProcessConstructor(ITestBuilder typeTestBuilder, IConstructorInfo constructor)
        {
            return PatternUtils.ConsumeWithFallback(typeTestBuilder, constructor, ProcessConstructorFallback);
        }

        /// <summary>
        /// Processes a constructor using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="constructor">The constructor</param>
        /// <returns>True if the constructor was consumed</returns>
        protected virtual bool ProcessConstructorFallback(ITestBuilder typeTestBuilder, IConstructorInfo constructor)
        {
            foreach (IParameterInfo parameter in constructor.GetParameters())
                ProcessSlot(typeTestBuilder, parameter);

            return true;
        }

        /// <summary>
        /// Processes an event.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="event">The event</param>
        /// <returns>True if the event was consumed</returns>
        protected virtual bool ProcessEvent(ITestBuilder typeTestBuilder, IEventInfo @event)
        {
            return PatternUtils.ConsumeWithFallback(typeTestBuilder, @event, ProcessEventFallback);
        }

        /// <summary>
        /// Processes an event using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="event">The event</param>
        /// <returns>True if the event was consumed</returns>
        protected virtual bool ProcessEventFallback(ITestBuilder typeTestBuilder, IEventInfo @event)
        {
            return false;
        }

        /// <summary>
        /// Processes a method.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="method">The method</param>
        /// <returns>True if the method was consumed</returns>
        protected virtual bool ProcessMethod(ITestBuilder typeTestBuilder, IMethodInfo method)
        {
            return PatternUtils.ConsumeWithFallback(typeTestBuilder, method, ProcessMethodFallback);
        }

        /// <summary>
        /// Processes a method using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="method">The method</param>
        /// <returns>True if the method was consumed</returns>
        protected virtual bool ProcessMethodFallback(ITestBuilder typeTestBuilder, IMethodInfo method)
        {
            return false;
        }

        private sealed class DefaultImpl : TestTypePatternAttribute
        {
        }
    }
}