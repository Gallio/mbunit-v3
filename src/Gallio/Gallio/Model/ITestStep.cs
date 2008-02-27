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
using System.Text;

namespace Gallio.Model
{
    /// <summary>
    /// <para>
    /// A test step is a delimited region of a test defined at run-time.
    /// Each test that is executed consists of at least one step: the "root" step.
    /// During execution, each test may spawn additional nested steps that may run in
    /// parallel or in series with one another to delimit portions of its execution.
    /// </para>
    /// <para>
    /// Each step has its own execution log and test result (pass/fail/inconclusive).
    /// Therefore a multi-step test may possess multiple execution logs and test results.
    /// This is deliberate.  Think of a <see cref="ITest" /> as being the declarative component
    /// of a test that specifies which test method to invoke and its arguments.  An
    /// <see cref="ITestStep" /> is the runtime counterpart of the <see cref="ITest" /> that
    /// captures output and control flow information about part or all of the test.
    /// </para>
    /// <para>
    /// A test step also has metadata that can be update at run-time to carry additional
    /// declarative information about the step.
    /// </para>
    /// </summary>
    public interface ITestStep : ITestComponent
    {
        /// <summary>
        /// <para>
        /// Gets the full name of the step.  The full name is derived from the full name
        /// of the test instance that contains the step and from all of the parent steps.
        /// </para>
        /// <para>
        /// The full name of the root step is simply the full name of the test instance itself.
        /// The name of a child step is the full name of the parent step followed by a
        /// colon if the parent step was the root or a slash otherwise and finally followed
        /// by the name of the step itself.
        /// </para>
        /// <para>
        /// Examples:
        /// <list type="bullet">
        /// <item><term>SomeAssembly.SomeFixture.SomeTest</term><description>The root step of SomeTest</description></item>
        /// <item><term>SomeAssembly.SomeFixture.SomeTest:ChildStep</term><description>A child step of the root step of SomeTest</description></item>
        /// <item><term>SomeAssembly.SomeFixture.SomeTest:ChildStep/GrandchildStep/BabyStep</term><description>A deeply nested descendent step</description></item>
        /// </list>
        /// </para>
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the step that contains this one, or null if this instance represents the
        /// root step of a test.
        /// </summary>
        ITestStep Parent { get; }

        /// <summary>
        /// Gets the test instance that contains this step.
        /// </summary>
        ITestInstance TestInstance { get; }
    }
}
