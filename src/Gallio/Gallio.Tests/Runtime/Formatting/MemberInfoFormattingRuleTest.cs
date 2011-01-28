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
using Gallio.Common.Reflection;
using Gallio.Runtime.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Formatting
{
    [TestFixture]
    [TestsOn(typeof(MemberInfoFormattingRule))]
    public class MemberInfoFormattingRuleTest : BaseFormattingRuleTest<MemberInfoFormattingRule>
    {
        public class SampleType
        {
            public int SampleField;
            public int SampleProperty { get; set; }
            public void SampleMethod() { }
        }

        public IEnumerable<object[]> GetMemberInfoData()
        {
            yield return new object[] { typeof(SampleType), null, null };
            yield return new object[] { 123, null, null };
            yield return new object[] { typeof(SampleType).GetMethod("SampleMethod"), FormattingRulePriority.Best, 
                                        "Gallio.Tests.Runtime.Formatting.MemberInfoFormattingRuleTest.SampleType, Method:SampleMethod" };
            yield return new object[] { typeof(SampleType).GetProperty("SampleProperty"), FormattingRulePriority.Best, 
                                        "Gallio.Tests.Runtime.Formatting.MemberInfoFormattingRuleTest.SampleType, Property:SampleProperty" };
            yield return new object[] { typeof(SampleType).GetField("SampleField"), FormattingRulePriority.Best, 
                                        "Gallio.Tests.Runtime.Formatting.MemberInfoFormattingRuleTest.SampleType, Field:SampleField" };
        }

        [Test, Factory("GetMemberInfoData")]
        public void Format(object value, int? expectedPriority, string expectedFormat)
        {
            int? actualPriority = FormattingRule.GetPriority(value.GetType());
            Assert.AreEqual(expectedPriority, actualPriority);

            if (expectedFormat != null)
            {
                string actualFormat = Formatter.Format(value);
                Assert.AreEqual(expectedFormat, actualFormat);
            }
        }
    }
}