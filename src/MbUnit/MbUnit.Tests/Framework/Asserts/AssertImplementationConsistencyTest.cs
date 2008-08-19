using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Framework;
using Gallio.Framework.Formatting;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [Row(typeof(NewAssert))]
    public class AssertImplementationConsistencyTest<TAssert>
    {
        private const string CustomMessageSuffix = "System.String, System.Object[])";
        private const string NoMessageSuffix = ")";

        [Test]
        public void EachAssertHasFlavorsWithAndWithoutMessageFormatAndArgs()
        {
            HashSet<string> asserts = new HashSet<string>();
            foreach (MethodInfo assertMethod in
                    typeof(TAssert).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (! assertMethod.Name.StartsWith("get_") && ! assertMethod.Name.StartsWith("set_"))
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

                NewAssert.Contains(asserts, counterpart, "Assert type {0} should contain a counterpart for its '{1}' assert with / without a custom message format and arguments.",
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
