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
using Gallio.Common;
using MbUnit.Framework;

namespace Gallio.Tests.Common
{
    [TestFixture]
    [TestsOn(typeof(ActionChain<,>))]
    public class ActionChainTestForTwoParameters
    {
        private ActionChain<string, int> chain;
        private List<string> trace;

        [SetUp]
        public void SetUp()
        {
            chain = new ActionChain<string, int>();
            trace = new List<string>();
        }

        [Test]
        public void GetAndSetAction()
        {
            Assert.AreEqual(ActionChain<string, int>.NoOp, chain.Action);

            Gallio.Common.Action<string, int> action = CreateAction("", 0);
            chain.Action = action;
            Assert.AreEqual(action, chain.Action);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetAction_ThrowsIfValueIfNull()
        {
            chain.Action = null;
        }

        [Test]
        public void Before()
        {
            chain.Before(CreateAction("abc", 0));
            chain.Before(CreateAction("def", 1));
            chain.Before(CreateAction("ghi", 2));
            chain.Action("key", 42);

            AssertTraceEquals("key,42: ghi,2", "key,42: def,1", "key,42: abc,0");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Before_ThrowsIfActionIsNull()
        {
            chain.Before(null);
        }

        [Test]
        public void After()
        {
            chain.After(CreateAction("abc", 0));
            chain.After(CreateAction("def", 1));
            chain.After(CreateAction("ghi", 2));
            chain.Action("key", 42);

            AssertTraceEquals("key,42: abc,0", "key,42: def,1", "key,42: ghi,2");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void After_ThrowsIfActionIsNull()
        {
            chain.After(null);
        }

        [Test]
        public void Around()
        {
            chain.Around(CreateActionDecorator("abc", "xyz", 0));
            chain.Around(CreateActionDecorator("def", "uvw", 1));
            chain.Around(CreateActionDecorator("ghi", "rst", 2));
            chain.Action("key", 42);

            AssertTraceEquals("key,42: ghi,2", "key,42: def,1", "key,42: abc,0", "key,42: xyz,0", "key,42: uvw,1", "key,42: rst,2");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Around_ThrowsIfActionIsNull()
        {
            chain.Around(null);
        }

        [Test]
        public void ClearSetsTheActionBackToNoOp()
        {
            chain.Before(CreateAction("abc", 0));
            Assert.AreNotEqual(ActionChain<string, int>.NoOp, chain.Action);

            chain.Clear();
            Assert.AreEqual(ActionChain<string, int>.NoOp, chain.Action);
        }

        private void AssertTraceEquals(params string[] expectedTrace)
        {
            Assert.AreElementsEqual(expectedTrace, trace);
        }

        private void Trace(string arg1, int arg2, string token, int value)
        {
            trace.Add(arg1 + "," + arg2 + ": " + token + "," + value);
        }

        private Gallio.Common.Action<string, int> CreateAction(string token, int value)
        {
            return delegate(string arg1, int arg2)
            {
                Trace(arg1, arg2, token, value);
            };
        }

        private ActionDecorator<string, int> CreateActionDecorator(string beforeToken, string afterToken, int value)
        {
            return delegate(string arg1, int arg2, Gallio.Common.Action<string, int> action)
            {
                Trace(arg1, arg2, beforeToken, value);
                action(arg1, arg2);
                Trace(arg1, arg2, afterToken, value);
            };
        }
    }
}