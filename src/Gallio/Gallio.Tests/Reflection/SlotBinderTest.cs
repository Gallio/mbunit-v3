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
using Gallio.Collections;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(SlotBinder))]
    public class SlotBinderTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void CreateInstanceThrowsWhenTypeIsNull()
        {
            SlotBinder.CreateInstance(null, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void CreateInstanceThrowsWhenSlotValuesIsNull()
        {
            SlotBinder.CreateInstance(Mocks.Stub<ITypeInfo>(), null);
        }

        [Test, ExpectedArgumentNullException]
        public void CreateInstanceThrowsWhenSlotValuesContainsANullSlot()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericType));
            SlotBinder.CreateInstance(type, new KeyValuePair<ISlotInfo, object>[]
                {
                    new KeyValuePair<ISlotInfo, object>(null, 42)
                });
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAbstract()
        {
            ITypeInfo type = Reflector.Wrap(typeof(AbstractType));
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAnInterface()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Interface));
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAGenericTypeParameter()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericType<>)).GenericParameters[0];
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAnArray()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericType[]));
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAPointer()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericType).MakePointerType());
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAByRef()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericType).MakeByRefType());
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        private interface Interface
        {
        }

        private abstract class AbstractType
        {
        }

        private struct Struct
        {
        }

        private class NonGenericType
        {
        }

        private class GenericType<T>
        {
        }
    }
}
