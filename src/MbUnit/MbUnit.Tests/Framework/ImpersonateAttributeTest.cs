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
using System.Security.Principal;
using System.Transactions;
using Gallio.Common.Security;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using System.Linq;
using Gallio.Common.Markup;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MbUnit.Tests.Framework
{
    [TestFixture, Explicit("Requires special test users to be created on the local machine.")]
    [TestsOn(typeof(ImpersonateAttribute))]
    [RunSample(typeof(ImpersonateAttributeSample))]
    public class ImpersonateAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void Test()
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ImpersonateAttributeSample).GetMethod("Test")));
            var userNames = GetLogs(run.Children)
                .Select(x => Regex.Match(x, @"User = \w+\\(?<UserName>\w+)"))
                .Where(m => m.Success)
                .Select(m => m.Groups["UserName"].Value);
            Assert.AreElementsEqualIgnoringOrder(new[] { "TestUser", "AnotherTestUser" }, userNames);
        }

        [TestFixture, Explicit("Sample")]
        internal class ImpersonateAttributeSample
        {
            [Test]
            [Impersonate(UserName = "TestUser", Password = "TheP@$$w0rd")]
            [Impersonate(UserName = "AnotherTestUser", Password = "AnotherP@$$w0rd")]
            public void Test()
            {
                var identity = WindowsIdentity.GetCurrent();
                TestLog.Write("User = {0}", identity.Name);
            }
        }
    }
}
