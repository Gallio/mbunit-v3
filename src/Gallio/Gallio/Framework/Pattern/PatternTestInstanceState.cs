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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Framework.Data;
using Gallio.Framework.Data.Binders;
using Gallio.Framework.Data.Conversions;
using Gallio.Framework.Data.Formatters;
using Gallio.Reflection;
using Gallio.Utilities;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Represents the run-time state of a single instance of a <see cref="PatternTest" />
    /// that is to be executed.
    /// </para>
    /// <para>
    /// Typical lifecycle of <see cref="PatternTestInstanceState" />:
    /// <list type="bullet">
    /// <item>The <see cref="PatternTestController" /> creates a <see cref="PatternTestInstanceState" /> for the
    /// instance of the <see cref="PatternTest" /> to be executed using particular data bindings.</item>
    /// <item>The controller populates the instance state with slot values for each slot with
    /// an associated <see cref="IDataBindingAccessor" /> in the <see cref="PatternTestState" />.</item>
    /// <item>The controller calls <see cref="IPatternTestInstanceHandler.BeforeTestInstance" /> to give test extensions
    /// the opportunity to modify the instance state.</item>
    /// <item>The controller initializes, sets up, executes, tears down and disposes the test instance.</item>
    /// <item>The controller calls <see cref="IPatternTestInstanceHandler.AfterTestInstance" /> to give test extensions
    /// the opportunity to clean up the instance state.</item>
    /// </list>
    /// </para>
    /// </summary>
    public class PatternTestInstanceState
    {
        private readonly PatternTestStep testStep;
        private readonly IPatternTestInstanceHandler testInstanceHandler;
        private readonly PatternTestState testState;
        private readonly DataBindingItem bindingItem;
        private readonly Dictionary<ISlotInfo, object> slotValues;
        private readonly UserDataCollection data;

        private Type fixtureType;
        private object fixtureInstance;
        private MethodInfo testMethod;
        private object[] testArguments;

        /// <summary>
        /// Creates an initial test instance state object.
        /// </summary>
        /// <param name="testStep">The test step used to execute the test instance</param>
        /// <param name="testInstanceHandler">The test instance handler</param>
        /// <param name="testState">The test state</param>
        /// <param name="bindingItem">The data binding item</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testStep"/>,
        /// <paramref name="testInstanceHandler"/> or <paramref name="testState"/> or <paramref name="bindingItem"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="testState"/> belongs to a
        /// different test from the <paramref name="testStep"/></exception>
        public PatternTestInstanceState(PatternTestStep testStep, 
            IPatternTestInstanceHandler testInstanceHandler,
            PatternTestState testState, DataBindingItem bindingItem)
        {
            if (testStep == null)
                throw new ArgumentNullException("testStep");
            if (testInstanceHandler == null)
                throw new ArgumentNullException("testInstanceHandler");
            if (testState == null)
                throw new ArgumentNullException("testState");
            if (testStep.Test != testState.Test)
                throw new ArgumentException("The test state belongs to a different test from the test step.", "testState");
            if (bindingItem == null)
                throw new ArgumentNullException("bindingItem");

            this.testStep = testStep;
            this.testInstanceHandler = testInstanceHandler;
            this.testState = testState;
            this.bindingItem = bindingItem;

            slotValues = new Dictionary<ISlotInfo, object>();
            data = new UserDataCollection();
        }

        /// <summary>
        /// Gets the converter for data binding.
        /// </summary>
        public IConverter Converter
        {
            get { return testState.Converter; }
        }

        /// <summary>
        /// Gets the formatter for data binding.
        /// </summary>
        public IFormatter Formatter
        {
            get { return testState.Formatter; }
        }

        /// <summary>
        /// Gets the test step used to execute the test instance.
        /// </summary>
        public PatternTestStep TestStep
        {
            get { return testStep; }
        }

        /// <summary>
        /// Gets the handler for the test instance.
        /// </summary>
        public IPatternTestInstanceHandler TestInstanceHandler
        {
            get { return testInstanceHandler; }
        }

        /// <summary>
        /// Gets the test associated with this test instance state.
        /// </summary>
        public PatternTest Test
        {
            get { return testState.Test; }
        }

        /// <summary>
        /// Gets the test state associated with this test instance state.
        /// </summary>
        public PatternTestState TestState
        {
            get { return testState; }
        }

        /// <summary>
        /// Gets the data binding item obtained from the test's <see cref="DataBindingContext" />
        /// to create this state.
        /// </summary>
        public DataBindingItem BindingItem
        {
            get { return bindingItem; }
        }

        /// <summary>
        /// Gets the user data collection associated with the test instance state.  It may be used
        /// to associate arbitrary key/value pairs with the execution of the test instance.
        /// </summary>
        public UserDataCollection Data
        {
            get { return data; }
        }

        /// <summary>
        /// Gets or sets the test fixture type or null if none.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        /// contains unbound generic parameters, is a generic parameter, has an element type</exception>
        public Type FixtureType
        {
            get { return fixtureType; }
            set
            {
                if (value != null)
                {
                    if (value.ContainsGenericParameters || value.IsGenericParameter || value.HasElementType)
                        throw new ArgumentException("The fixture type must not be an array, pointer, reference, generic parameter, or contain unbound generic parameters.", "value");
                }

                fixtureType = value;
            }
        }

        /// <summary>
        /// Gets or sets the test fixture instance or null if none.
        /// </summary>
        public object FixtureInstance
        {
            get { return fixtureInstance; }
            set { fixtureInstance = value; }
        }

        /// <summary>
        /// Gets or sets the test method or null if none.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        /// is contains unbound generic parameters</exception>
        public MethodInfo TestMethod
        {
            get { return testMethod; }
            set
            {
                if (value != null)
                {
                    if (value.ContainsGenericParameters)
                        throw new ArgumentException("The test method must not contain unbound generic parameters.", "value");
                }

                testMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the test method arguments or null if none.
        /// </summary>
        public object[] TestArguments
        {
            get { return testArguments; }
            set { testArguments = value; }
        }

        /// <summary>
        /// <para>
        /// Gets a mutable dictionary of slots and their bound values.
        /// </para>
        /// <para>
        /// The dictionary maps slots to the values that will be stored in them
        /// during test execution.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The slots should be valid for the type of test in question.  For example,
        /// a test fixture supports constructor parameters, generic type parameters,
        /// fields and properies declared by the test fixture type.  Likewise a test method
        /// supports method parameters and generic method parameters declared by the test
        /// method.  Other novel kinds of tests might additional capabilities.
        /// </para>
        /// <para>
        /// A value should be of a type that is compatible with the slot's <see cref="ISlotInfo.ValueType" />.
        /// If any type conversion is required, it should already have taken place prior to
        /// adding the value to this dictionary.
        /// </para>
        /// </remarks>
        public IDictionary<ISlotInfo, object> SlotValues
        {
            get { return slotValues; }
        }

        /// <summary>
        /// Returns true if the <see cref="TestStep" /> is the <see cref="PatternTestState.PrimaryTestStep" />
        /// that was created for the test.  False if a new <see cref="PatternTestStep"/> 
        /// was created as a child of the primary test step just for this test instance.
        /// </summary>
        public bool IsReusingPrimaryTestStep
        {
            get { return testStep.IsPrimary; }
        }

        /// <summary>
        /// Gets a fixture object creation specification using the state's bound <see cref="SlotValues"/>.
        /// </summary>
        /// <param name="type">The fixture type or generic type definition</param>
        /// <returns>The fixture instance</returns>
        /// <remarks>
        /// The values of <see cref="FixtureType" /> and <see cref="FixtureInstance" /> are not used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// are not appropriate for instantiating <paramref name="type"/></exception>
        /// <seealso cref="ObjectCreationSpec"/>
        public ObjectCreationSpec GetFixtureObjectCreationSpec(ITypeInfo type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return new ObjectCreationSpec(type, SlotValues, Converter);
        }

        /// <summary>
        /// Gets a test method invocation specification using the state's bound <see cref="SlotValues"/>.
        /// </summary>
        /// <param name="method">The test method or generic method definition,
        /// possibly declared by a generic type or generic type defintion</param>
        /// <returns>The method return value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// or <see cref="FixtureType" /> are not appropriate for invoking <paramref name="method"/></exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="FixtureType" /> is null</exception>
        /// <seealso cref="MethodInvocationSpec"/>
        public MethodInvocationSpec GetTestMethodInvocationSpec(IMethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (fixtureType == null)
                throw new InvalidOperationException("This method cannot be used when FixtureType is null.");

            return new MethodInvocationSpec(fixtureType, method, SlotValues, Converter);
        }

        /// <summary>
        /// <para>
        /// Invokes a fixture method using the specified <paramref name="slotValues"/>.
        /// </para>
        /// </summary>
        /// <param name="method">The fixture method or generic method definition,
        /// possibly declared by a generic type or generic type defintion</param>
        /// <param name="slotValues">The slot values to use for invoking the method</param>
        /// <returns>The method return value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> or <paramref name="slotValues"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// or <see cref="FixtureType" /> or <see cref="FixtureInstance" /> are not appropriate for
        /// invoking <paramref name="method"/></exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="FixtureType" /> is null</exception>
        /// <exception cref="Exception">Any exception thrown by the invoked method</exception>
        /// <seealso cref="MethodInvocationSpec"/>
        public object InvokeFixtureMethod(IMethodInfo method, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");
            if (fixtureType == null)
                throw new InvalidOperationException("This method cannot be used when FixtureType is null.");

            MethodInvocationSpec spec = new MethodInvocationSpec(fixtureType, method, slotValues, Converter);
            return spec.Invoke(fixtureInstance);
        }

        /// <summary>
        /// <para>
        /// Invokes the test method specified by <see cref="TestMethod" />, <see cref="FixtureInstance" />
        /// and <see cref="TestArguments" />.  If there is no test method or no arguments, does nothing.
        /// </para>
        /// </summary>
        /// <returns>The method return value, or null if there was none</returns>
        /// <exception cref="Exception">Any exception thrown by the invoked method</exception>
        public object InvokeTestMethod()
        {
            if (testMethod != null && testArguments != null)
                return ExceptionUtils.InvokeMethodWithoutTargetInvocationException(testMethod, fixtureInstance, testArguments);

            return null;
        }
    }
}