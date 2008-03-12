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
using Gallio.Collections;
using Gallio.Framework.Data.Binders;
using Gallio.Framework.Data.Conversions;
using Gallio.Framework.Data.Formatters;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Represents the run-time state of a <see cref="PatternTest" /> that is to be executed.
    /// </para>
    /// <para>
    /// Typical lifecycle of <see cref="PatternTestState" />:
    /// <list type="bullet">
    /// <item>The <see cref="PatternTestController" /> creates a <see cref="PatternTestState" /> for the
    /// <see cref="PatternTest" /> to be executed.</item>
    /// <item>The controller populates the test state with slot binding accessors for each <see cref="PatternTestParameter" />
    /// associated with the test.</item>
    /// <item>The controller calls <see cref="IPatternTestHandler.BeforeTest" /> to give test extensions
    /// the opportunity to modify the test state.</item>
    /// <item>The controller begins iterating over the <see cref="DataBindingItem"/>s produced by the
    /// state's <see cref="BindingContext" />.  For each item it constructs a <see cref="PatternTestInstance" />
    /// and executes the test instance.
    /// <seealso cref="PatternTestInstanceState"/> for further details.</item>
    /// <item>The controller calls <see cref="IPatternTestHandler.AfterTest" /> to give test extensions
    /// the opportunity to clean up the test state.</item>
    /// </list>
    /// </para>
    /// </summary>
    public class PatternTestState
    {
        private readonly PatternTest test;
        private readonly IPatternTestHandler testHandler;
        private readonly IConverter converter;
        private readonly IFormatter formatter;
        private readonly bool isExplicit;

        private readonly DataBindingContext bindingContext;
        private readonly Dictionary<ISlotInfo, IDataBindingAccessor> slotBindingAccessors;
        private readonly UserDataCollection data;

        /// <summary>
        /// Creates an initial test state object.
        /// </summary>
        /// <param name="test">The test</param>
        /// <param name="testHandler">The handler for the test</param>
        /// <param name="converter">The converter for data binding</param>
        /// <param name="formatter">The formatter for data binding</param>
        /// <param name="isExplicit">True if the test was selected explicitly</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/>,
        /// <paramref name="testHandler"/>, <paramref name="converter"/>
        /// or <paramref name="formatter"/> is null</exception>
        public PatternTestState(PatternTest test, IPatternTestHandler testHandler, IConverter converter, IFormatter formatter, bool isExplicit)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (testHandler == null)
                throw new ArgumentNullException("testHandler");
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            this.test = test;
            this.testHandler = testHandler;
            this.converter = converter;
            this.formatter = formatter;
            this.isExplicit = isExplicit;

            bindingContext = new DataBindingContext(converter);
            slotBindingAccessors = new Dictionary<ISlotInfo, IDataBindingAccessor>();
            data = new UserDataCollection();
        }

        /// <summary>
        /// Gets the test associated with this test state.
        /// </summary>
        public PatternTest Test
        {
            get { return test; }
        }

        /// <summary>
        /// Gets the handler for the test.
        /// </summary>
        public IPatternTestHandler TestHandler
        {
            get { return testHandler; }
        }

        /// <summary>
        /// Gets the data binding context of the test.
        /// The context is used to produce data items for test instances.
        /// </summary>
        public DataBindingContext BindingContext
        {
            get { return bindingContext; }
        }

        /// <summary>
        /// Gets the converter for data binding.
        /// </summary>
        public IConverter Converter
        {
            get { return converter; }
        }

        /// <summary>
        /// Gets the formatter for data binding.
        /// </summary>
        public IFormatter Formatter
        {
            get { return formatter; }
        }

        /// <summary>
        /// Returns true if the test was selected explicitly.
        /// </summary>
        public bool IsExplicit
        {
            get { return isExplicit; }
        }

        /// <summary>
        /// Gets the user data collection associated with the test state.  It may be used
        /// to associate arbitrary key/value pairs with the execution of the test.
        /// </summary>
        public UserDataCollection Data
        {
            get { return data; }
        }

        /// <summary>
        /// <para>
        /// Gets a mutable dictionary of slots and their binding accessors.
        /// </para>
        /// <para>
        /// The dictionary maps slots to accessors that will provide values for those
        /// slots when building test instances.  The accessor will be applied to
        /// <see cref="DataBindingItem"/>s produced iteratively by the
        /// <see cref="BindingContext" /> of this test state.
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
        /// A <see cref="IDataBindingAccessor" /> must not be null.
        /// </para>
        /// </remarks>
        public IDictionary<ISlotInfo, IDataBindingAccessor> SlotBindingAccessors
        {
            get { return slotBindingAccessors; }
        }
    }
}
