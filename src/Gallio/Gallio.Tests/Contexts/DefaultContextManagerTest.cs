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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Gallio.Contexts;
using MbUnit.Framework;
using MbUnit.Framework.Concurrency;

namespace Gallio.Tests.Contexts
{
    [TestFixture]
    [TestsOn(typeof(DefaultContextManager))]
    public class DefaultContextManagerTest
    {
        private DefaultContextManager mgr;

        [SetUp]
        public void SetUp()
        {
            mgr = new DefaultContextManager();
        }

        [Test]
        public void TheInitialContextShouldBeNull()
        {
            Assert.IsNull(mgr.CurrentContext);
            Assert.IsNull(mgr.GlobalContext);
        }

        [Test]
        public void TheCurrentThreadInheritsTheGlobalContext()
        {
            StubContext context = new StubContext();
            mgr.GlobalContext = context;

            Assert.AreSame(context, mgr.GlobalContext);
            Assert.AreSame(context, mgr.CurrentContext);

            mgr.GlobalContext = null;
            Assert.IsNull(mgr.GlobalContext);
            Assert.IsNull(mgr.CurrentContext);
        }

        [Test]
        public void TheCurrentThreadUsesTheThreadDefaultContextInsteadOfInheritingTheGlobalContext()
        {
            mgr.GlobalContext = new StubContext();

            StubContext context = new StubContext();
            mgr.SetThreadDefaultContext(Thread.CurrentThread, context);

            Assert.AreSame(context, mgr.GetThreadDefaultContext(Thread.CurrentThread));
            Assert.AreSame(context, mgr.CurrentContext);
            Assert.AreNotSame(context, mgr.GlobalContext);
        }

        [Test]
        public void TheCurrentThreadUsesTheEnteredContextInsteadOfItsDefaultContext()
        {
            StubContext enteredContext = new StubContext();
            mgr.EnterContext(enteredContext);

            StubContext context = new StubContext();
            mgr.SetThreadDefaultContext(Thread.CurrentThread, context);

            Assert.AreSame(context, mgr.GetThreadDefaultContext(Thread.CurrentThread));
            Assert.AreSame(enteredContext, mgr.CurrentContext);
            Assert.IsNull(mgr.GlobalContext);
        }

        [Test]
        public void TheEnteredContextPropagatesAcrossThreadsWithIndependentStacks()
        {
            StubContext rootContext = new StubContext();
            mgr.EnterContext(rootContext);
            Assert.AreSame(rootContext, mgr.CurrentContext);

            TaskManager.StartThreadTask("A", delegate
            {
                Assert.AreSame(rootContext, mgr.CurrentContext);

                StubContext leafContext = new StubContext();
                mgr.EnterContext(leafContext);
                Assert.AreSame(leafContext, mgr.CurrentContext);

                TaskManager.StartThreadTask("B", delegate
                {
                    Assert.AreSame(leafContext, mgr.CurrentContext);

                    StubContext leafContext2 = new StubContext();
                    mgr.EnterContext(leafContext2);
                    Assert.AreSame(leafContext2, mgr.CurrentContext);
                }).Join(new TimeSpan(0, 0, 1));

                Assert.AreSame(leafContext, mgr.CurrentContext);
            }).Join(new TimeSpan(0, 0, 1));

            Assert.AreSame(rootContext, mgr.CurrentContext);

            TaskManager.JoinAndVerify(new TimeSpan(0, 0, 5));
        }

        [Test]
        public void TheEnteredContextIsExitedWhenTheCookieIsDisposed()
        {
            StubContext context1 = new StubContext();
            using (mgr.EnterContext(context1))
            {
                Assert.AreSame(context1, mgr.CurrentContext);

                StubContext context2 = new StubContext();
                using (mgr.EnterContext(context2))
                {
                    Assert.AreSame(context2, mgr.CurrentContext);
                }

                Assert.AreSame(context1, mgr.CurrentContext);
            }

            Assert.IsNull(mgr.CurrentContext);
        }

        [Test]
        public void TheEnteredContextCanBeNullWhichWillOverrideEvenTheGlobalContext()
        {
            StubContext rootContext = new StubContext();
            mgr.GlobalContext = rootContext;

            using (mgr.EnterContext(null))
            {
                Assert.IsNull(mgr.CurrentContext);

                StubContext context1 = new StubContext();
                using (mgr.EnterContext(context1))
                {
                    Assert.AreSame(context1, mgr.CurrentContext);

                    using (mgr.EnterContext(null))
                    {
                        Assert.IsNull(mgr.CurrentContext);

                        StubContext context2 = new StubContext();
                        using (mgr.EnterContext(context2))
                        {
                            Assert.AreSame(context2, mgr.CurrentContext);
                        }

                        Assert.IsNull(mgr.CurrentContext);
                    }

                    Assert.AreSame(context1, mgr.CurrentContext);
                }

                Assert.IsNull(mgr.CurrentContext);
            }

            Assert.AreSame(rootContext, mgr.CurrentContext);
        }

        [Test]
        public void AContextCannotBeExitedTwice()
        {
            ContextCookie cookie = mgr.EnterContext(new StubContext());
            cookie.Dispose();

            InterimAssert.Throws<InvalidOperationException>(delegate { cookie.Dispose(); });
        }

        [Test]
        public void AContextCannotBeExitedOnADifferentThreadFromTheOneThatEnteredIt()
        {
            ContextCookie cookie = mgr.EnterContext(new StubContext());

            TaskManager.StartThreadTask("A different thread.", delegate
            {
                Thread.Sleep(30000);
                InterimAssert.Throws<InvalidOperationException>(delegate { cookie.Dispose(); });
            });

            TaskManager.JoinAndVerify(new TimeSpan(0, 0, 15));
        }
    }
}
