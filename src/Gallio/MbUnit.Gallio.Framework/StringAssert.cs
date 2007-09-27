using System;
using System.Text.RegularExpressions;

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
