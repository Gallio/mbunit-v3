// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Threading;
using Gallio.Common.Policies;
using Gallio.Framework;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Policies
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
            CorrelatedExceptionEventArgs finalArgs;

            using (IHost host = new IsolatedProcessHostFactory(RuntimeAccessor.Instance).CreateHost(new HostSetup(), new MarkupStreamLogger(TestLog.Default)))
            {
                HostAssemblyResolverHook.InstallCallback(host);
                finalArgs = (CorrelatedExceptionEventArgs)host.GetHostService().Do<object, object>(PolicyPerformsCorrelationThenReportingCallback, null);
            }

            Assert.IsNotNull(finalArgs);
            Assert.AreEqual("foo\nbar", finalArgs.Message);
            Assert.IsInstanceOfType<Exception>(finalArgs.Exception);
            Assert.IsFalse(finalArgs.IsRecursive);
        }

        [Test]
        public void PolicyHandlesUnhandledExceptionsAndRecursion()
        {
            List<CorrelatedExceptionEventArgs> args;

            using (IHost host = new IsolatedProcessHostFactory(RuntimeAccessor.Instance).CreateHost(new HostSetup(), new MarkupStreamLogger(TestLog.Default)))
            {
                HostAssemblyResolverHook.InstallCallback(host);
                args = (List<CorrelatedExceptionEventArgs>)host.GetHostService().Do<object, object>(PolicyHandlesUnhandledExceptionsAndRecursionCallback, null);
            }

            Assert.Count(3, args);
            Assert.AreEqual("Error.", args[0].Exception.Message);
            Assert.IsFalse(args[0].IsRecursive);
            Assert.AreEqual("Correlation error.", args[1].Exception.Message);
            Assert.IsTrue(args[1].IsRecursive);
            Assert.AreEqual("Reporting error.", args[2].Exception.Message);
            Assert.IsTrue(args[2].IsRecursive);
        }

        private static object PolicyPerformsCorrelationThenReportingCallback(object dummy)
        {
            var ex = new Exception("Some exception");
            CorrelatedExceptionEventArgs finalArgs = null;
            UnhandledExceptionPolicy.CorrelateUnhandledException += (sender, e) => e.AddCorrelationMessage("bar");
            UnhandledExceptionPolicy.ReportUnhandledException += (sender, e) => finalArgs = e;
            UnhandledExceptionPolicy.Report("foo", ex);
            return finalArgs;
        }

        private static object PolicyHandlesUnhandledExceptionsAndRecursionCallback(object dummy)
        {
            var args = new List<CorrelatedExceptionEventArgs>();

            UnhandledExceptionPolicy.CorrelateUnhandledException += (sender, e) =>
            {
                args.Add(e);

                if (!e.IsRecursive)
                    throw new Exception("Correlation error.");
            };

            UnhandledExceptionPolicy.ReportUnhandledException += (sender, e) =>
            {
                if (!e.IsRecursive)
                    throw new Exception("Reporting error.");
            };

            var t = new Thread(() => { throw new Exception("Error."); });
            t.Start();
            t.Join();
            return args;
        }
    }
}