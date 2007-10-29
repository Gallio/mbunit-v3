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
using MbUnit.Attributes;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class RowAttribute : DataPatternAttribute
    {
        // TODO.
        private readonly object[] values;
        private Type expectedException;
        private string description;

        public RowAttribute(params object[] values)
        {
            this.values = values ?? new object[] { null };
        }

        public object[] Values
        {
            get { return values; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Type ExpectedException
        {
            get { return expectedException; }
            set { expectedException = value; }
        }
    }
}
