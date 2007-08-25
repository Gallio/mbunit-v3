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
using MbUnit.Framework;
using MbUnit.Framework.Exceptions;
using MbUnit.Framework.Kernel.Assertions;
using MbUnit.Framework.Kernel.Contexts;

namespace MbUnit.Framework.Kernel.Assertions
{
    /// <summary>
    /// The assertion service tracks assertions within a test execution context.
    /// </summary>
    public interface IAssertionService
    {
        /// <summary>
        /// Gets the total number of assertions verified so far in the specified context.
        /// </summary>
        int GetAssertionCount(IContext context);

        /// <summary>
        /// Runs a block of code in a certain context and collects the assertion results
        /// that were passed to <see cref="Verify" /> within the block.
        /// </summary>
        /// <param name="context">The test execution context</param>
        /// <param name="block">The code block to run</param>
        /// <param name="allowMultipleFailures">If true, allows multiple failures to occur
        /// and collects them all.  Otherwise aborts execution of the code block (by throwing
        /// <see cref="AssertionException" /> when an assertion failure occurs and then
        /// later catching that exception).</param>
        /// <returns>The array of assertions that were verified, may be zero or more</returns>
        AssertionResult[] Run(IContext context, Block block, bool allowMultipleFailures);

        /// <summary>
        /// Evaluates an assertion condition and returns its result.
        /// </summary>
        /// <param name="context">The test execution context</param>
        /// <param name="assertion">The description of the assertion to evaluate</param>
        /// <param name="condition">The assertion condition delegate</param>
        /// <returns>The assertion result, never null</returns>
        AssertionResult Evaluate(IContext context, Assertion assertion, AssertionCondition condition);

        /// <summary>
        /// Verifies that the result succeeded.
        /// Adds the result to the list being prepared for the current assertion scope
        /// such as the containing <see cref="Run" /> block.
        /// Throws an <see cref="AssertionException" /> to terminate execution of the
        /// code block if invoked within a scope that does not permit multiple failures.
        /// Automatically increments the assertion count for the context.
        /// </summary>
        /// <param name="context">The test execution context</param>
        /// <param name="result">The assertion result to verify</param>
        void Verify(IContext context, AssertionResult result);

        /* Not implemented yet.
        /// <summary>
        /// Includes zero or more named objects in assertion failure reports.
        /// Takes a snapshot of the current state of the objects and includes them
        /// in the report to help diagnose a particular failure.
        /// </summary>
        /// <example>
        /// <code>
        /// Uri uri = new Uri("foo://bar");
        /// string request = "Quux";
        /// using (Assert.With("uri", uri, "request", request))
        /// {
        ///     Assert.AreEqual("foo://bar", uri.AbsolutePath);
        /// }
        /// </code>
        /// </example>
        /// <param name="context">The test execution context</param>
        /// <param name="objectNamesAndValues">The array of alternating name and value pairs for objects to
        /// include in the report</param>
        /// <returns>An object that when disposed indicates the end of the block</returns>
        IDisposable With(IContext context, params object[] objectNamesAndValues);
         */
    }
}