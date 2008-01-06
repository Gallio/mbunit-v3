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
using System.Diagnostics;
using Gallio.Contexts;
using Gallio.Logging;
using Gallio.Reflection;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// An implementation of <see cref="ITestStepMonitor" /> that notifies a
    /// <see cref="IContextHandler" /> as its state changes.
    /// </summary>
    public class ContextualTestStepMonitor : ITestStepMonitor
    {
        private readonly IContextHandler handler;
        private readonly ITestStep step;

        private Context context;

        /// <summary>
        /// Creates a test step monitor.
        /// </summary>
        /// <param name="handler">The context handler</param>
        /// <param name="step">The test step</param>
        public ContextualTestStepMonitor(IContextHandler handler, ITestStep step)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");
            if (step == null)
                throw new ArgumentNullException("step");

            this.handler = handler;
            this.step = step;
        }

        /// <inheritdoc />
        public Context Context
        {
            get { return context; }
        }

        /// <inheritdoc />
        public ITestStep Step
        {
            get { return step; }
        }

        /// <inheritdoc />
        public LogWriter LogWriter
        {
            get { return context.LogWriter; }
        }

        /// <inheritdoc />
        public string LifecyclePhase
        {
            get { return context.LifecyclePhase; }
            set { handler.SetLifecyclePhase(context, value); }
        }

        /// <inheritdoc />
        public TestOutcome Outcome
        {
            get { return context.Outcome; }
        }

        /// <inheritdoc />
        public ITestStepMonitor StartChildStep(ITestStep childStep)
        {
            if (childStep == null)
                throw new ArgumentNullException(@"childStep");
            if (childStep.Parent != Step)
                throw new ArgumentException("Expected a child of this step.", "childStep");

            ContextualTestStepMonitor stepMonitor = new ContextualTestStepMonitor(handler, childStep);
            stepMonitor.Start();
            return stepMonitor;
        }

        /// <inheritdoc />
        public ITestStepMonitor StartChildStep(string name, ICodeElementInfo codeElement)
        {
            return StartChildStep(new BaseTestStep(step.TestInstance, name, codeElement, step));
        }

        /// <inheritdoc />
        public void FinishStep(TestStatus status, TestOutcome outcome, TimeSpan? actualDuration)
        {
            handler.FinishStep(context, status, outcome, actualDuration);
        }

        /// <inheritdoc />
        public void SetInterimOutcome(TestOutcome outcome)
        {
            handler.SetInterimOutcome(context, outcome);
        }

        /// <inheritdoc />
        public void AddMetadata(string metadataKey, string metadataValue)
        {
            handler.AddMetadata(context, metadataKey, metadataValue);
        }

        /// <summary>
        /// Starts the step.
        /// </summary>
        public void Start()
        {
            context = handler.CreateContext(step);
            handler.StartStep(context);
        }
    }
}