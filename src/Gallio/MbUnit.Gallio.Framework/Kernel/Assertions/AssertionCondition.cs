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
using MbUnit.Framework.Kernel.Assertions;

namespace MbUnit.Framework.Kernel.Assertions
{
    /// <summary>
    /// A delegate used to evaluate an assertion condition.
    /// Returns the assertion result.  If an exception occurs, the framework
    /// will interpret it as a fatal assertion failure and generate a suitable
    /// result object containing the exception.  Even though it is tolerated,
    /// an assertion condition should generally not fail with an exception.
    /// </summary>
    /// <param name="assertion">The assertion being checked.</param>
    /// <returns>The result of having evaluated the assertion condition, never null.
    /// Upon return, the framework will automatically set the <see cref="AssertionResult.Assertion" />
    /// property of the result.</returns>
    /// <exception cref="Exception">Any exception thrown is interpreted as an assertion failure.</exception>
    public delegate AssertionResult AssertionCondition(Assertion assertion);
}