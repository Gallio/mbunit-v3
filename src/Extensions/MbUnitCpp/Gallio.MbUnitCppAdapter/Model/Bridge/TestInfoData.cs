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
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;
using Gallio.Common.Collections;
using System.Collections.Generic;
using Gallio.Model;
using System.Text.RegularExpressions;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// The structure that warps native information about a MbUnitCpp test or a MbUnitCpp fixture.
    /// </summary>
    public class TestInfoData
    {
        private readonly NativeTestInfoData native;
        private string name;
        private string fileName;
        private KeyValuePair<string, string>[] metadata;

        public TestInfoData(NativeTestInfoData native)
        {
            this.native = native;
        }

        /// <summary>
        /// Returns a unique key identifying the MbUnitCpp test/fixture that may be used to build the model tree.
        /// </summary>
        /// <returns>A unique key name.</returns>
        public string GetId()
        {
            return String.Format("MbUnitCpp_{0}_{1}_{2}", native.Position.pTestFixture, native.Position.pTest, native.Position.pRow);
        }

        /// <summary>
        /// Marshals the test/fixture name.
        /// </summary>
        public string Name
        {
            get
            {
                if (name == null)
                    name = Marshal.PtrToStringUni(native.NamePtr);

                return name;
            }
        }

        /// <summary>
        /// Marshals the source file name where the test/fixture is defined.
        /// </summary>
        public string FileName
        {
            get
            {
                if (fileName == null)
                    fileName = Marshal.PtrToStringUni(native.FileNamePtr);

                return fileName;
            }
        }

        /// <summary>
        /// Gets the test kind.
        /// </summary>
        public NativeTestKind Kind
        {
            get
            {
                return native.Kind;
            }
        }

        /// <summary>
        /// Gets the native unmanaged structure.
        /// </summary>
        public NativeTestInfoData Native
        {
            get
            {
                return native;
            }
        }

        /// <summary>
        /// Determines whether this test is expected to have child tests.
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return (Kind == NativeTestKind.Fixture) || (Kind == NativeTestKind.Group);
            }
        }

        /// <summary>
        /// Builds and returns an artifical stack trace that points to the current test/fixture.
        /// </summary>
        /// <returns></returns>
        public StackTraceData GetStackTraceData()
        {
            return new StackTraceData(String.Format("   at {0} in {1}:line {2}\r\n", Name, FileName, native.LineNumber));
        }

        /// <summary>
        /// Gets a fake code element to fool the Gallio filtering engine.
        /// </summary>
        /// <seealso cref="Gallio.Model.Filters.TypeFilter{T}"/>
        /// <seealso cref="Gallio.Model.Filters.MemberFilter{T}"/>
        public ICodeElementInfo FakeCodeElement
        {
            get
            {
                return (native.Kind == NativeTestKind.Fixture)
                    ? (ICodeElementInfo)new FakeTypeInfo(Name) 
                    : new FakeMemberInfo(Name);
            }
        }

        public KeyValuePair<string, string>[] GetMetadata(IStringResolver resolver)
        {
            if (metadata == null)
                metadata = GenericCollectionUtils.ToArray(EnumerateMetadata(resolver));

            return metadata;
        }

        private IEnumerable<KeyValuePair<string, string>> EnumerateMetadata(IStringResolver resolver)
        {
            string data = resolver.GetString(native.MetadataId);
            var matches = Regex.Matches(data, @"(?<key>\w+)=\{(?<value>[\w\s]+)\}");
            
            foreach (Match match in matches)
            {
                yield return new KeyValuePair<string, string>(match.Groups["key"].Value, match.Groups["value"].Value);
            }
        }
    }
}
