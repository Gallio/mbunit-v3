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
using System.Collections.Generic;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model.Diagnostics;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest : BaseAssertTest
    {
        #region Fail

        [Test]
        public void Fail_without_parameters()
        {
            AssertionFailure[] failures = Capture(Assert.Fail);
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("An assertion failed.", failures[0].Description);
            Assert.IsNull(failures[0].Message);
        }

        [Test]
        public void Fail_with_message_and_arguments()
        {
            AssertionFailure[] failures = Capture(() => Assert.Fail("{0} {1}.", "MbUnit", "message"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("An assertion failed.", failures[0].Description);
            Assert.AreEqual("MbUnit message.", failures[0].Message);
        }

        #endregion
    }
}