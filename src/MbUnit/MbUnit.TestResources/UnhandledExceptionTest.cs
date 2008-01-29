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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MbUnit.Framework;

namespace MbUnit.TestResources
{
    /// <summary>
    /// This test fixture simply contains a test that throws an unhandled exception.
    /// We use this test to verify that the test runner does not terminate abruptly
    /// when unhandled exceptions occur.
    /// </summary>
    [TestFixture]
    public class UnhandledExceptionTest
    {
        [Test]
        public void ThrowUnhandledExceptionBeforeTheTestExits()
        {
            Thread t = new Thread((ThreadStart)delegate
            {
                throw new Exception("Unhandled!");
            });

            t.Start();
            t.Join();
        }

        [Test]
        public void ThrowUnhandledExceptionAfterTheTestExits()
        {
            Thread t = new Thread((ThreadStart)delegate
            {
                Thread.Sleep(3000);
                throw new Exception("Unhandled!");
            });

            t.Start();
        }
    }
}
