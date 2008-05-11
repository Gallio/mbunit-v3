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
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(ArrayAssert))]
    public class ArrayAssertTest
    {
        [Test]
        public void AreEqualBool()
        {
            bool[] arr1 = new bool[5];
            bool[] arr2 = new bool[5];

            arr1[0] = true;
            arr1[1] = false;
            arr1[2] = true;
            arr1[3] = false;
            
            arr2[0] = true;
            arr2[1] = false;
            arr2[2] = true;
            arr2[3] = false;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualBoolNull()
        {
            bool[] arr1 = null;
            bool[] arr2 = null;
            
            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualChar()
        {
            Char[] arr1 = new Char[5];
            Char[] arr2 = new Char[5];

            arr1[0] = char.MaxValue;
            arr1[1] = char.MinValue;
            arr1[2] = char.MaxValue;
            arr1[3] = char.MinValue;

            arr2[0] = char.MaxValue;
            arr2[1] = char.MinValue;
            arr2[2] = char.MaxValue;
            arr2[3] = char.MinValue;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualCharNull()
        {
            Char[] arr1 = null;
            Char[] arr2 = null;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualByte()
        {
            byte[] arr1 = new byte[5];
            byte[] arr2 = new byte[5];

            arr1[0] = byte.MaxValue;
            arr1[1] = byte.MinValue;
            arr1[2] = byte.MaxValue;
            arr1[3] = byte.MinValue;

            arr2[0] = byte.MaxValue;
            arr2[1] = byte.MinValue;
            arr2[2] = byte.MaxValue;
            arr2[3] = byte.MinValue;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualByteNull()
        {
            byte[] arr1 = null;
            byte[] arr2 = null;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualInt()
        {
            int[] arr1 = new int[5];
            int[] arr2 = new int[5];

            arr1[0] = int.MaxValue;
            arr1[1] = int.MinValue;
            arr1[2] = int.MaxValue;
            arr1[3] = int.MinValue;

            arr2[0] = int.MaxValue;
            arr2[1] = int.MinValue;
            arr2[2] = int.MaxValue;
            arr2[3] = int.MinValue;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualIntNull()
        {
            int[] arr1 = null;
            int[] arr2 = null;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualLong()
        {
            long[] arr1 = new long[5];
            long[] arr2 = new long[5];

            arr1[0] = long.MaxValue;
            arr1[1] = long.MinValue;
            arr1[2] = long.MaxValue;
            arr1[3] = long.MinValue;

            arr2[0] = long.MaxValue;
            arr2[1] = long.MinValue;
            arr2[2] = long.MaxValue;
            arr2[3] = long.MinValue;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualLongNull()
        {
            long[] arr1 = null;
            long[] arr2 = null;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualFloat()
        {
            float[] arr1 = new float[5];
            float[] arr2 = new float[5];

            arr1[0] = float.MaxValue;
            arr1[1] = float.MinValue;
            arr1[2] = float.MaxValue;
            arr1[3] = float.MinValue;

            arr2[0] = float.MaxValue;
            arr2[1] = float.MinValue;
            arr2[2] = float.MaxValue;
            arr2[3] = float.MinValue;

            ArrayAssert.AreEqual(arr1, arr2, 0);
        }

        [Test]
        public void AreEqualFloatNull()
        {
            float[] arr1 = null;
            float[] arr2 = null;

            ArrayAssert.AreEqual(arr1, arr2, 0);
        }

        [Test]
        public void AreEqualDouble()
        {
            double[] arr1 = new double[5];
            double[] arr2 = new double[5];

            arr1[0] = double.MaxValue;
            arr1[1] = double.MinValue;
            arr1[2] = double.MaxValue;
            arr1[3] = double.MinValue;

            arr2[0] = double.MaxValue;
            arr2[1] = double.MinValue;
            arr2[2] = double.MaxValue;
            arr2[3] = double.MinValue;

            ArrayAssert.AreEqual(arr1, arr2, 0);
        }

        [Test]
        public void AreEqualDoubleNull()
        {
            double[] arr1 = null;
            double[] arr2 = null;

            ArrayAssert.AreEqual(arr1, arr2, 0);
        }

        [Test]
        public void AreEqualObject()
        {
            object[] arr1 = new object[5];
            object[] arr2 = new object[5];

            arr1[0] = true;
            arr1[1] = char.MaxValue;
            arr1[2] = float.MaxValue;
            arr1[3] = double.MinValue;

            arr2[0] = true;
            arr2[1] = char.MaxValue;
            arr2[2] = float.MaxValue;
            arr2[3] = double.MinValue;

            ArrayAssert.AreEqual(arr1, arr2);
        }

        [Test]
        public void AreEqualObjectNull()
        {
            object[] arr1 = null;
            object[] arr2 = null;

            ArrayAssert.AreEqual(arr1, arr2);
        }
    }
}
