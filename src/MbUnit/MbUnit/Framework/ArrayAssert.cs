// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace MbUnit.Framework
{
	/// <summary>
	/// Array Assertion class
	/// </summary>
	public static class ArrayAssert
	{
		/// <summary>
		/// Verifies that both array have the same dimension and elements.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		public static void AreEqual(bool[] expected, bool[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			
			Assert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			Assert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

		public static void AreEqual(char[] expected, char[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			
			Assert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			Assert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

		public static void AreEqual(byte[] expected, byte[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			
			Assert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			Assert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}
						
		public static void AreEqual(int[] expected, int[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			
			Assert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			Assert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

		
		public static void AreEqual(long[] expected, long[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			
			Assert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			Assert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

		public static void AreEqual(float[] expected, float[] actual, float delta)
		{
			if(expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			
			Assert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			Assert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				Assert.AreEqual(expected[i], actual[i],delta);
			}
		}


		public static void AreEqual(double[] expected, double[] actual, double delta)
		{
			if(expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			
			Assert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			Assert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				Assert.AreEqual(expected[i], actual[i],delta);
			}
		}	
		

		public static void AreEqual(object[] expected, object[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			
			Assert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			Assert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

        /// <summary>
        /// Checks whether an object is of an <see cref="Array" /> type.
        /// </summary>
        /// <param name="obj">Object instance to check as an array.</param>
        /// <returns>True if <see cref="Array"/> Type, False otherwise.</returns>
        static internal bool IsArrayType(object obj)
        {
            bool isArrayType = false;
            if (obj != null)
            {
                isArrayType = obj.GetType().IsArray;
            }
            return isArrayType;
        }

	}
}
