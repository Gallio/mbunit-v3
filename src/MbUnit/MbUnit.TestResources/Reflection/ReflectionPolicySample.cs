// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

        /// <summary>
        /// This sample is a bit of a torture test for the exhaustive test case.
        /// </summary>
        [Sample(typeof(int))]
        [Serializable]
        public class TortureTest<[Sample(typeof(int))] T>
        {
            [Sample(typeof(int))]
            public int Field;

            public int Field2;

            private int nonInheritedField = 0;

            public const int Constant = 42;

            // Causes Cecil to throw NotSupportedException.
            //public volatile int VolatileField;

            public readonly int ReadOnlyField = 42;

            [return: Sample(typeof(int))]
            [Sample(typeof(int))]
            public virtual S InheritedMethod<[Sample(typeof(int))] S>([Sample(typeof(int))] S s, T t)
            {
                return default(S);
            }

            [return: Sample(typeof(string[]), Field = 2, Property = "foo")]
            [Sample(typeof(string[]), Field = 2, Property = "foo")]
            public virtual T InheritedMethod<[Sample(typeof(string[]), Field = 2, Property = "foo")] S>(T t, [Sample(typeof(string[]), Field = 2, Property = "foo")] S s)
            {
                return default(T);
            }

            [return: Sample(typeof(int))]
            [Sample(typeof(int))]
            public S NonInheritedMethod<[Sample(typeof(int))] S>([Sample(typeof(int))] S x)
            {
                return x;
            }

            [return: Sample(typeof(int))]
            [Sample(typeof(int))]
            public virtual S NonInheritedMethod2<[Sample(typeof(int))] S>([Sample(typeof(int))] S x)
            {
                return x;
            }

            [Sample(typeof(int))]
            public virtual event EventHandler InheritedEvent
            {
                add { }
                remove { }
            }

            [Sample(typeof(int))]
            public event EventHandler NonInheritedEvent
            {
                add { }
                remove { }
            }

            [Sample(typeof(int))]
            public virtual event EventHandler NonInheritedEvent2
            {
                add { }
                remove { }
            }

            [Sample(typeof(int))]
            public virtual int InheritedProperty
            {
                get { return 0; }
            }

            public virtual int InheritedProperty2
            {
                set { }
            }

            public virtual int InheritedProperty3
            {
                get { return nonInheritedField; }
                protected set { }
            }

            [Sample(typeof(int))]
            public int NonInherited
            {
                get { return 0; }
            }

            public virtual int NonInherited2
            {
                get { return 0; }
            }

            public int this[string index]
            {
                get { return 0; }
                set { }
            }

            public string this[int index1, int index2]
            {
                get { return ""; }
                set { }
            }

            public class NestedType
            {
                public class DirectlyRecursiveNestedType : NestedType
                {
                }

                public class MiddleType
                {
                    public class IndirectlyRecursiveNestedType : NestedType
                    {
                    }
                }
            }

            public class GenericDoublyNestedType<S>
            {
            }
        }

        public class TortureTest2<[Sample(typeof(string[]), Field = 2, Property = "foo")] T> : TortureTest<T[]>
        {
            new public string Field2;

            ~TortureTest2()
            {
            }

            [Sample(typeof(int))]
            public override S InheritedMethod<S>(S s, [Sample(typeof(int))] T[] t)
            {
                return default(S);
            }

            [Sample(typeof(int))]
            public override T[] InheritedMethod<S>([Sample(typeof(int))] T[] t, S s)
            {
                return null;
            }

            [return: Sample(typeof(int))]
            [Sample(typeof(int))]
            new public S NonInheritedMethod<[Sample(typeof(int))] S>([Sample(typeof(int))] S x)
            {
                return x;
            }

            [return: Sample(typeof(int))]
            [Sample(typeof(int))]
            new public virtual S NonInheritedMethod2<[Sample(typeof(int))] S>([Sample(typeof(int))] S x)
            {
                return x;
            }

            public override event EventHandler InheritedEvent
            {
                add { }
                remove { }
            }

            new public event EventHandler NonInheritedEvent
            {
                add { }
                remove { }
            }

            new public virtual event EventHandler NonInheritedEvent2
            {
                add { }
                remove { }
            }

            public override int InheritedProperty
            {
                get { return 0; }
            }

            public override int InheritedProperty2
            {
                set { }
            }

            public override int InheritedProperty3
            {
                get { return 0; }
                protected set { }
            }

            new public int NonInherited
            {
                get { return 0; }
            }

            new public virtual int NonInherited2
            {
                get { return 0; }
            }
        }
    }
}

/// <summary>
/// A test type in the global namespace.
/// </summary>
internal class ReflectionPolicySampleInGlobalNamespace
{
}