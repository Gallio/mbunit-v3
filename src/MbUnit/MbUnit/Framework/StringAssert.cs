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
using System.Text.RegularExpressions;

#pragma warning disable 1591
#pragma warning disable 3001

namespace MbUnit.Framework
{
	/// <summary>
	/// String Assertion class
	/// </summary>
	public static class StringAssert
	{
		/// <summary>
		/// Asserts that two strings are equal, ignoring the case
		/// </summary>
		/// <param name="s1">
		/// Expected string
		/// </param>
		/// <param name="s2">
		/// Actual string
		/// </param>
		public static void AreEqualIgnoreCase(string s1, string s2)
		{
			if (s1==null || s2==null)
				Assert.AreEqual(s1,s2);
			else
				Assert.AreEqual(s1.ToLower(), s2.ToLower());
		}

		/// <summary>
		/// Asserts that the string is non null and empty
		/// </summary>
		/// <param name="s">
		/// String to test.
		/// </param>
		public static void IsEmpty(String s)
		{
			Assert.IsNotNull(s,"String is null");
			Assert.AreEqual(0,s.Length,
			                "String count is not 0");
		}

		/// <summary>
		/// Asserts that the string is non null and non empty
		/// </summary>
		/// <param name="s">
		/// String to test.
		/// </param>
		public static void IsNonEmpty(String s)
		{
			Assert.IsNotNull(s,"String is null");
			Assert.IsTrue(s.Length!=0,
			                "String count is 0");
		}

		/// <summary>
		/// Asserts the regular expression reg makes a full match on s
		/// </summary>
		/// <param name="s">
		/// String to test.
		/// </param>
		/// <param name="reg">
		/// Regular expression
		/// </param>
		public static void FullMatch(String s, string reg)
		{
			Regex regex = new Regex(reg);
			FullMatch(s,regex);
		}

		/// <summary>
		/// Asserts the regular expression regex makes a full match on
		///<paramref name="s"/>.
		/// </summary>		
		/// <param name="s">
		/// String to test.
		/// </param>
		/// <param name="regex">
		/// Regular expression
		/// </param>
		public static void FullMatch(String s, Regex regex)
		{
			Assert.IsNotNull(regex);
			
			Match m = regex.Match(s);
			Assert.IsTrue(m.Success, "Match is not successful");
			Assert.AreEqual(s.Length, m.Length, "Not a full match");
		}

		/// <summary>
		/// Asserts the regular expression reg makes a match on s
		/// </summary>
		/// <param name="s">
		/// String to test.
		/// </param>
		/// <param name="reg">
		/// Regular expression
		/// </param>
		public static void Like(String s, string reg)
		{
			Regex regex = new Regex(reg);
			Like(s,regex);
		}

		/// <summary>
		/// Asserts the regular expression regex makes a match on s
		/// </summary>	
		/// <param name="s">
		/// String to test.
		/// </param>
		/// <param name="regex">
		/// A <see cref="Regex"/> instance. 
		/// </param>
		public static void Like(String s, Regex regex)
		{
			Assert.IsNotNull(regex);
			
			Match m = regex.Match(s);
			Assert.IsTrue(m.Success, "Match is not successful");
		}

		/// <summary>
		/// Asserts the regular expression reg makes a match on s
		/// </summary>
		/// <param name="s">
		/// String to test.
		/// </param>
		/// <param name="reg">
		/// Regular expression
		/// </param>
		public static void NotLike(String s, string reg)
		{
			Regex regex = new Regex(reg);
			NotLike(s,regex);
		}

		/// <summary>
		/// Asserts the regular expression regex makes a match on s
		/// </summary>		
		/// <param name="s">
		/// String to test.
		/// </param>
		/// <param name="regex">
		/// A <see cref="Regex"/> instance. 
		/// </param>
		public static void NotLike(String s, Regex regex)
		{
			Assert.IsNotNull(regex);
			
			Match m = regex.Match(s);
			Assert.IsFalse(m.Success, "Match was found successful");
		}

		/// <summary>
		/// Asserts the string does not contain c
		/// </summary>	
		/// <param name="s">
		/// String to test.
		/// </param>
		/// <param name="anyOf">
		/// Variable list of characeters.
		/// </param> 			
		public static void DoesNotContain(String s, params char[] anyOf)
		{
			if (s==null)
				return;
			
			Assert.AreEqual(-1, s.IndexOfAny(anyOf),
			                "{0} contains at {1}",
			                s,s.IndexOfAny(anyOf));
		}

        public static void StartsWith(String s, string pattern)
        {
            Assert.IsTrue(s.StartsWith(pattern), "String [[{0}]] does not start with [[{1}]]",
                s, pattern);
        }

        public static void EndsWith(String s, string pattern)
        {
            Assert.IsTrue(s.EndsWith(pattern), "String [[{0}]] does not end with [[{1}]]",
                s, pattern);
        }

        public static void Contains(String s, string contain)
        {
            Assert.IsTrue(s.IndexOf(contain) >= 0, "String [[{0}]] does not contain [[{1}]]",
                s, contain);
        }
    }
}
