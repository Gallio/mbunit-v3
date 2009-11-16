// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using System.Collections.Generic;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(FormatterAttribute))]
    public class FormatterAttributeTest : BaseAssertTest
    {
        [Formatter]
        public static string Format(FormattableStub obj)
        {
            return String.Format("CustomFormatter: FormattableStub's value is {0}.", obj.Value);
        }

        [Test]
        public void Run()
        {
            var stub = new FormattableStub(123);
            AssertionFailure[] failures = Capture(() => Assert.IsNull(stub));
            Assert.AreEqual(1, failures.Length);
            var actualValue = failures[0].LabeledValues.Single(x => x.Label == "Actual Value");
            Assert.AreEqual("CustomFormatter: FormattableStub's value is 123.", actualValue.FormattedValue.ToString());
        }

        public class FormattableStub
        {
            private readonly int value;

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public FormattableStub(int value)
            {
                this.value = value;
            }
        }
    }
}
