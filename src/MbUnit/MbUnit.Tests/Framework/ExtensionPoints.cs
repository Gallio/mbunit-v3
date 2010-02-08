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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Tests.Framework;

namespace MbUnit.Framework.Tests
{
    public static class ExtensionPoints
    {
        [Converter]
        public static ConverterAttributeTest.NonConvertibleStub Convert(string source)
        {
            TestLog.WriteLine("CustomConverter: source = {0}", source);
            return new ConverterAttributeTest.NonConvertibleStub(Int32.Parse(source));
        }

        [Comparer]
        public static int Compare(ComparerAttributeTest.NonComparableStub x, ComparerAttributeTest.NonComparableStub y)
        {
            TestLog.WriteLine("CustomComparer: x = {0}, y = {1}", x.Value, y.Value);
            return x.Value.CompareTo(y.Value);
        }

        [EqualityComparer]
        public static bool Equals(EqualityComparerAttributeTest.NonEquatableStub x, EqualityComparerAttributeTest.NonEquatableStub y)
        {
            TestLog.WriteLine("CustomEqualityComparer: x = {0}, y = {1}", x.Value, y.Value);
            return x.Value == y.Value;
        }

        [Formatter]
        public static string Format(FormatterAttributeTest.FormattableStub obj)
        {
            return String.Format("CustomFormatter: FormattableStub's value is {0}.", obj.Value);
        }
    }
}
