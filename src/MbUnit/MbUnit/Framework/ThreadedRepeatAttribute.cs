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

#region Using Directives

using System;
using System.Collections.Generic;
using System.Threading;
using Gallio.Framework.Pattern;

#endregion

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>This attribute defines a test method that will be invoked in the specified
    /// number of concurrent threads.
    /// </para>
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class ThreadedRepeatAttribute : TestDecoratorAttribute
    {
        #region Private Members

        private readonly int _count;
        private readonly object _lock = new object();

        #endregion

        #region Construction

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
        /// <param name="count">The number of threads to execute the test on</param>
        public ThreadedRepeatAttribute(int count)
        {
            _count = count;
        }

        #endregion

        #region Overridden and Protected Methods

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <param name="testInstanceState">The test instance state, not null</param>
        /// <seealso cref="IPatternTestInstanceHandler.ExecuteTestInstance"/>
        protected override void Execute(PatternTestInstanceState testInstanceState)
        {
            lock (_lock)
            {
                Exception lastException = null;

                // Launch the test in threads
                List<ThreadedRepeatRunner> children = new List<ThreadedRepeatRunner>(_count);
                for (int i = 0; i < _count; i++)
                {
                    ThreadedRepeatRunner runner = new ThreadedRepeatRunner(this, testInstanceState);
                    children.Add(runner);
                    runner.Start();
                }

                // Wait for our children to finish
                while( children.Count > 0 )
                {
                    Monitor.Wait(_lock);
                    List<ThreadedRepeatRunner> finished = new List<ThreadedRepeatRunner>();

                    // Check to see which children have finished
                    foreach( ThreadedRepeatRunner runner in children )
                    {
                        if( runner.Finished )
                        {
                            finished.Add(runner);
                            if( runner.HasThrown )
                            {
                                lastException = runner.Exception;
                            }
                        }
                    }

                    // Remove finished children from the list
                    foreach (ThreadedRepeatRunner runner in finished)
                    {
                        children.Remove(runner);
                    }
                }

                // If any of our children threw an exception, then rethrow the last exception we found
                if( lastException != null )
                {
                    // TODO: This will change the callstack, how do we rethrow without doing that?
                    throw lastException;
                }
            }
        }

        /// <summary>
        /// Calls the Execute method on the base class. This is so the ThreadedRepeatRunner can call to
        /// the base in its thread.
        /// </summary>
        /// <param name="testInstanceState"></param>
        private void BaseExecute(PatternTestInstanceState testInstanceState)
        {
            base.Execute(testInstanceState);
        } 

        #endregion

        /// <summary>
        /// This class is used to run a test method on a thread.
        /// </summary>
        private class ThreadedRepeatRunner
        {
            #region Private Members

            private readonly ThreadedRepeatAttribute _parent;
            private readonly PatternTestInstanceState _testInstanceState;
            private Exception _exception; 

            #endregion

            #region Construction

            public ThreadedRepeatRunner(ThreadedRepeatAttribute parent, PatternTestInstanceState testInstanceState)
            {
                _parent = parent;
                _testInstanceState = testInstanceState;
            } 

            #endregion

            #region Public Properties

            /// <summary>
            /// Returns true if an exception was thrown while running the test
            /// </summary>
            public bool HasThrown { get { return _exception != null; } }

            /// <summary>
            /// Returns any exception that has been thrown while running the test
            /// </summary>
            public Exception Exception { get { return _exception; } }

            /// <summary>
            /// Has this thread finished running the test?
            /// </summary>
            public bool Finished { get; private set; }

            #endregion

            #region Public Methods

            public void Start()
            {
                new Thread(Run).Start();
            } 

            #endregion

            #region Private Methods

            private void Run()
            {
                _exception = null;
                try
                {
                    _parent.BaseExecute(_testInstanceState);
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }

                // Let our parent know we are finished
                lock( _parent._lock )
                {
                    Finished = true;
                    Monitor.PulseAll(_parent._lock);
                }
            } 

            #endregion
        }
    }
}
