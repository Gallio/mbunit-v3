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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Framework;
using Gallio.Framework.Formatting;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [Row(typeof(Assert))]
    [Row(typeof(AssertOverSyntax))]
    public class AssertImplementationConsistencyTest<TAssert>
    {
        private const string CustomMessageSuffix = "System.String, System.Object[])";
        private const string NoMessageSuffix = ")";

        private static readonly IList<string> IgnoredNames = new ReadOnlyCollection<string>(new[]
        { 
            "Equals",
            "ReferenceEquals"
        });

        [Test]
        public void EachAssertHasFlavorsWithAndWithoutMessageFormatAndArgs()
        {
            HashSet<string> asserts = new HashSet<string>();
            foreach (MethodInfo assertMethod in
                    typeof(TAssert).GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (! assertMethod.Name.StartsWith("get_") && ! assertMethod.Name.StartsWith("set_")
                    && ! IgnoredNames.Contains(assertMethod.Name))
                    asserts.Add(assertMethod.ToString());
            }

            foreach (string assert in asserts)
            {
                string counterpart;
                if (assert.EndsWith(", " + CustomMessageSuffix))
                    counterpart = ReplaceSuffix(assert, ", " + CustomMessageSuffix, NoMessageSuffix);
                else if (assert.EndsWith(CustomMessageSuffix))
                    counterpart = ReplaceSuffix(assert, CustomMessageSuffix, NoMessageSuffix);
                else if (assert.EndsWith("(" + NoMessageSuffix))
                    counterpart = ReplaceSuffix(assert, NoMessageSuffix, CustomMessageSuffix);
                else
                    counterpart = ReplaceSuffix(assert, NoMessageSuffix, ", " + CustomMessageSuffix);

                Assert.Contains(asserts, counterpart, "Assert type {0} should contain a counterpart for its '{1}' assert with / without a custom message format and arguments.",
                    typeof(TAssert), assert);
            }
        }

        private static string ReplaceSuffix(string str, string oldSuffix, string newSuffix)
        {
            AssertEx.That(() => str.EndsWith(oldSuffix));
            return string.Concat(str.Substring(0, str.Length - oldSuffix.Length), newSuffix); 
        }
    }
}
