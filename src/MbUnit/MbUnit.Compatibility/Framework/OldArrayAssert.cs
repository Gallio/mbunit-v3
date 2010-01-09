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

#pragma warning disable 1591
#pragma warning disable 3001

namespace MbUnit.Framework
{
	/// <summary>
	/// Array Assertion class
	/// </summary>
    [Obsolete("Use Assert instead.")]
	public static class OldArrayAssert
	{
		/// <summary>
		/// Verifies that both array have the same dimension and elements.
		/// </summary>
		/// <param name="expected">The expected values.</param>
		/// <param name="actual">The actual values.</param>
		public static void AreEqual(bool[] expected, bool[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			OldAssert.IsNotNull(expected);
			OldAssert.IsNotNull(actual);
			
			OldAssert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			OldAssert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				OldAssert.AreEqual(expected[i], actual[i]);
			}
		}

		public static void AreEqual(char[] expected, char[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			OldAssert.IsNotNull(expected);
			OldAssert.IsNotNull(actual);
			
			OldAssert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			OldAssert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				OldAssert.AreEqual(expected[i], actual[i]);
			}
		}

		public static void AreEqual(byte[] expected, byte[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			OldAssert.IsNotNull(expected);
			OldAssert.IsNotNull(actual);
			
			OldAssert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			OldAssert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				OldAssert.AreEqual(expected[i], actual[i]);
			}
		}
						
		public static void AreEqual(int[] expected, int[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			OldAssert.IsNotNull(expected);
			OldAssert.IsNotNull(actual);
			
			OldAssert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			OldAssert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				OldAssert.AreEqual(expected[i], actual[i]);
			}
		}

		
		public static void AreEqual(long[] expected, long[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			OldAssert.IsNotNull(expected);
			OldAssert.IsNotNull(actual);
			
			OldAssert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			OldAssert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				OldAssert.AreEqual(expected[i], actual[i]);
			}
		}

		public static void AreEqual(float[] expected, float[] actual, float delta)
		{
			if(expected==null && actual==null)
				return;
			
			OldAssert.IsNotNull(expected);
			OldAssert.IsNotNull(actual);
			
			OldAssert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			OldAssert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				OldAssert.AreEqual(expected[i], actual[i],delta);
			}
		}


		public static void AreEqual(double[] expected, double[] actual, double delta)
		{
			if(expected==null && actual==null)
				return;
			
			OldAssert.IsNotNull(expected);
			OldAssert.IsNotNull(actual);
			
			OldAssert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			OldAssert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				OldAssert.AreEqual(expected[i], actual[i],delta);
			}
		}	
		

		public static void AreEqual(object[] expected, object[] actual)
		{
			if(expected==null && actual==null)
				return;
			
			OldAssert.IsNotNull(expected);
			OldAssert.IsNotNull(actual);
			
			OldAssert.AreEqual(expected.Rank,actual.Rank,"Rank are not equal");
			OldAssert.AreEqual(expected.Length,actual.Length);
			for(int i = 0;i<expected.Length;++i)
			{
				OldAssert.AreEqual(expected[i], actual[i]);
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
