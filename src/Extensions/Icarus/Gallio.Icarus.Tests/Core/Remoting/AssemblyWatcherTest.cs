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

using System.IO;
using System.Threading;

using Gallio.Icarus.Core.Remoting;

using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Core.Remoting
{
    [TestFixture]
    public class AssemblyWatcherTest
    {
        private AssemblyWatcher assemblyWatcher;
        
        [SetUp]
        public void SetUp()
        {
            assemblyWatcher = new AssemblyWatcher();
        }

        [Test]
        public void Add_Test()
        {
            bool flag = false;
            string file = Path.GetTempFileName();
            assemblyWatcher.Add(file);
            assemblyWatcher.AssemblyChangedEvent += delegate
            {
                flag = true;
            };
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(file);
                sw.Write("test");
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
            File.Delete(file);
            Thread.Sleep(2000);
            Assert.IsTrue(flag);
        }

        [Test]
        public void Remove_Test()
        {
            string file = Path.GetTempFileName();
            assemblyWatcher.AssemblyChangedEvent += delegate
            {
                Assert.Fail("Still watching!");
            };
            assemblyWatcher.Add(file);
            assemblyWatcher.Remove(file);
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(file);
                sw.Write("test");
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
            File.Delete(file);
            Thread.Sleep(2000);
        }

        [Test]
        public void Clear_Test()
        {
            string file = Path.GetTempFileName();
            assemblyWatcher.AssemblyChangedEvent += delegate
            {
                Assert.Fail("Still watching!");
            };
            assemblyWatcher.Add(file);
            assemblyWatcher.Clear();
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(file);
                sw.Write("test");
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
            File.Delete(file);
            Thread.Sleep(2000);
        }
    }
}
