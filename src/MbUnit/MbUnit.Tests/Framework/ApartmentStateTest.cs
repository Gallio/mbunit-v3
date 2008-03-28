// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    public class ApartmentStateTest
    {
        [Test]
        public void DefaultApartmentStateShouldBeSTA()
        {
            Assert.AreEqual(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        }

        [Test, ApartmentState(ApartmentState.MTA)]
        public void CanOverrideApartmentState()
        {
            Assert.AreEqual(ApartmentState.MTA, Thread.CurrentThread.GetApartmentState());
        }

        [Test, ApartmentState(ApartmentState.STA)]
        public void NoChangeIfApartmentStateIsUnchanged()
        {
            Assert.AreEqual(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        }

        [Test, ApartmentState(ApartmentState.Unknown)]
        public void NoChangeIfApartmentStateIsUnknown()
        {
            Assert.AreEqual(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        }

        [TestFixture, ApartmentState(ApartmentState.MTA)]
        public class InheritedApartmentStateTest
        {
            [Test]
            public void InheritsApartmentStateOfParentTest()
            {
                Assert.AreEqual(ApartmentState.MTA, Thread.CurrentThread.GetApartmentState());
            }

            [Test, ApartmentState(ApartmentState.STA)]
            public void CanOverrideTheInheritedApartmentState()
            {
                Assert.AreEqual(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
            }
        }
    }
}
