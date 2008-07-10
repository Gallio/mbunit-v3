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
using Gallio.Framework.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Formatting
{
    [TestFixture]
    [TestsOn(typeof(TypeFormattingRule))]
    public class TypeFormattingRuleTest : BaseFormattingRuleTest<TypeFormattingRule>
    {
        [Test]
        [Row(typeof(bool), "bool")]
        [Row(typeof(char), "char")]
        [Row(typeof(sbyte), "sbyte")]
        [Row(typeof(byte), "byte")]
        [Row(typeof(short), "short")]
        [Row(typeof(ushort), "ushort")]
        [Row(typeof(int), "int")]
        [Row(typeof(uint), "uint")]
        [Row(typeof(long), "long")]
        [Row(typeof(ulong), "ulong")]
        [Row(typeof(float), "float")]
        [Row(typeof(double), "double")]
        [Row(typeof(decimal), "decimal")]
        [Row(typeof(string), "string")]
        [Row(typeof(object), "object")]
        [Row(typeof(string[]), "string[]")]
        [Row(typeof(string[,]), "string[,]")]
        [Row(typeof(Dictionary<,>), "System.Collections.Generic.Dictionary<,>")]
        [Row(typeof(Dictionary<int,string>), "System.Collections.Generic.Dictionary<int,string>")]
        [Row(typeof(Dictionary<,>.Enumerator), "System.Collections.Generic.Dictionary<,>.Enumerator<,>")]
        public void Format(Type value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        public void FormatPointer()
        {
            Assert.AreEqual("string*", Formatter.Format(typeof(string).MakePointerType()));
        }

        [Test]
        public void FormatReference()
        {
            Assert.AreEqual("string&", Formatter.Format(typeof(string).MakeByRefType()));
        }

        [Test]
        [Row(typeof(Type), FormattingRulePriority.Best)]
        [Row(typeof(int), null)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }
    }
}