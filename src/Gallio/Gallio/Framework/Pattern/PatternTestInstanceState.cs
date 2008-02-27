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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Framework.Data;
using Gallio.Framework.Data.Binders;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Represents the run-time state of a <see cref="PatternTestInstance" />
    /// that is to be executed.
    /// </para>
    /// <para>
    /// Typical lifecycle of <see cref="PatternTestInstanceState" />:
    /// <list type="bullet">
    /// <item>The <see cref="PatternTestController" /> creates a <see cref="PatternTestInstanceState" /> for the
    /// <see cref="PatternTestInstance" /> to be executed.</item>
    /// <item>The controller populates the instance state with slot values for each slot with
    /// an associated <see cref="IDataBindingAccessor" /> in the <see cref="PatternTestState" />.</item>
    /// <item>The controller calls <see cref="IPatternTestHandler.BeforeTestInstance" /> to give test extensions
    /// the opportunity to modify the instance state.</item>
    /// <item>The controller initializes, sets up, executes, tears down and disposes the test instance.</item>
    /// <item>The controller calls <see cref="IPatternTestHandler.AfterTestInstance" /> to give test extensions
    /// the opportunity to clean up the instance state.</item>
    /// </list>
    /// </para>
    /// </summary>
    public class PatternTestInstanceState
    {
        private readonly PatternTestInstance testInstance;
        private readonly PatternTestState testState;
        private readonly DataBindingItem bindingItem;
        private readonly Dictionary<ISlotInfo, object> slotValues;
        private readonly UserDataCollection data;

        private Type fixtureType;
        private object fixtureInstance;

        /// <summary>
        /// Creates an initial test instance state object.
        /// </summary>
        /// <param name="testInstance">The test instance</param>
        /// <param name="testState">The test state</param>
        /// <param name="bindingItem">The data binding item</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testInstance"/>,
        /// <paramref name="testState"/> or <paramref name="bindingItem"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="testState"/> belongs to a
        /// different test from the <paramref name="testInstance"/></exception>
        public PatternTestInstanceState(PatternTestInstance testInstance, PatternTestState testState, DataBindingItem bindingItem)
        {
            if (testInstance == null)
                throw new ArgumentNullException("testInstance");
            if (testState == null)
                throw new ArgumentNullException("testState");
            if (testInstance.Test != testState.Test)
                throw new ArgumentException("The test state belongs to a different test from the test instance.", "testState");
            if (bindingItem == null)
                throw new ArgumentNullException("bindingItem");

            this.testInstance = testInstance;
            this.testState = testState;
            this.bindingItem = bindingItem;

            slotValues = new Dictionary<ISlotInfo, object>();
            data = new UserDataCollection();
        }

        /// <summary>
        /// Gets the test instance associated with this test instance state.
        /// </summary>
        public PatternTestInstance TestInstance
        {
            get { return testInstance; }
        }

        /// <summary>
        /// Gets the test associated with this test instance state.
        /// </summary>
        public PatternTest Test
        {
            get { return testState.Test; }
        }

        /// <summary>
        /// Gets the handler for the test.
        /// </summary>
        public IPatternTestHandler TestHandler
        {
            get { return testState.TestHandler; }
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
        /// is a generic type definition, a generic parameter, has an element type,
        /// or contains generic parameters</exception>
        public Type FixtureType
        {
            get { return fixtureType; }
            set
            {
                if (value != null)
                {
                    if (value.HasElementType || value.ContainsGenericParameters || value.IsGenericParameter)
                        throw new ArgumentException("The fixture type must not be an array, pointer, reference, generic parameter, or open generic type.", "value");
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
        /// <para>
        /// Formats the <see cref="SlotValues" /> to a string for presentation.
        /// </para>
        /// </summary>
        /// <returns>The formatted slots</returns>
        /// <seealso cref="SlotBinder.FormatSlotValues"/> for details about the algorithm.
        public string FormatSlotValues()
        {
            return SlotBinder.FormatSlotValues(slotValues, testState.Formatter);
        }

        /// <summary>
        /// Makes a fixture type using the state's bound <see cref="SlotValues"/> using
        /// its <see cref="IGenericParameterInfo"/> slots, if any.
        /// </summary>
        /// <param name="type">The fixture type or generic type definition</param>
        /// <returns>The fixture type or generic type instantiation</returns>
        /// <remarks>
        /// The values of <see cref="FixtureType" /> and <see cref="FixtureInstance" /> are not used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// are not appropriate for creating a generic type instantiation of <paramref name="type"/></exception>
        /// <seealso cref="SlotBinder.MakeType"/>
        public Type MakeFixtureType(ITypeInfo type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return SlotBinder.MakeType(type, slotValues);
        }

        /// <summary>
        /// Creates a fixture instance using the state's bound <see cref="SlotValues"/>.
        /// </summary>
        /// <param name="type">The fixture type or generic type definition</param>
        /// <returns>The fixture instance</returns>
        /// <remarks>
        /// The values of <see cref="FixtureType" /> and <see cref="FixtureInstance" /> are not used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values are not appropriate for instantiating <paramref name="type"/></exception>
        /// <seealso cref="SlotBinder.CreateInstance(ITypeInfo, IEnumerable{KeyValuePair{ISlotInfo, object}})"/>
        public object CreateFixtureInstance(ITypeInfo type)
        {
            return CreateFixtureInstanceWithSlotValues(type, slotValues);
        }

        /// <summary>
        /// Creates a fixture instance using the specified slot values.
        /// </summary>
        /// <param name="type">The fixture type or generic type definition</param>
        /// <param name="slotValues">The slot values to use for instantiating the fixture</param>
        /// <returns>The fixture instance</returns>
        /// <remarks>
        /// The values of <see cref="FixtureType" /> and <see cref="FixtureInstance" /> are not used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values are not appropriate for instantiating <paramref name="type"/></exception>
        /// <seealso cref="SlotBinder.CreateInstance(ITypeInfo, IEnumerable{KeyValuePair{ISlotInfo, object}})"/>
        public object CreateFixtureInstanceWithSlotValues(ITypeInfo type, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return SlotBinder.CreateInstance(type, slotValues);
        }

        /// <summary>
        /// <para>
        /// Invokes a fixture method using the state's bound <see cref="SlotValues"/>.
        /// </para>
        /// <para>
        /// If the method is static, uses the state's <see cref="FixtureType" /> to determine
        /// the appropriate generic type instantiation on which to invoke a static method
        /// declared by a generic type definition.
        /// </para>
        /// <para>
        /// If the method is non-static, uses the state's <see cref="FixtureInstance" /> as
        /// the instance on which to invoke the method.
        /// </para>
        /// </summary>
        /// <param name="method">The fixture method or generic method definition,
        /// possibly declared by a generic type or generic type defintion</param>
        /// <returns>The method return value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// and <see cref="FixtureInstance" /> are not appropriate for invoking <paramref name="method"/></exception>
        /// <exception cref="TargetInvocationException">Thrown if the method itself throws an exception</exception>
        /// <seealso cref="SlotBinder.InvokeInstanceMethod"/> and <seealso cref="SlotBinder.InvokeStaticMethod"/>
        public object InvokeFixtureMethodWithSlotValues(IMethodInfo method)
        {
            return InvokeFixtureMethod(method, slotValues);
        }

        /// <summary>
        /// <para>
        /// Invokes a fixture method using the specified <paramref name="slotValues"/>.
        /// </para>
        /// <para>
        /// If the method is static, uses the state's <see cref="FixtureType" /> to determine
        /// the appropriate generic type instantiation on which to invoke a static method
        /// declared by a generic type definition.
        /// </para>
        /// <para>
        /// If the method is non-static, uses the state's <see cref="FixtureInstance" /> as
        /// the instance on which to invoke the method.
        /// </para>
        /// </summary>
        /// <param name="method">The fixture method or generic method definition,
        /// possibly declared by a generic type or generic type defintion</param>
        /// <param name="slotValues">The slot values to use for invoking the method</param>
        /// <returns>The method return value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// and <see cref="FixtureInstance" /> are not appropriate for invoking <paramref name="method"/></exception>
        /// <exception cref="TargetInvocationException">Thrown if the method itself throws an exception</exception>
        /// <seealso cref="SlotBinder.InvokeInstanceMethod"/> and <seealso cref="SlotBinder.InvokeStaticMethod"/>
        public object InvokeFixtureMethod(IMethodInfo method, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (method.IsStatic)
                return SlotBinder.InvokeStaticMethod(method, fixtureType, slotValues);

            return SlotBinder.InvokeInstanceMethod(method, fixtureInstance, slotValues);
        }
    }
}