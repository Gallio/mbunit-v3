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

extern alias MbUnit2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Castle.Core.Interceptor;
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Plugin.CecilInstrumentation;
using MbUnit.Instrumentation;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Plugin.CecilInstrumentation.Tests
{
    /// <summary>
    /// Base class for Cecil-based instrumentation tests.
    /// </summary>
    /// <todo>
    /// Run PEVerify.
    /// Avoid reloading the assembly too often.
    /// </todo>
    /// <typeparam name="T">The instrumentor to test</typeparam>
    public abstract class BaseInstrumentorTest<T>
        where T : IInstrumentor, new()
    {
        private T instrumentor;
        private Assembly sampleAssembly;
        private SampleWrapper sample;
        private Type sampleType;

        private string assemblyPath;
        private string instrumentedAssemblyPath;

        private MultiMap<MethodInfo, IInterceptor> interceptors;

        public T Instrumentor
        {
            get { return instrumentor; }
        }

        public Assembly SampleAssembly
        {
            get { return sampleAssembly; }
        }

        public SampleWrapper Sample
        {
            get { return sample; }
        }

        public Type SampleType
        {
            get { return sampleType; }
        }

        public MethodInfo GetSampleMethod(string name)
        {
            return sampleType.GetMethod(name);
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            assemblyPath = Path.GetFullPath(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            instrumentedAssemblyPath = Path.Combine(Path.GetDirectoryName(assemblyPath),
                typeof(T).Name + "-" + Path.GetFileName(assemblyPath));

            instrumentor = new T();
            sampleAssembly = instrumentor.InstrumentAndLoad(assemblyPath, instrumentedAssemblyPath);
            sampleType = sampleAssembly.GetType(typeof(Sample).FullName);
        }

        [SetUp]
        public void SetUp()
        {
            sample = new SampleWrapper(Activator.CreateInstance(sampleType));
            interceptors = new MultiMap<MethodInfo, IInterceptor>();
        }

        [TearDown]
        public void TearDown()
        {
            ClearInterceptors();
        }

        [Test]
        public void PEVerify()
        {
            // TODO: Properly derive the path of peverify tool and fail with
            //       a warning if not installed.
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "peverify.exe";
            start.Arguments = string.Format("\"{0}\" /VERBOSE", instrumentedAssemblyPath);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardError = true;
            start.RedirectStandardOutput = true;

            Process process = Process.Start(start);
            process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) { Console.Out.WriteLine(e.Data); };
            process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) { Console.Error.WriteLine(e.Data); };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            Assert.AreEqual(0, process.ExitCode, "PEVerify generated errors.");
        }

        [Test]
        public void CallAppendHello()
        {
            Sample.AppendHello();

            Assert.AreEqual("Hello", Sample.ToString());
        }

        [Test]
        public void CallAppendHelloWithInterception()
        {
            AddInterceptor("AppendHello", new DelegateInterceptor(delegate(IInvocation invocation)
            {
                Sample.Append("I'm late!  ");
                invocation.Proceed();
                Sample.Append(", Goodbye!");
            }));

            Sample.AppendHello();

            Assert.AreEqual("I'm late!  Hello, Goodbye!", Sample.ToString());
        }

        protected void AddInterceptor(string methodName, IInterceptor interceptor)
        {
            MethodInfo method = GetSampleMethod(methodName);
            instrumentor.AddInterceptor(method, interceptor);
            interceptors.Add(method, interceptor);
        }

        protected bool RemoveInterceptor(string methodName, IInterceptor interceptor)
        {
            MethodInfo method = GetSampleMethod(methodName);
            bool result = instrumentor.RemoveInterceptor(method, interceptor);
            interceptors.Remove(method, interceptor);
            return result;
        }

        protected void ClearInterceptors()
        {
            foreach (KeyValuePair<MethodInfo, IList<IInterceptor>> entry in interceptors)
            {
                foreach (IInterceptor interceptor in entry.Value)
                    instrumentor.RemoveInterceptor(entry.Key, interceptor);
            }

            interceptors.Clear();
        }
    }
}
