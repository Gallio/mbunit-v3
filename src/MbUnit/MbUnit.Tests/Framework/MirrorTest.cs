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
using System.Reflection;
using System.Text;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Mirror))]
    public class MirrorTest
    {
        [Test]
        public void NullOfUnknownType_HasExpectedProperties()
        {
            Assert.Multiple(() =>
            {
                Assert.IsTrue(Mirror.NullOfUnknownType.IsNull);
                Assert.IsTrue(Mirror.NullOfUnknownType.IsNullOfUnknownType);
                Assert.IsNull(Mirror.NullOfUnknownType.Type);
                Assert.IsNull(Mirror.NullOfUnknownType.Instance);
            });
        }

        [Test]
        public void ForType_Generic_ReturnsTypeMirror()
        {
            var mirror = Mirror.ForType<int>();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(mirror.IsNull);
                Assert.IsFalse(mirror.IsNullOfUnknownType);
                Assert.AreEqual(typeof(int), mirror.Type);
                Assert.IsNull(mirror.Instance);
            });
        }

        [Test]
        public void ForType_BYType_WhenTypeIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Mirror.ForType((Type)null));
        }

        [Test]
        public void ForType_ByType_WhenTypeIsNotNull_ReturnsTypeMirror()
        {
            var mirror = Mirror.ForType(typeof(int));

            Assert.Multiple(() =>
            {
                Assert.IsTrue(mirror.IsNull);
                Assert.IsFalse(mirror.IsNullOfUnknownType);
                Assert.AreEqual(typeof(int), mirror.Type);
                Assert.IsNull(mirror.Instance);
            });
        }

        [Test]
        public void ForType_ByTypeName_WhenTypeNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Mirror.ForType((string)null));
        }

        [Test]
        public void ForType_ByTypeName_WhenTypeNameIsValid_ReturnsTypeMirror()
        {
            var mirror = Mirror.ForType("System.Int32");

            Assert.Multiple(() =>
            {
                Assert.IsTrue(mirror.IsNull);
                Assert.IsFalse(mirror.IsNullOfUnknownType);
                Assert.AreEqual(typeof(int), mirror.Type);
                Assert.IsNull(mirror.Instance);
            });
        }

        [Test]
        public void ForType_ByTypeName_WhenTypeNameIsNotValid_Throws()
        {
            var ex = Assert.Throws<MirrorException>(() => Mirror.ForType("This_Is_Not_A_Known_Type_Name"));
            Assert.AreEqual("Could not find type 'This_Is_Not_A_Known_Type_Name'.", ex.Message);
        }

        [Test]
        public void ForType_ByTypeNameAndAssembly_WhenTypeNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Mirror.ForType(null, typeof(int).Assembly));
        }

        [Test]
        public void ForType_ByTypeNameAndAssembly_WhenAssemblyIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Mirror.ForType("System.Int32", (Assembly)null));
        }

        [Test]
        public void ForType_ByTypeNameAndAssembly_WhenTypeNameAndAssemblyAreValid_ReturnsTypeMirror()
        {
            var mirror = Mirror.ForType("System.Int32", typeof(int).Assembly);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(mirror.IsNull);
                Assert.IsFalse(mirror.IsNullOfUnknownType);
                Assert.AreEqual(typeof(int), mirror.Type);
                Assert.IsNull(mirror.Instance);
            });
        }

        [Test]
        public void ForType_ByTypeNameAndAssembly_WhenTypeNameIsNotValid_Throws()
        {
            var ex = Assert.Throws<MirrorException>(() => Mirror.ForType("This_Is_Not_A_Known_Type_Name", typeof(int).Assembly));
            Assert.Like(ex.Message, @"Could not find type 'This_Is_Not_A_Known_Type_Name' in assembly 'mscorlib.*'\.");
        }

        [Test]
        public void ForType_ByTypeNameAndAssemblyName_WhenTypeNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Mirror.ForType(null, "mscorlib"));
        }

        [Test]
        public void ForType_ByTypeNameAndAssemblyName_WhenAssemblyNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Mirror.ForType("System.Int32", (string)null));
        }

        [Test]
        public void ForType_ByTypeNameAndAssemblyName_WhenTypeNameAndAssemblyNameAreValid_ReturnsTypeMirror()
        {
            var mirror = Mirror.ForType("System.Int32", "mscorlib");

            Assert.Multiple(() =>
            {
                Assert.IsTrue(mirror.IsNull);
                Assert.IsFalse(mirror.IsNullOfUnknownType);
                Assert.AreEqual(typeof(int), mirror.Type);
                Assert.IsNull(mirror.Instance);
            });
        }

        [Test]
        public void ForType_ByTypeNameAndAssemblyName_WhenTypeNameIsNotValid_Throws()
        {
            var ex = Assert.Throws<MirrorException>(() => Mirror.ForType("This_Is_Not_A_Known_Type_Name", "mscorlib"));
            Assert.Like(ex.Message, @"Could not find type 'This_Is_Not_A_Known_Type_Name' in assembly 'mscorlib.*'\.");
        }

        [Test]
        public void ForType_ByTypeNameAndAssemblyName_WhenAssemblyNameIsNotValid_Throws()
        {
            var ex = Assert.Throws<MirrorException>(() => Mirror.ForType("System.Int32", "This_Is_Not_A_Known_Assembly_Name"));
            Assert.Like(ex.Message, @"Could not find type 'System\.Int32' in assembly 'This_Is_Not_A_Known_Assembly_Name'.");
        }

        [Test]
        public void ForObject_WhenObjectIsNull_ReturnsNullOfUnknownType()
        {
            var mirror = Mirror.ForObject(null);

            Assert.AreSame(Mirror.NullOfUnknownType, mirror);
        }

        [Test]
        public void ForObject_WhenObjectIsNotNull_ReturnsObjectMirror()
        {
            var obj = this;
            var mirror = Mirror.ForObject(obj);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(mirror.IsNull);
                Assert.IsFalse(mirror.IsNullOfUnknownType);
                Assert.AreEqual(obj.GetType(), mirror.Type);
                Assert.AreSame(obj, mirror.Instance);
            });
        }

        [Test]
        public void Indexer_WhenMemberNameIsNull_Throws()
        {
            var mirror = Mirror.ForType<SampleObject>();

            Assert.Throws<ArgumentNullException>(() => { object x = mirror[null]; });
        }

        [Test]
        public void MemberSetIndexer_WhenIndexArgsIsNull_Throws()
        {
            var mirror = Mirror.ForType<SampleObject>();

            Assert.Throws<ArgumentNullException>(() => { object x = mirror["StaticProperty"][(object[])null]; });
        }

        [Test]
        public void MemberSetInvoke_WhenArgsIsNull_Throws()
        {
            var mirror = Mirror.ForType<SampleObject>();

            Assert.Throws<ArgumentNullException>(() => mirror["StaticMethod"].Invoke((object[])null));
        }

        [Test]
        public void MemberSetInvokeAsMirror_WhenArgsIsNull_Throws()
        {
            var mirror = Mirror.ForType<SampleObject>();

            Assert.Throws<ArgumentNullException>(() => mirror["StaticMethod"].InvokeAsMirror((object[])null));
        }

        [Test]
        public void MemberSetWithSignature_WhenArgTypesIsNull_Throws()
        {
            var mirror = Mirror.ForType<SampleObject>();

            Assert.Throws<ArgumentNullException>(() => mirror["StaticMethod"].WithSignature((Type[])null));
        }

        [Test]
        public void MemberSetWithGenericArgs_WhenArgTypesIsNull_Throws()
        {
            var mirror = Mirror.ForType<SampleObject>();

            Assert.Throws<ArgumentNullException>(() => mirror["StaticMethod"].WithGenericArgs((Type[])null));
        }

        [Test]
        public void Syntax_InstanceField()
        {
            var obj = new SampleObject();
            var mirror = Mirror.ForObject(obj);

            Assert.AreEqual("InstanceField", mirror["InstanceField"].MemberInfo.Name);

            mirror["InstanceField"].Value = 5;
            Assert.AreEqual(5, obj.InstanceField);

            Assert.AreEqual(5, mirror["InstanceField"].Value);

            Assert.AreEqual(5, mirror["InstanceField"].ValueAsMirror.Instance);
        }

        [Test]
        public void Syntax_StaticField([Column(false, true)] bool usingObjectMirror)
        {
            var mirror = usingObjectMirror ? Mirror.ForObject(new SampleObject()) : Mirror.ForType<SampleObject>();

            Assert.AreEqual("StaticField", mirror["StaticField"].MemberInfo.Name);

            mirror["StaticField"].Value = 5;
            Assert.AreEqual(5, SampleObject.StaticField);

            Assert.AreEqual(5, mirror["StaticField"].Value);

            Assert.AreEqual(5, mirror["StaticField"].ValueAsMirror.Instance);
        }

        [Test]
        public void Syntax_InstanceProperty()
        {
            var obj = new SampleObject();
            var mirror = Mirror.ForObject(obj);

            Assert.AreEqual("InstanceProperty", mirror["InstanceProperty"].MemberInfo.Name);

            mirror["InstanceProperty"].Value = 5;
            Assert.AreEqual(5, obj.InstanceProperty);

            Assert.AreEqual(5, mirror["InstanceProperty"].Value);

            Assert.AreEqual(5, mirror["InstanceProperty"].ValueAsMirror.Instance);
        }

        [Test]
        public void Syntax_StaticProperty([Column(false, true)] bool usingObjectMirror)
        {
            var mirror = usingObjectMirror ? Mirror.ForObject(new SampleObject()) : Mirror.ForType<SampleObject>();

            Assert.AreEqual("StaticProperty", mirror["StaticProperty"].MemberInfo.Name);

            mirror["StaticProperty"].Value = 5;
            Assert.AreEqual(5, SampleObject.StaticProperty);

            Assert.AreEqual(5, mirror["StaticProperty"].Value);

            Assert.AreEqual(5, mirror["StaticProperty"].ValueAsMirror.Instance);
        }

        [Test]
        public void Syntax_IndexedProperty()
        {
            var obj = new SampleObject();
            var mirror = Mirror.ForObject(obj);

            var ex = Assert.Throws<MirrorException>(() => { var m = mirror["Item"].MemberInfo; });
            Assert.AreEqual("Could not find a unique matching member 'Item' of type 'MbUnit.Tests.Framework.MirrorTest+SampleObject'.  There were 2 matches out of 2 members with the same name.  Try providing additional information to narrow down the choices.", ex.Message);

            Assert.AreEqual("Item", mirror["Item"].WithSignature(typeof(int)).MemberInfo.Name);
            Assert.AreEqual(1, ((PropertyInfo)mirror["Item"].WithSignature(typeof(int)).MemberInfo).GetIndexParameters().Length);
            Assert.AreEqual("Item", mirror["Item"].WithSignature(typeof(int), typeof(int)).MemberInfo.Name);
            Assert.AreEqual(2, ((PropertyInfo)mirror["Item"].WithSignature(typeof(int), typeof(int)).MemberInfo).GetIndexParameters().Length);

            Assert.AreEqual(8, mirror["Item"][4].Value);
            Assert.AreEqual(28, mirror["Item"][4, 7].Value);

            Assert.AreEqual(8, mirror["Item"][4].ValueAsMirror.Instance);
            Assert.AreEqual(28, mirror["Item"][4, 7].ValueAsMirror.Instance);

            mirror["Item"][4].Value = 2;
            Assert.AreEqual(8, obj.indexerLastSetValue);
            mirror["Item"][4, 7].Value = 2;
            Assert.AreEqual(56, obj.indexerLastSetValue);
        }

        [Test]
        public void Syntax_InstanceEvent()
        {
            var obj = new SampleObject();
            var mirror = Mirror.ForObject(obj);

            Assert.AreEqual("InstanceCustomEvent", mirror["InstanceCustomEvent"].MemberInfo.Name);

            // adding/removing null should work event though no handlers will be registered
            mirror["InstanceCustomEvent"].AddHandler((Delegate)null);
            mirror["InstanceCustomEvent"].AddHandler((EventHandler)null);
            mirror["InstanceCustomEvent"].AddHandler((EventHandler<EventArgs>)null);
            mirror["InstanceCustomEvent"].RemoveHandler((Delegate)null);
            mirror["InstanceCustomEvent"].RemoveHandler((EventHandler)null);
            mirror["InstanceCustomEvent"].RemoveHandler((EventHandler<EventArgs>)null);
            Assert.AreEqual(6, obj.instanceCustomEventAddRemoveCount);
            Assert.IsNull(obj.instanceCustomEvent);

            // adding other handlers
            int handled = 0;
            SampleObject.CustomEventHandler nonCoercedHandler = (sender, e) => { handled++; };
            mirror["InstanceCustomEvent"].AddHandler(nonCoercedHandler);
            mirror["InstanceCustomEvent"].AddHandler((sender, e) => { handled++; }); // coerced
            mirror["InstanceCustomEvent"].AddHandler<EventArgs>((sender, e) => { handled++; }); // coerced
            Assert.AreEqual(9, obj.instanceCustomEventAddRemoveCount);
            Assert.IsNotNull(obj.instanceCustomEvent);
            obj.RaiseInstanceCustomEvent();
            Assert.AreEqual(3, handled);

            // remove handler (non-coerced only)
            mirror["InstanceCustomEvent"].RemoveHandler(nonCoercedHandler);
            Assert.AreEqual(10, obj.instanceCustomEventAddRemoveCount);
            Assert.IsNotNull(obj.instanceCustomEvent);
            obj.RaiseInstanceCustomEvent();
            Assert.AreEqual(5, handled);
        }

        [Test]
        public void Syntax_StaticEvent([Column(false, true)] bool usingObjectMirror)
        {
            var mirror = usingObjectMirror ? Mirror.ForObject(new SampleObject()) : Mirror.ForType<SampleObject>();
            SampleObject.staticCustomEvent = null;
            SampleObject.staticCustomEventAddRemoveCount = 0;

            Assert.AreEqual("StaticCustomEvent", mirror["StaticCustomEvent"].MemberInfo.Name);

            // adding/removing null should work event though no handlers will be registered
            mirror["StaticCustomEvent"].AddHandler((Delegate)null);
            mirror["StaticCustomEvent"].AddHandler((EventHandler)null);
            mirror["StaticCustomEvent"].AddHandler((EventHandler<EventArgs>)null);
            mirror["StaticCustomEvent"].RemoveHandler((Delegate)null);
            mirror["StaticCustomEvent"].RemoveHandler((EventHandler)null);
            mirror["StaticCustomEvent"].RemoveHandler((EventHandler<EventArgs>)null);
            Assert.AreEqual(6, SampleObject.staticCustomEventAddRemoveCount);
            Assert.IsNull(SampleObject.staticCustomEvent);

            // adding other handlers
            int handled = 0;
            SampleObject.CustomEventHandler nonCoercedHandler = (sender, e) => { handled++; };
            mirror["StaticCustomEvent"].AddHandler(nonCoercedHandler);
            mirror["StaticCustomEvent"].AddHandler((sender, e) => { handled++; }); // coerced
            mirror["StaticCustomEvent"].AddHandler<EventArgs>((sender, e) => { handled++; }); // coerced
            Assert.AreEqual(9, SampleObject.staticCustomEventAddRemoveCount);
            Assert.IsNotNull(SampleObject.staticCustomEvent);
            SampleObject.RaiseStaticCustomEvent();
            Assert.AreEqual(3, handled);

            // remove handler (non-coerced only)
            mirror["StaticCustomEvent"].RemoveHandler(nonCoercedHandler);
            Assert.AreEqual(10, SampleObject.staticCustomEventAddRemoveCount);
            Assert.IsNotNull(SampleObject.staticCustomEvent);
            SampleObject.RaiseStaticCustomEvent();
            Assert.AreEqual(5, handled);
        }

        [Test]
        public void Syntax_InstanceMethod()
        {
            var obj = new SampleObject();
            var mirror = Mirror.ForObject(obj);

            var ex = Assert.Throws<MirrorException>(() => { var m = mirror["InstanceMethod"].MemberInfo; });
            Assert.AreEqual("Could not find a unique matching member 'InstanceMethod' of type 'MbUnit.Tests.Framework.MirrorTest+SampleObject'.  There were 5 matches out of 5 members with the same name.  Try providing additional information to narrow down the choices.", ex.Message);

            Assert.AreEqual("InstanceMethod", mirror["InstanceMethod"].WithGenericArgs().WithSignature().MemberInfo.Name);
            Assert.AreEqual(0, ((MethodInfo)mirror["InstanceMethod"].WithGenericArgs().WithSignature().MemberInfo).GetParameters().Length);
            Assert.AreEqual("InstanceMethod", mirror["InstanceMethod"].WithGenericArgs().WithSignature(typeof(int)).MemberInfo.Name);
            Assert.AreEqual(1, ((MethodInfo)mirror["InstanceMethod"].WithGenericArgs().WithSignature(typeof(int)).MemberInfo).GetParameters().Length);
            Assert.AreEqual("InstanceMethod", mirror["InstanceMethod"].WithSignature(typeof(int), typeof(int)).MemberInfo.Name);
            Assert.AreEqual(2, ((MethodInfo)mirror["InstanceMethod"].WithSignature(typeof(int), typeof(int)).MemberInfo).GetParameters().Length);
            Assert.AreEqual("InstanceMethod", mirror["InstanceMethod"].WithGenericArgs(typeof(int)).WithSignature().MemberInfo.Name);
            Assert.AreEqual(0, ((MethodInfo)mirror["InstanceMethod"].WithGenericArgs(typeof(int)).WithSignature().MemberInfo).GetParameters().Length);
            Assert.AreEqual("InstanceMethod", mirror["InstanceMethod"].WithGenericArgs(typeof(int)).WithSignature(typeof(int)).MemberInfo.Name);
            Assert.AreEqual(1, ((MethodInfo)mirror["InstanceMethod"].WithGenericArgs(typeof(int)).WithSignature(typeof(int)).MemberInfo).GetParameters().Length);

            Assert.AreEqual(5, mirror["InstanceMethod"].WithGenericArgs().Invoke());
            Assert.AreEqual(8, mirror["InstanceMethod"].WithGenericArgs().Invoke(4));
            Assert.AreEqual(28, mirror["InstanceMethod"].Invoke(4, 7));
            Assert.AreEqual(0, mirror["InstanceMethod"].WithGenericArgs(typeof(int)).Invoke());
            Assert.AreEqual(4, mirror["InstanceMethod"].WithGenericArgs(typeof(int)).Invoke(4));
        }

        [Test]
        public void Syntax_InstanceMethod_with_null_arguments()
        {
            var obj = new SampleObject();
            var mirror = Mirror.ForObject(obj);

            var ex = Assert.Throws<MirrorException>(() => { var m = mirror["InstanceMethodForNull"].MemberInfo; });
            Assert.AreEqual("Could not find a unique matching member 'InstanceMethodForNull' of type 'MbUnit.Tests.Framework.MirrorTest+SampleObject'.  There were 2 matches out of 2 members with the same name.  Try providing additional information to narrow down the choices.", ex.Message);

            Assert.AreEqual("InstanceMethodForNull", mirror["InstanceMethodForNull"].WithSignature(typeof(string)).MemberInfo.Name);
            Assert.AreEqual(1, ((MethodInfo)mirror["InstanceMethodForNull"].WithSignature(typeof(string)).MemberInfo).GetParameters().Length);
            Assert.AreEqual("InstanceMethodForNull", mirror["InstanceMethodForNull"].WithSignature(typeof(int)).MemberInfo.Name);
            Assert.AreEqual(1, ((MethodInfo)mirror["InstanceMethodForNull"].WithSignature(typeof(int)).MemberInfo).GetParameters().Length);

            Assert.AreEqual(25, mirror["InstanceMethodForNull"].Invoke(25));
            Assert.AreEqual(6, mirror["InstanceMethodForNull"].Invoke("abcdef"));
            Assert.AreEqual(0, mirror["InstanceMethodForNull"].Invoke(new object[] { null })); // Resolve null parameter as string.
        }

        [Test]
        public void Syntax_StaticMethod([Column(false, true)] bool usingObjectMirror)
        {
            var mirror = usingObjectMirror ? Mirror.ForObject(new SampleObject()) : Mirror.ForType<SampleObject>();

            var ex = Assert.Throws<MirrorException>(() => { var m = mirror["StaticMethod"].MemberInfo; });
            Assert.AreEqual("Could not find a unique matching member 'StaticMethod' of type 'MbUnit.Tests.Framework.MirrorTest+SampleObject'.  There were 5 matches out of 5 members with the same name.  Try providing additional information to narrow down the choices.", ex.Message);

            Assert.AreEqual("StaticMethod", mirror["StaticMethod"].WithGenericArgs().WithSignature().MemberInfo.Name);
            Assert.AreEqual(0, ((MethodInfo)mirror["StaticMethod"].WithGenericArgs().WithSignature().MemberInfo).GetParameters().Length);
            Assert.AreEqual("StaticMethod", mirror["StaticMethod"].WithGenericArgs().WithSignature(typeof(int)).MemberInfo.Name);
            Assert.AreEqual(1, ((MethodInfo)mirror["StaticMethod"].WithGenericArgs().WithSignature(typeof(int)).MemberInfo).GetParameters().Length);
            Assert.AreEqual("StaticMethod", mirror["StaticMethod"].WithSignature(typeof(int), typeof(int)).MemberInfo.Name);
            Assert.AreEqual(2, ((MethodInfo)mirror["StaticMethod"].WithSignature(typeof(int), typeof(int)).MemberInfo).GetParameters().Length);

            Assert.AreEqual(5, mirror["StaticMethod"].WithGenericArgs().Invoke());
            Assert.AreEqual(8, mirror["StaticMethod"].WithGenericArgs().Invoke(4));
            Assert.AreEqual(28, mirror["StaticMethod"].Invoke(4, 7));
            Assert.AreEqual(0, mirror["StaticMethod"].WithGenericArgs(typeof(int)).Invoke());
            Assert.AreEqual(4, mirror["StaticMethod"].WithGenericArgs(typeof(int)).Invoke(4));
        }

        [Test]
        public void Syntax_Constructor()
        {
            var obj = new SampleObject();
            var mirror = Mirror.ForObject(obj);

            var ex = Assert.Throws<MirrorException>(() => { var m = mirror.Constructor.MemberInfo; });
            Assert.AreEqual("Could not find a unique matching member '.ctor' of type 'MbUnit.Tests.Framework.MirrorTest+SampleObject'.  There were 2 matches out of 2 members with the same name.  Try providing additional information to narrow down the choices.", ex.Message);

            Assert.AreEqual(".ctor", mirror.Constructor.WithSignature().MemberInfo.Name);
            Assert.AreEqual(0, ((ConstructorInfo)mirror.Constructor.WithSignature().MemberInfo).GetParameters().Length);
            Assert.AreEqual(".ctor", mirror.Constructor.WithSignature(typeof(int)).MemberInfo.Name);
            Assert.AreEqual(1, ((ConstructorInfo)mirror.Constructor.WithSignature(typeof(int)).MemberInfo).GetParameters().Length);

            Assert.AreEqual(0, ((SampleObject)mirror.Constructor.Invoke()).constructorArg);
            Assert.AreEqual(4, ((SampleObject)mirror.Constructor.Invoke(4)).constructorArg);
        }

        [Test]
        public void Syntax_StaticConstructor([Column(false, true)] bool usingObjectMirror)
        {
            var mirror = usingObjectMirror ? Mirror.ForObject(new SampleObject()) : Mirror.ForType<SampleObject>();

            Assert.AreEqual(".cctor", mirror.StaticConstructor.WithSignature().MemberInfo.Name);

            SampleObject.callsToStaticConstructor = 0;
            mirror.StaticConstructor.Invoke();
            Assert.AreEqual(1, SampleObject.callsToStaticConstructor);
        }

        [Test]
        public void Syntax_NestedType([Column(false, true)] bool usingObjectMirror)
        {
            var mirror = usingObjectMirror ? Mirror.ForObject(new SampleObject()) : Mirror.ForType<SampleObject>();

            Assert.AreEqual("NestedType", mirror["NestedType"].MemberInfo.Name);
            Assert.AreEqual("NestedType", mirror["NestedType"].NestedType.Name);
        }

        private class SampleObject
        {
            internal class CustomEventArgs : EventArgs { }
            internal delegate void CustomEventHandler(object sender, CustomEventArgs e);

            internal SampleObject()
            {
            }

            internal SampleObject(int constructorArg)
            {
                this.constructorArg = constructorArg;
            }

            static SampleObject()
            {
                callsToStaticConstructor += 1;
            }

            internal static int callsToStaticConstructor;

            internal int constructorArg;

            internal int InstanceField = 0;
            internal int InstanceProperty { get; set; }
            internal static int StaticField = 0;
            internal static int StaticProperty { get; set; }

            internal CustomEventHandler instanceCustomEvent;
            internal int instanceCustomEventAddRemoveCount; 
            internal static CustomEventHandler staticCustomEvent;
            internal static int staticCustomEventAddRemoveCount; 

            private event CustomEventHandler InstanceCustomEvent
            {
                add { instanceCustomEvent += value; instanceCustomEventAddRemoveCount++; }
                remove { instanceCustomEvent -= value; instanceCustomEventAddRemoveCount++; }
            }

            private static event CustomEventHandler StaticCustomEvent
            {
                add { staticCustomEvent += value; staticCustomEventAddRemoveCount++; }
                remove { staticCustomEvent -= value; staticCustomEventAddRemoveCount++; }
            }

            public void RaiseInstanceCustomEvent()
            {
                if (instanceCustomEvent != null)
                    instanceCustomEvent(this, new CustomEventArgs());
            }

            public static void RaiseStaticCustomEvent()
            {
                if (staticCustomEvent != null)
                    staticCustomEvent(null, new CustomEventArgs());
            }

            internal int indexerLastSetValue;
            private int this[int x] { get { return x * 2; } set { indexerLastSetValue = x * value; } }
            private int this[int x, int y] { get { return x * y; } set { indexerLastSetValue = x* y * value; } }

            private int InstanceMethod()
            {
                return 5;
            }

            private int InstanceMethod(int x)
            {
                return x* 2;
            }

            private int InstanceMethod(int x, int y)
            {
                return x * y;
            }

            private T InstanceMethod<T>()
            {
                return default(T);
            }

            private T InstanceMethod<T>(T x)
            {
                return x;
            }

            private int InstanceMethodForNull(string x)
            {
                return (x ?? String.Empty).Length;
            }

            private int InstanceMethodForNull(int x)
            {
                return x;
            }

            private static int StaticMethod()
            {
                return 5;
            }

            private static int StaticMethod(int x)
            {
                return x * 2;
            }

            private static int StaticMethod(int x, int y)
            {
                return x * y;
            }

            private static T StaticMethod<T>()
            {
                return default(T);
            }

            private static T StaticMethod<T>(T x)
            {
                return x;
            }

            private class NestedType
            {
            }
        }
    }
}
