// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License"){}
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
using System.Globalization;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
	public class Assert
	{
        public static void AreEqual(object expected, object actual){}
        public static void AreEqual<T>(T expected, T actual){}
        public static void AreEqual(double expected, double actual, double delta){}
        public static void AreEqual(object expected, object actual, string message){}
        public static void AreEqual(float expected, float actual, float delta){}
        public static void AreEqual<T>(T expected, T actual, string message){}
        public static void AreEqual(string expected, string actual, bool ignoreCase){}
        public static void AreEqual(double expected, double actual, double delta, string message){}
        public static void AreEqual(object expected, object actual, string message, params object[] parameters){}
        public static void AreEqual(float expected, float actual, float delta, string message){}
        public static void AreEqual<T>(T expected, T actual, string message, params object[] parameters){}
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture){}
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message){}
        public static void AreEqual(double expected, double actual, double delta, string message, params object[] parameters){}
        public static void AreEqual(float expected, float actual, float delta, string message, params object[] parameters){}
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message){}
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters){}
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters){}
        public static void AreNotEqual<T>(T notExpected, T actual){}
        public static void AreNotEqual(object notExpected, object actual){}
        public static void AreNotEqual(double notExpected, double actual, double delta){}
        public static void AreNotEqual(object notExpected, object actual, string message){}
        public static void AreNotEqual<T>(T notExpected, T actual, string message){}
        public static void AreNotEqual(float notExpected, float actual, float delta){}
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase){}
        public static void AreNotEqual(double notExpected, double actual, double delta, string message){}
        public static void AreNotEqual(float notExpected, float actual, float delta, string message){}
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message){}
        public static void AreNotEqual<T>(T notExpected, T actual, string message, params object[] parameters){}
        public static void AreNotEqual(object notExpected, object actual, string message, params object[] parameters){}
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture){}
        public static void AreNotEqual(double notExpected, double actual, double delta, string message, params object[] parameters){}
        public static void AreNotEqual(float notExpected, float actual, float delta, string message, params object[] parameters){}
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message){}
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message, params object[] parameters){}
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters){}
        public static void AreNotSame(object notExpected, object actual){}
        public static void AreNotSame(object notExpected, object actual, string message){}
        public static void AreNotSame(object notExpected, object actual, string message, params object[] parameters){}
        public static void AreSame(object expected, object actual){}
        public static void AreSame(object expected, object actual, string message){}
        public static void AreSame(object expected, object actual, string message, params object[] parameters){}
        internal static void CheckParameterNotNull(object param, string assertionName, string parameterName, string message, params object[] parameters){}
        public static void Fail(){}
        public static void Fail(string message){}
        public static void Fail(string message, params object[] parameters){}
        internal static void HandleFail(string assertionName, string message, params object[] parameters){}
        public static void Inconclusive(){}
        public static void Inconclusive(string message){}
        public static void Inconclusive(string message, params object[] parameters){}
        public static void IsFalse(bool condition){}
        public static void IsFalse(bool condition, string message){}
        public static void IsFalse(bool condition, string message, params object[] parameters){}
        public static void IsInstanceOfType(object value, Type expectedType){}
        public static void IsInstanceOfType(object value, Type expectedType, string message){}
        public static void IsInstanceOfType(object value, Type expectedType, string message, params object[] parameters){}
        public static void IsNotInstanceOfType(object value, Type wrongType){}
        public static void IsNotInstanceOfType(object value, Type wrongType, string message){}
        public static void IsNotInstanceOfType(object value, Type wrongType, string message, params object[] parameters){}
        public static void IsNotNull(object value){}
        public static void IsNotNull(object value, string message){}
        public static void IsNotNull(object value, string message, params object[] parameters){}
        public static void IsNull(object value){}
        public static void IsNull(object value, string message){}
        public static void IsNull(object value, string message, params object[] parameters){}
        public static void IsTrue(bool condition){}
        public static void IsTrue(bool condition, string message){}
        public static void IsTrue(bool condition, string message, params object[] parameters){}
	}
}
