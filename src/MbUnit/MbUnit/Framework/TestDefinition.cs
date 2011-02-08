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
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Abstract class for custom test definitions such as generated test cases
    /// and test suites.
    /// </summary>
    public abstract class TestDefinition : Test
    {
        private readonly string name;
        private readonly PropertyBag metadata = new PropertyBag();

        private TimeSpan? timeout;

        /// <summary>
        /// Creates a test.
        /// </summary>
        /// <param name="name">The test name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        protected TestDefinition(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the test name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets the description metadata of the test, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that accesses the <see cref="MetadataKeys.Description" />
        /// element of the test case's <see cref="Metadata" />.
        /// </para>
        /// </remarks>
        public string Description
        {
            get { return metadata.GetValue(MetadataKeys.Description); }
            set { metadata.SetValue(MetadataKeys.Description, value); }
        }

        /// <summary>
        /// Gets a mutable table of metadata entries associated with the test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Add any extra metadata to this map.
        /// </para>
        /// </remarks>
        /// <seealso cref="MetadataKeys"/>
        public PropertyBag Metadata
        {
            get { return metadata; }
        }

        /// <summary>
        /// Gets or sets the maximum amount of time the whole test including
        /// its setup, teardown and body should be permitted to run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the test runs any longer than this, it will be aborted by the framework.
        /// The timeout may be null to indicate the absence of a timeout.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/>
        /// represents a negative time span.</exception>
        /// <value>The timeout.  Default value is null.</value>
        public TimeSpan? Timeout
        {
            get { return timeout; }
            set
            {
                if (value.HasValue && value.Value.Ticks < 0)
                    throw new ArgumentOutOfRangeException(@"value");
                timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the code element associated with the test, or null if none.
        /// </summary>
        public ICodeElementInfo CodeElement
        {
            get;
            set;
        }

        /// <inheritdoc />
        protected override void BuildStaticTest(IPatternScope containingScope, ICodeElementInfo declaringCodeElement)
        {
            IPatternScope childTestScope = containingScope.CreateChildTestScope(Name, CodeElement ?? declaringCodeElement);
            childTestScope.TestBuilder.Kind = Kind;
            childTestScope.TestBuilder.TimeoutFunc = () => Timeout;
            childTestScope.TestBuilder.IsTestCase = IsTestCase;

            foreach (var pair in Metadata.Pairs)
                childTestScope.TestBuilder.AddMetadata(pair.Key, pair.Value);

            childTestScope.TestBuilder.TestInstanceActions.SetUpTestInstanceChain.Before(state => OnSetupSelf());
            childTestScope.TestBuilder.TestInstanceActions.ExecuteTestInstanceChain.After(state => OnExecuteSelf());
            childTestScope.TestBuilder.TestInstanceActions.TearDownTestInstanceChain.After(state => OnTearDownSelf());

            childTestScope.TestBuilder.TestInstanceActions.DecorateChildTestChain.After((outerState, decoratedChildActions) =>
                decoratedChildActions.TestInstanceActions.SetUpTestInstanceChain.Before(innerState => OnSetupChild()));
            childTestScope.TestBuilder.TestInstanceActions.DecorateChildTestChain.After((outerState, decoratedChildActions) =>
                decoratedChildActions.TestInstanceActions.TearDownTestInstanceChain.After(innerState => OnTearDownChild()));

            BuildStaticTests(GetChildren(), childTestScope, declaringCodeElement);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        protected override TestOutcome RunDynamicTest(ICodeElementInfo declaringCodeElement, Action setUp, Action tearDown)
        {
            return TestStep.RunStep(Name, () =>
            {
                TestStep.AddMetadata(MetadataKeys.TestKind, Kind);
                foreach (var pair in Metadata.Pairs)
                    TestStep.AddMetadata(pair.Key, pair.Value);

                try
                {
                    TestContext.CurrentContext.LifecyclePhase = LifecyclePhases.SetUp;
                    if (setUp != null)
                        setUp();
                    try
                    {
                        OnSetupSelf();

                        TestContext.CurrentContext.LifecyclePhase = LifecyclePhases.Execute;
                        OnExecuteSelf();

                        TestOutcome outcome = RunDynamicTests(GetChildren(), declaringCodeElement, OnSetupChild, OnTearDownChild);
                        if (outcome != TestOutcome.Passed)
                            throw new SilentTestException(outcome);
                    }
                    finally
                    {
                        TestContext.CurrentContext.LifecyclePhase = LifecyclePhases.TearDown;
                        OnTearDownSelf();
                    }
                }
                finally
                {
                    TestContext.CurrentContext.LifecyclePhase = LifecyclePhases.TearDown;
                    if (tearDown != null)
                        tearDown();
                }
            }, Timeout, IsTestCase, CodeElement ?? declaringCodeElement).Outcome;
        }

        /// <summary>
        /// Gets the test kind.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A test suite will return <see cref="TestKinds.Suite" />.
        /// A test case will return <see cref="TestKinds.Test" />.
        /// Custom tests may return other kinds.
        /// </para>
        /// <para>
        /// Subclasses must override this behavior.
        /// </para>
        /// </remarks>
        /// <seealso cref="TestKinds"/>
        protected abstract string Kind { get; }

        /// <summary>
        /// Returns true if the test represents an individual test case.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A test suite will return <c>true</c>.
        /// A test case will return <c>false</c>.
        /// Custom tests may return an appropriate result.
        /// </para>
        /// <para>
        /// Subclasses must override this behavior.
        /// </para>
        /// </remarks>
        /// <seealso cref="TestKinds"/>
        protected abstract bool IsTestCase { get; }

        /// <summary>
        /// Gets the children of this test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns an empty array.  Subclasses may
        /// override this behavior as appropriate.
        /// </para>
        /// </remarks>
        protected virtual IEnumerable<Test> GetChildren()
        {
            return EmptyArray<Test>.Instance;
        }

        /// <summary>
        /// Runs set-up code before execute and before all children.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override this
        /// behavior.
        /// </para>
        /// </remarks>
        protected virtual void OnSetupSelf()
        {
        }

        /// <summary>
        /// Runs tear-down code after execute and after all children.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override this
        /// behavior.
        /// </para>
        /// </remarks>
        protected virtual void OnTearDownSelf()
        {
        }

        /// <summary>
        /// Executes the main body of the test, excluding its children (which will
        /// run afterwards).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override this
        /// behavior.
        /// </para>
        /// </remarks>
        protected virtual void OnExecuteSelf()
        {
        }

        /// <summary>
        /// Runs set-up code before each child.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override this
        /// behavior.
        /// </para>
        /// </remarks>
        protected virtual void OnSetupChild()
        {
        }

        /// <summary>
        /// Runs tear-down code after each child.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override this
        /// behavior.
        /// </para>
        /// </remarks>
        protected virtual void OnTearDownChild()
        {
        }
    }
}
