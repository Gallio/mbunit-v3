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

namespace MbUnit.TestResources.Reflection
{
    /// <summary>
    /// This class contains a variety of different types that are used by
    /// the reflection policy tests.
    /// </summary>
    public class ReflectionPolicySample
    {
        public abstract class Class1
        {
            static Class1() { }

            public void Method1<T>(T param) { Event1 += null; }

            [return: Sample(typeof(int))]
            protected abstract int Method2();

            public int Field1;
            internal object Field2 = null;

            public int Property1 { get { return 0; } }
            public int Property2 { get { return 0; } set { } }
            protected abstract string Property3 { set; }

            [Sample(typeof(int))]
            public event EventHandler Event1;
            protected abstract event EventHandler Event2;
        }

        [Sample(typeof(int))]
        internal class Class2 : Class1
        {
            protected override event EventHandler Event2 { add { } remove { } }
            protected override string Property3 { set { } }
            protected override int Method2() { return 0; }
        }

        [Sample(typeof(string[]), Field = 2, Property = "foo")]
        internal class Class3 : Class2
        {
        }

        public struct Struct1<[Sample(typeof(int))] S, T> : Interface1
        {
            public Struct1(S s, [Sample(typeof(string[]))] T t) { }

            string Interface1.Method1([Sample(typeof(int), Field = 5)] string s, int x) { return ""; }
        }

        [Sample(typeof(string[]), Field = 2, Property = "foo")]
        public interface Interface1
        {
            string Method1(string s, int x);
        }

        [Sample(typeof(int))]
        public interface Interface2
        {
        }
    }
}
