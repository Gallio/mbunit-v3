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
using System.Collections.Generic;
using System.Threading;
using Gallio.Runtime;
using Gallio.Framework.Utilities;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime
{
    [TestFixture]
    [TestsOn(typeof(UnhandledExceptionPolicy))]
    public class UnhandledExceptionPolicyTest
    {
        [Test, ExpectedArgumentNullException]
        public void ReportThrowsIfMessageIsNull()
        {
            UnhandledExceptionPolicy.Report(null, new Exception());
        }

        [Test, ExpectedArgumentNullException]
        public void ReportThrowsIfExceptionIsNull()
        {
            UnhandledExceptionPolicy.Report("blah", null);
        }

        [Test]
        public void PolicyPerformsCorrelationThenReporting()
        {
            using (IHost host = new IsolatedProcessHostFactory().CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.GetHostService().DoCallback(PolicyPerformsCorrelationThenReportingCallback);
            }
        }

        [Test]
        public void PolicyHandlesUnhandledExceptionsAndRecursion()
        {
            using (IHost host = new IsolatedProcessHostFactory().CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.GetHostService().DoCallback(PolicyHandlesUnhandledExceptionsAndRecursionCallback);
            }
        }

        private static void PolicyPerformsCorrelationThenReportingCallback()
        {
            Exception ex = new Exception();
            CorrelatedExceptionEventArgs finalArgs = null;

            UnhandledExceptionPolicy.CorrelateUnhandledException += delegate(object sender, CorrelatedExceptionEventArgs e)
            {
                e.AddCorrelationMessage("bar");
            };

            UnhandledExceptionPolicy.ReportUnhandledException += delegate(object sender, CorrelatedExceptionEventArgs e)
            {
                finalArgs = e;
            };

            UnhandledExceptionPolicy.Report("foo", ex);

            Assert.IsNotNull(finalArgs);
            Assert.AreEqual("foo\nbar", finalArgs.Message);
            Assert.AreSame(ex, finalArgs.Exception);
            Assert.IsFalse(finalArgs.IsRecursive);
        }

        private static void PolicyHandlesUnhandledExceptionsAndRecursionCallback()
        {
            List<CorrelatedExceptionEventArgs> args = new List<CorrelatedExceptionEventArgs>();

            UnhandledExceptionPolicy.CorrelateUnhandledException += delegate(object sender, CorrelatedExceptionEventArgs e)
            {
                args.Add(e);

                if (!e.IsRecursive)
                    throw new Exception("Correlation error.");
            };

            UnhandledExceptionPolicy.ReportUnhandledException += delegate(object sender, CorrelatedExceptionEventArgs e)
            {
                if (!e.IsRecursive)
                    throw new Exception("Reporting error.");
            };

            Thread t = new Thread((ThreadStart)delegate
            {
                throw new Exception("Error.");
            });

            t.Start();
            t.Join();

            Assert.AreEqual(3, args.Count);

            Assert.AreEqual("Error.", args[0].Exception.Message);
            Assert.IsFalse(args[0].IsRecursive);

            Assert.AreEqual("Correlation error.", args[1].Exception.Message);
            Assert.IsTrue(args[1].IsRecursive);

            Assert.AreEqual("Reporting error.", args[2].Exception.Message);
            Assert.IsTrue(args[2].IsRecursive);
        }
    }
}
