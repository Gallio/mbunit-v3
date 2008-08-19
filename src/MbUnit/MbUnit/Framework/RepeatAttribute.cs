// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// This attribute decorates a test method and causes it to be invoked repeatedly.
    /// </para>
    /// </summary>
    /// <seealso cref="ThreadedRepeatAttribute"/>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class RepeatAttribute : TestDecoratorPatternAttribute
    {
        private readonly int numRepetitions;

        /// <summary>
        /// Executes the test method repeatedly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <code>
        /// [Test]
        /// [Repeat(10)]
        /// public void Test()
        /// {
        ///     // This test will be executed 10 times.
        /// }
        /// </code>
        /// </para>
        /// </remarks>
        /// <param name="numRepetitions">The number of times to repeat the test</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="numRepetitions"/>
        /// is less than 1</exception>
        public RepeatAttribute(int numRepetitions)
        {
            if (numRepetitions < 1)
                throw new ArgumentOutOfRangeException("numRepetitions", "The number of repetitions must be at least 1.");

            this.numRepetitions = numRepetitions;
        }

        /// <inheritdoc />
        protected override void DecorateTest(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            scope.Test.TestInstanceActions.ExecuteTestInstanceChain.Around(delegate(PatternTestInstanceState state, Action<PatternTestInstanceState> inner)
            {
                TestOutcome outcome = TestOutcome.Passed;
                int passedCount = 0;

                for (int i = 0; i < numRepetitions; i++)
                {
                    string name = String.Format("Repetition #{0}", i + 1);
                    TestContext context = TestStep.RunStep(name, delegate
                    {
                        inner(state);
                    });

                    outcome = outcome.CombineWith(context.Outcome);
                    if (context.Outcome.Status == TestStatus.Passed)
                        passedCount += 1;
                }

                TestLog.WriteLine(String.Format("{0} of {1} repetitions passed.",
                    passedCount, numRepetitions));

                if (outcome.Status != TestStatus.Passed)
                    throw new SilentTestException(outcome);
            });
        }
    }
}
