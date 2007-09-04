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
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A step is a delimited region of a test.  Each step appears in the report as
    /// if it were a dynamically generated test nested within the body of the test
    /// (or some other step) that spawned it.  The step has its own execution log,
    /// pass/fail/inconclusive result and in all other respects behaves much like
    /// an ordinary test would.
    /// </summary>
    public interface IStep
    {
        /// <summary>
        /// Gets the unique identifier of the step.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the name of the step.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// <para>
        /// Gets the full name of the step.  The full name is derived from the name
        /// of the test that contains the step and from all of the parent steps.
        /// </para>
        /// <para>
        /// The full name of the root step is simply the name of the test itself.
        /// The name of a child step is the full name of the parent step followed by a
        /// colon if the parent step was the root or a slash otherwise and finally followed
        /// by the name of the step itself.
        /// </para>
        /// <para>
        /// Examples:
        /// <list type="bullet">
        /// <item><term>SomeTest</term><description>The root step of SomeTest</description></item>
        /// <item><term>SomeTest:ChildStep</term><description>A child step of the root step of SomeTest</description></item>
        /// <item><term>SomeTest:ChildStep/GrandchildStep/BabyStep</term><description>A deeply nested descendent step</description></item>
        /// </list>
        /// </para>
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the step that contains this one, or null if this instance represents the
        /// root step of a test.
        /// </summary>
        IStep Parent { get; }

        /// <summary>
        /// Gets the test that contains this step.
        /// </summary>
        ITest Test { get; }
    }
}
