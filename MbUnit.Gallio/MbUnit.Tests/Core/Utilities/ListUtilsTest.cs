extern alias MbUnit24;
using MbUnit24::MbUnit.Framework;

using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Utilities;
using MbUnit.Framework;


namespace MbUnit.Tests.Core.Utilities
{
    [TestFixture]
    [TestsOn(typeof(ListUtils))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ListUtilsTest
    {
        [RowTest]
        [Row(new int[] { 1, 2, 3 }, 3, new string[] { "1", "2", "3" })]
        public void ConvertAndCopyAll(int[] input, int outputLength, string[] expectedOutput)
        {
            string[] output = new string[outputLength];

            ListUtils.ConvertAndCopyAll<int, string>(input, output, delegate(int value)
            {
                return value.ToString();
            });

            ArrayAssert.AreEqual(expectedOutput, output);
        }

        [RowTest]
        [Row(new int[] { 1, 2, 3 }, new string[] { "1", "2", "3" })]
        public void ConvertAllToArray(int[] input, string[] expectedOutput)
        {
            string[] output = ListUtils.ConvertAllToArray<int, string>(input, delegate(int value)
            {
                return value.ToString();
            });

            ArrayAssert.AreEqual(expectedOutput, output);
        }
    }
}
