// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace MbUnit.Plugin.CecilInstrumentation.Tests
{
    [DebuggerDisplay("SampleWrapper")] // prevent ToString from being called automatically
    public class SampleWrapper : ISample
    {
        private readonly object instance;

        public SampleWrapper(object instance)
        {
            this.instance = instance;
        }

        public object Instance
        {
            get { return instance; }
        }

        public void AppendHello()
        {
            instance.GetType().InvokeMember(@"AppendHello", BindingFlags.InvokeMethod |
                BindingFlags.Instance | BindingFlags.Public, null, instance,
                null);
        }

        public void Append(string message)
        {
            instance.GetType().InvokeMember(@"Append", BindingFlags.InvokeMethod |
                BindingFlags.Instance | BindingFlags.Public, null, instance,
                new object[] { message });
        }

        public override string ToString()
        {
            return (string) instance.GetType().InvokeMember(@"ToString", BindingFlags.InvokeMethod |
                BindingFlags.Instance | BindingFlags.Public, null, instance,
                null);
        }
    }
}
