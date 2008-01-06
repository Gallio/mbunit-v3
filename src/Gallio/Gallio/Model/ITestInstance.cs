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
        /// Returns true if the test instance is dynamic and cannot not be known with certainty
        /// prior to test execution because its parameters are bound to values that
        /// may be unavailable ahead of time, may change over time or that may be
        /// expensive to obtain.
        /// </summary>
        bool IsDynamic { get; }

        /// <summary>
        /// Gets the value associated with the specified test parameter, if available.
        /// </summary>
        /// <param name="parameter">The test parameter</param>
        /// <returns>The value associated with the test parameter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="parameter"/> does not
        /// belong to this test</exception>
        /// <exception cref="InvalidOperationException">Thrown if the parameter value is not
        /// available (perhaps because the test is not currently running)</exception>
        object GetParameterValue(ITestParameter parameter);
    }
}
