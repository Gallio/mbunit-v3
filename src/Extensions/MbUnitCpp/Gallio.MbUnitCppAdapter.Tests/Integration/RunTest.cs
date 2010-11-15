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
// WITHOUT WC:\Projects\Gallio\v3\src\Extensions\MbUnitCpp\Gallio.MbUnitCppAdapter.Tests\Model\Bridge\UnmanagedTestRepositoryTest.csARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Tests;
using MbUnit.Framework;
using System.Reflection;
using System.IO;
using Gallio.Runner.Reports.Schema;
using Gallio.Framework;
using Gallio.Common.Markup;

namespace Gallio.MbUnitCppAdapter.Tests.Integration
{
    [TestFixture]
    public class RunTest : BaseTestWithSampleRunner
    {
        protected override void ConfigureRunner()
        {
            Runner.AddFile(new FileInfo(Helper.GetTestResources()));
        }

        [Test]
        public void ListAllTests()
        {
            IEnumerable<TestStepRun> runs = Runner.Report.TestPackageRun.AllTestStepRuns;
            string[] names = runs.Select(x => x.Step.FullName).Where(x => x.Length > 0).ToArray();
            Assert.IsNotEmpty(names);

            foreach (string name in names)
                TestLog.WriteLine("> " + name);
        }

        [Test]
        [Row("Simple", TestStatus.Passed, 0, null)]
        [Row("Simple/Empty_should_pass", TestStatus.Passed, 0, null)]
       
        [Row("Outcome", TestStatus.Failed, 2, null)]
        [Row("Outcome/Assert_Fail_with_default_message", TestStatus.Failed, 1, null)]
        [Row("Outcome/Assert_Fail_with_custom_message", TestStatus.Failed, 1, "Boom!")]
       
        [Row("Logic", TestStatus.Failed, 10, null)]
        [Row("Logic/Assert_IsTrue_should_pass", TestStatus.Passed, 1, null)]
        [Row("Logic/Assert_IsTrue_should_fail", TestStatus.Failed, 1, null)]
        [Row("Logic/Assert_IsTrue_as_int_should_pass", TestStatus.Passed, 1, null)]
        [Row("Logic/Assert_IsTrue_as_int_should_fail", TestStatus.Failed, 1, null)]
        [Row("Logic/Assert_IsTrue_should_fail_with_custom_message", TestStatus.Failed, 1, "This is a custom message.")]
        [Row("Logic/Assert_IsFalse_should_pass", TestStatus.Passed, 1, null)]
        [Row("Logic/Assert_IsFalse_should_fail", TestStatus.Failed, 1, null)]
        [Row("Logic/Assert_IsFalse_as_int_should_pass", TestStatus.Passed, 1, null)]
        [Row("Logic/Assert_IsFalse_as_int_should_fail", TestStatus.Failed, 1, null)]
        [Row("Logic/Assert_IsFalse_should_fail_with_custom_message", TestStatus.Failed, 1, "This is a custom message.")]

        [Row("Equality", TestStatus.Failed, 30, null)]
        [Row("Equality/Assert_AreEqual_bool_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_bool_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_char_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_char_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_wchar_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_wchar_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_uchar_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_uchar_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_short_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_short_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_ushort_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_ushort_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_int_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_int_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_uint_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_uint_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_longlong_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_longlong_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_float_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_float_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_double_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_double_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_char_pointer_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_char_pointer_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_constant_char_pointer_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_constant_char_pointer_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_wide_char_pointer_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_wide_char_pointer_should_fail", TestStatus.Failed, 1, null)]
        [Row("Equality/Assert_AreEqual_constant_wide_char_pointer_should_pass", TestStatus.Passed, 1, null)]
        [Row("Equality/Assert_AreEqual_constant_wide_char_pointer_should_fail", TestStatus.Failed, 1, null)]

        public void Test(string fullName, TestStatus expectedStatus, int expectedAssertCount, string expectedFailureLog)
        {
            IEnumerable<TestStepRun> runs = Runner.GetTestStepRuns(r => r.Step.FullName == fullName);
            Assert.IsNotEmpty(runs);
            TestStepRun run = runs.First();
            Assert.IsNotNull(run);
            Assert.AreEqual(expectedStatus, run.Result.Outcome.Status);
            Assert.AreEqual(expectedAssertCount, run.Result.AssertCount);

            if (expectedFailureLog != null)
                AssertLogContains(run, expectedFailureLog, MarkupStreamNames.Failures);
        }
    }
}
