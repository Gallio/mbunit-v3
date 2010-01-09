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
using System.Text;
using Gallio.Common.Platform;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Platform
{
    public class ProcessSupportTest
    {
        [Test]
        [Explicit("Must be verified manually due to significant OS dependencies.")]
        public void PrintProcessInformationForManualInspection()
        {
            TestLog.WriteLine("Is32BitProcess: {0}", ProcessSupport.Is32BitProcess);
            TestLog.WriteLine("Is64BitProcess: {0}", ProcessSupport.Is64BitProcess);
            TestLog.WriteLine("ProcessType: {0}", ProcessSupport.ProcessType);
            TestLog.WriteLine("ProcessIntegrityLevel: {0}", ProcessSupport.ProcessIntegrityLevel);
            TestLog.WriteLine("HasElevatedPrivileges: {0}", ProcessSupport.HasElevatedPrivileges);
        }
    }
}
