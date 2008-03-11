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

namespace Gallio.Model
{
    /// <summary>
    /// A test instance is a <see cref="ITest" /> with bound values.  We create
    /// an instance of a test before running it.  The same instance may be run
    /// repeatedly, or it might not be run at all.
    /// </summary>
    public interface ITestInstance : ITestComponent
    {
        /// <summary>
        /// Gets the test from which this instance was created.
        /// </summary>
        ITest Test { get; }

        /// <summary>
        /// Gets the parent of this test instance, or null if this is
        /// the root test instance.
        /// </summary>
        ITestInstance Parent { get; }

        /// <summary>
        /// <para>
        /// Gets the full name of the test instance.  The full name is derived by concatenating the
        /// <see cref="FullName" /> of the <see cref="Parent"/> followed by a slash ('/')
        /// followed by the <see cref="ITestComponent.Name" /> of this test instance.
        /// </para>
        /// <para>
        /// The full name of the root test instance is empty.
        /// </para>
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Returns true if the test instance is dynamic and cannot not be known with certainty
        /// prior to test execution because its parameters are bound to values that
        /// may be unavailable ahead of time, may change over time or that may be
        /// expensive to obtain.
        /// </summary>
        bool IsDynamic { get; }
    }
}
