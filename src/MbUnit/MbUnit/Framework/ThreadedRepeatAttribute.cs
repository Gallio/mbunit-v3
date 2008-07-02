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
using Gallio.Concurrency;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// This attribute decorates a test method and causes it to be invoked repeatedly
    /// on multiple concurrent threads.
    /// </para>
    /// </summary>
    /// <seealso cref="RepeatAttribute"/>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class ThreadedRepeatAttribute : TestDecoratorPatternAttribute
    {
        private readonly int numThreads;

        /// <summary>
        /// Executes the test method on the specified number of concurrent threads.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <code>
        /// [Test]
        /// [ThreadedRepeat(10)]
        /// public void Test()
        /// {
        ///     // This test will be executed 10 times on 10 concurrent threads.
        /// }
        /// </code>
        /// </para>
        /// </remarks>
        /// <param name="numThreads">The number of threads to execute the test on</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="numThreads"/>
        /// is less than 1</exception>
        public ThreadedRepeatAttribute(int numThreads)
        {
            if (numThreads < 1)
                throw new ArgumentOutOfRangeException("numThreads", "The number of concurrent threads must be at least 1.");

            this.numThreads = numThreads;
        }

        /// <inheritdoc />
        protected override void DecorateTest(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            scope.Test.TestInstanceActions.ExecuteTestInstanceChain.Around(delegate(PatternTestInstanceState state, Action<PatternTestInstanceState> inner)
            {
                TaskContainer container = new TaskContainer();
                try
                {
                    TestOutcome[] threadOutcomes = new TestOutcome[numThreads];
                    TestContext context = TestContext.CurrentContext;

                    for (int i = 0; i < numThreads; i++)
                    {
                        int index = i;

                        string name = String.Format("Threaded Repetition #{0}", index + 1);
                        ThreadTask task = new ThreadTask(name, delegate
                        {
                            TestContext threadContext = Step.RunStep(name, delegate
                            {
                                inner(state);
                            });

                            threadOutcomes[index] = threadContext.Outcome;
                        });

                        task.Terminated += delegate
                        {
                            Exception ex = task.Result.Exception;
                            if (ex != null)
                            {
                                threadOutcomes[index] = TestOutcome.Error;
                                context.LogWriter.Default.WriteException(ex,
                                    String.Format("An exception occurred while starting Threaded Repetition #{0}.",
                                        index));
                            }
                        };

                        container.Watch(task);
                        task.Start();
                    }

                    container.JoinAll(null);

                    TestOutcome outcome = TestOutcome.Passed;
                    int passedCount = 0;
                    foreach (TestOutcome threadOutcome in threadOutcomes)
                    {
                        outcome = outcome.CombineWith(threadOutcome);
                        if (threadOutcome.Status == TestStatus.Passed)
                            passedCount += 1;
                    }

                    context.LogWriter.Default.WriteLine(String.Format("{0} of {1} threaded repetitions passed.",
                        passedCount, numThreads));

                    if (outcome.Status != TestStatus.Passed)
                        throw new SilentTestException(outcome);
                }
                finally
                {
                    container.AbortAll();
                    container.JoinAll(null);
                }
            });
        }
    }
}
