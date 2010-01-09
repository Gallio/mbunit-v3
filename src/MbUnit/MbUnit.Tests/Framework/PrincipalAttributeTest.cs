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
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using System.Security.Principal;
using System.Threading;
using System.Text;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(PrincipalAttribute))]
    [RunSample(typeof(PrincipalAttributeSampleFixture))]
    public class PrincipalAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void Test()
        {
            TestStepRun test1Run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(PrincipalAttributeSampleFixture).GetMethod("Impersonate")));
            Assert.Contains(test1Run.TestLog.ToString(), "InRole = True, NotInRole = False");
        }

        internal class MyPrincipalAttribute : PrincipalAttribute
        {
            internal class MyPrincipal : IPrincipal
            {
                public IIdentity Identity
                {
                    get
                    {
                        return WindowsIdentity.GetCurrent();
                    }
                }

                public bool IsInRole(string role)
                {
                    return role == "InRole";
                }
            }


            protected override IPrincipal CreatePrincipal()
            {
                return new MyPrincipal();
            }
        }
        
        [TestFixture, Explicit("Sample")]
        internal class PrincipalAttributeSampleFixture
        {
            [Test, MyPrincipal]
            public void Impersonate()
            {
                var principal = Thread.CurrentPrincipal;
                TestLog.WriteLine(new StringBuilder()
                    .AppendFormat("InRole = {0}, ", principal.IsInRole("InRole"))
                    .AppendFormat("NotInRole = {0}", principal.IsInRole("NotInRole"))
                    .ToString());
            }
        }
    }
}
