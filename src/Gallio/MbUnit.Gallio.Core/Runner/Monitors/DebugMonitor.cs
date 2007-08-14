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
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Serialization;
using MbUnit.Core.Reporting;
using MbUnit.Framework.Kernel.Events;
using System.Xml;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// Monitors <see cref="ITestRunner" /> events and writes messages to
    /// an output stream for debugging.
    /// </summary>
    /// <todo author="jeff">
    /// Tentative.  Subject to change!!
    /// Needs a lot of work anyways.
    /// </todo>
    public class DebugMonitor : BaseTestRunnerMonitor
    {
        private TextWriter writer;
        private TestSummaryMonitor summaryMonitor;

        /// <summary>
        /// Creates a console monitor.
        /// </summary>
        /// <param name="writer">The text writer for all output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public DebugMonitor(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;

            summaryMonitor = new TestSummaryMonitor();
        }

        /// <summary>
        /// Gets the associated test summary monitor.
        /// </summary>
        public TestSummaryMonitor SummaryMonitor
        {
            get { return summaryMonitor; }
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            summaryMonitor.Attach(Runner);
            Runner.EventDispatcher.Message += HandleMessageEvent;
            Runner.EventDispatcher.TestLifecycle += HandleTestLifecycleEvent;
        }

        private void HandleMessageEvent(object sender, MessageEventArgs e)
        {
            writer.WriteLine("[message({0})] - {1}\n", e.MessageType, e.Message);
        }

        private void HandleTestLifecycleEvent(object sender, TestLifecycleEventArgs e)
        {
            TestInfo testInfo = Runner.TestModel.Tests[e.TestId];

            switch (e.EventType)
            {
                case TestLifecycleEventType.Start:
                    writer.WriteLine("[start] - {0}\n", testInfo.Name);

                    foreach (KeyValuePair<string, IList<string>> entry in testInfo.Metadata.Entries)
                    {
                        foreach (string value in entry.Value)
                            writer.WriteLine("\t{0} = {1}", entry.Key, value);
                    }
                    break;

                case TestLifecycleEventType.Step:
                    writer.WriteLine("[step({0})] - {1}\n", e.StepName, testInfo.Name);
                    break;

                case TestLifecycleEventType.Finish:
                    writer.WriteLine("[finish] - {0}", testInfo.Name);

                    TestSummary summary = summaryMonitor.Summaries[e.TestId];
                    writer.WriteLine("\tState: {0}", summary.Result.State);
                    writer.WriteLine("\tOutcome: {0}", summary.Result.Outcome);
                    writer.WriteLine("\tAsserts: {0}", summary.Result.AssertCount);
                    writer.WriteLine("\tDuration: {0}", summary.Result.Duration);
                    writer.WriteLine("\tExecution Log:");

                    XmlSerializer serializer = new XmlSerializer(typeof(ExecutionLog));

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;

                    using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                        serializer.Serialize(xmlWriter, summary.ExecutionLog);

                    writer.WriteLine();
                    break;
            }
        }
    }
}
