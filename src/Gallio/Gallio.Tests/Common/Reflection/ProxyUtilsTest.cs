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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Reflection
{
    public class ProxyUtilsTest
    {
        private delegate void VoidDelegate1();
        private delegate void VoidDelegate2();

        private delegate int IntDelegate1();
        private delegate int IntDelegate2();

        private delegate int IntIntDelegate1(int x);
        private delegate int IntIntDelegate2(int y);

        private delegate int IntOutIntDelegate1(out int x);
        private delegate int IntOutIntDelegate2(out int y);

        private delegate int IntRefIntDelegate1(ref int x);
        private delegate int IntRefIntDelegate2(ref int y);

        private delegate object ObjectRefObjectDelegate1(ref object x);
        private delegate object ObjectRefObjectDelegate2(ref object y);

        private delegate void EventHandlerDelegate1(object sender, EventArgs e);
        private delegate void EventHandlerDelegate2(object sender, EventArgs e);

        [Test]
        public void CoerceDelegate_WhenTargetDelegateTypeIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ProxyUtils.CoerceDelegate(null, new VoidDelegate1(() => { })));
        }

        [Test]
        public void CoerceDelegate_WhenTargetDelegateTypeIsNotAValidDelegateType_Throws()
        {
            Assert.Throws<ArgumentException>(() => ProxyUtils.CoerceDelegate(typeof(object), new VoidDelegate1(() => { })));
        }

        [Test]
        public void CoerceDelegate_WhenSourceDelegateIsNull_ReturnsNull()
        {
            Assert.IsNull(ProxyUtils.CoerceDelegate(typeof(VoidDelegate1), null));
        }

        [Test]
        public void CoerceDelegate_WhenSourceDelegateIsAssignableToTargetDelegateType_ReturnsSameDelegate()
        {
            VoidDelegate1 sourceDelegate = () => { };

            var targetDelegate = (VoidDelegate1)ProxyUtils.CoerceDelegate(typeof(VoidDelegate1), sourceDelegate);

            Assert.AreSame(sourceDelegate, targetDelegate);
        }

        [Test]
        public void CoerceDelegate_WhenDelegateTypesIncompatible_Throws()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<InvalidOperationException>(() => ProxyUtils.CoerceDelegate(
                    typeof(VoidDelegate1), new IntDelegate1(() => 42)));
                Assert.Throws<InvalidOperationException>(() => ProxyUtils.CoerceDelegate(
                    typeof(IntDelegate1), new IntIntDelegate1(x => 42)));
                Assert.Throws<InvalidOperationException>(() => ProxyUtils.CoerceDelegate(
                    typeof(IntIntDelegate1), new IntOutIntDelegate1((out int x) => { x = 42; return 42; })));
                Assert.Throws<InvalidOperationException>(() => ProxyUtils.CoerceDelegate(
                    typeof(IntIntDelegate1), new IntRefIntDelegate1((ref int x) => { x = 42; return 42; })));
                Assert.Throws<InvalidOperationException>(() => ProxyUtils.CoerceDelegate(
                    typeof(IntOutIntDelegate1), new IntRefIntDelegate1((ref int x) => { x = 42; return 42; })));
            });
        }

        [Test]
        public void CoerceDelegate_Void_To_Void()
        {
            bool called = false;
            VoidDelegate1 d1 = () => { called = true; };
            var d2 = (VoidDelegate2)ProxyUtils.CoerceDelegate(typeof(VoidDelegate2), d1);

            d2();

            Assert.IsTrue(called);
        }

        [Test]
        public void CoerceDelegate_IntInt_To_IntInt()
        {
            bool called = false;
            IntIntDelegate1 d1 = x => { called = true; return x * x; };
            var d2 = (IntIntDelegate2)ProxyUtils.CoerceDelegate(typeof(IntIntDelegate2), d1);

            int result = d2(4);

            Assert.IsTrue(called);
            Assert.AreEqual(16, result);
        }

        [Test]
        public void CoerceDelegate_IntRefInt_To_IntRefInt()
        {
            bool called = false;
            IntRefIntDelegate1 d1 = (ref int x) => { called = true; return x * x++; };
            var d2 = (IntRefIntDelegate2)ProxyUtils.CoerceDelegate(typeof(IntRefIntDelegate2), d1);

            int arg = 4;
            int result = d2(ref arg);

            Assert.IsTrue(called);
            Assert.AreEqual(16, result);
            Assert.AreEqual(5, arg);
        }

        [Test]
        public void CoerceDelegate_IntOutInt_To_IntOutInt()
        {
            bool called = false;
            IntOutIntDelegate1 d1 = (out int x) => { called = true; x = 4; return x * x; };
            var d2 = (IntOutIntDelegate2)ProxyUtils.CoerceDelegate(typeof(IntOutIntDelegate2), d1);

            int arg;
            int result = d2(out arg);

            Assert.IsTrue(called);
            Assert.AreEqual(16, result);
            Assert.AreEqual(4, arg);
        }

        [Test]
        public void CoerceDelegate_IntRefInt_To_ObjectRefObject()
        {
            bool called = false;
            IntRefIntDelegate1 d1 = (ref int x) => { called = true; return x * x++; };
            var d2 = (ObjectRefObjectDelegate1)ProxyUtils.CoerceDelegate(typeof(ObjectRefObjectDelegate1), d1);

            object arg = 4;
            object result = d2(ref arg);

            Assert.IsTrue(called);
            Assert.AreEqual(16, result);
            Assert.AreEqual(5, arg);
        }

        [Test]
        public void CoerceDelegate_ObjectRefObject_To_IntRefInt()
        {
            bool called = false;
            ObjectRefObjectDelegate1 d1 = (ref object x) => { called = true; int y = (int)x; x = y + 1; return y * y; };
            var d2 = (IntRefIntDelegate2)ProxyUtils.CoerceDelegate(typeof(IntRefIntDelegate2), d1);

            int arg = 4;
            int result = d2(ref arg);

            Assert.IsTrue(called);
            Assert.AreEqual(16, result);
            Assert.AreEqual(5, arg);
        }

        [Test]
        public void CoerceDelegate_ObjectRefObject_To_ObjectRefObject()
        {
            bool called = false;
            ObjectRefObjectDelegate1 d1 = (ref object x) => { called = true; int y = (int)x; x = y + 1; return y * y; };
            var d2 = (ObjectRefObjectDelegate2)ProxyUtils.CoerceDelegate(typeof(ObjectRefObjectDelegate2), d1);

            object arg = 4;
            object result = d2(ref arg);

            Assert.IsTrue(called);
            Assert.AreEqual(16, result);
            Assert.AreEqual(5, arg);
        }

        [Test]
        public void CoerceDelegate_EventHandler_To_EventHandler()
        {
            bool called = false;
            object actualSender = null;
            EventArgs actualEventArgs = null;
            EventHandlerDelegate1 d1 = (sender, e) => { called = true; actualSender = sender; actualEventArgs = e; };
            var d2 = (EventHandlerDelegate2)ProxyUtils.CoerceDelegate(typeof(EventHandlerDelegate2), d1);

            object expectedSender = this;
            EventArgs expectedEventArgs = EventArgs.Empty;
            d2(expectedSender, expectedEventArgs);

            Assert.IsTrue(called);
            Assert.AreSame(expectedSender, actualSender);
            Assert.AreSame(expectedEventArgs, actualEventArgs);
        }
    }
}
