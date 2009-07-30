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
using Gallio.Common.Collections;
using Gallio.Framework.Data;
using Gallio.Runtime.Conversions;
using Gallio.Runtime.Formatting;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Represents the run-time state of a <see cref="PatternTest" /> that is to be executed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typical lifecycle of <see cref="PatternTestState" />:
    /// <list type="bullet">
    /// <item>The <see cref="PatternTestController" /> creates a <see cref="PatternTestState" /> for the
    /// <see cref="PatternTest" /> to be executed.</item>
    /// <item>The controller populates the test state with slot binding accessors for each <see cref="PatternTestParameter" />
    /// associated with the test.</item>
    /// <item>The controller calls <see cref="PatternTestActions.BeforeTestChain" /> to give test extensions
    /// the opportunity to modify the test state.</item>
    /// <item>The controller begins iterating over the <see cref="IDataItem"/>s produced by the
    /// state's <see cref="BindingContext" />.  For each item it constructs a <see cref="PatternTestInstanceState" />
    /// and executes the test instance.</item>
    /// <item>The controller calls <see cref="PatternTestActions.AfterTestChain" /> to give test extensions
    /// the opportunity to clean up the test state.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class PatternTestState
    {
        private static readonly Key<PatternTestState> ContextKey = new Key<PatternTestState>("Gallio.PatternTestState");

        private readonly PatternTestStep primaryTestStep;
        private readonly PatternTestActions testActions;
        private readonly IConverter converter;
        private readonly IFormatter formatter;
        private readonly bool isExplicit;

        private readonly DataBindingContext bindingContext;
        private readonly Dictionary<PatternTestParameter, IDataAccessor> testParameterDataAccessors;
        private readonly UserDataCollection data;

        /// <summary>
        /// Creates an initial test state object.
        /// </summary>
        /// <param name="primaryTestStep">The primary test step.</param>
        /// <param name="testActions">The test actions.</param>
        /// <param name="converter">The converter for data binding.</param>
        /// <param name="formatter">The formatter for data binding.</param>
        /// <param name="isExplicit">True if the test was selected explicitly.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="primaryTestStep"/>,
        /// <paramref name="testActions"/>, <paramref name="converter"/>
        /// or <paramref name="formatter"/> is null.</exception>
        internal PatternTestState(PatternTestStep primaryTestStep,
            PatternTestActions testActions, IConverter converter, IFormatter formatter, bool isExplicit)
        {
            if (primaryTestStep == null)
                throw new ArgumentNullException("primaryTestStep");
            if (testActions == null)
                throw new ArgumentNullException("testActions");
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            this.primaryTestStep = primaryTestStep;
            this.testActions = testActions;
            this.converter = converter;
            this.formatter = formatter;
            this.isExplicit = isExplicit;

            bindingContext = new DataBindingContext(converter);
            testParameterDataAccessors = new Dictionary<PatternTestParameter, IDataAccessor>();
            data = new UserDataCollection();
        }

        /// <summary>
        /// Gets the pattern test state from the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The pattern test state, or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null.</exception>
        public static PatternTestState FromContext(TestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            while (!context.Data.HasValue(ContextKey) && context.Parent != null)
                context = context.Parent;

            return context.Data.GetValueOrDefault(ContextKey, null);
        }

        internal void SetInContext(TestContext context)
        {
            context.Data.SetValue(ContextKey, this);
        }

        /// <summary>
        /// Gets the test associated with this test state.
        /// </summary>
        public PatternTest Test
        {
            get { return primaryTestStep.Test; }
        }

        /// <summary>
        /// Gets the primary test step associated with this test state.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the test has data bindings, the test instance for each data item will
        /// be executed as children of the primary test step.  Otherwise,
        /// the primary test step will be used for the entire test run.
        /// </para>
        /// </remarks>
        public PatternTestStep PrimaryTestStep
        {
            get { return primaryTestStep; }
        }

        /// <summary>
        /// Gets the test actions.
        /// </summary>
        public PatternTestActions TestActions
        {
            get { return testActions; }
        }

        /// <summary>
        /// Gets the data binding context of the test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The context is used to produce data items for test instances.
        /// </para>
        /// </remarks>
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
        /// Gets the user data collection associated with the test state.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// It may be used to associate arbitrary key/value pairs with the execution of the test.
        /// </para>
        /// </remarks>
        public UserDataCollection Data
        {
            get { return data; }
        }

        /// <summary>
        /// Gets a mutable dictionary of data accessors that will provide
        /// values assigned to test parameters given a data binding item.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The contents of the dictionary are initialized by the framework as part of the
        /// test parameter binding phase for the test instance, just before the "before test"
        /// actions run.
        /// </para>
        /// </remarks>
        public IDictionary<PatternTestParameter, IDataAccessor> TestParameterDataAccessors
        {
            get { return testParameterDataAccessors; }
        }
    }
}
