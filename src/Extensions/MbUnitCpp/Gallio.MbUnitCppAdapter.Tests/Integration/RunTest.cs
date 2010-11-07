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
// WITHOUT WC:\Projects\Gallio\v3\src\Extensions\MbUnitCpp\Gallio.MbUnitCppAdapter.Tests\Model\Bridge\UnmanagedTestRepositoryTest.csARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Tests;
using MbUnit.Framework;
using System.Reflection;
using System.IO;
using Gallio.Runner.Reports.Schema;
using Gallio.Framework;

namespace Gallio.MbUnitCppAdapter.Tests.Integration
{
    [TestFixture]
    public class RunTest : BaseTestWithSampleRunner
    {
        [Column("x86"/*, "x64"*/)]
        private readonly string architecture;

        protected override void ConfigureRunner()
        {
            Runner.AddFile(new FileInfo(Helper.GetTestResources(architecture)));
        }

        private TestStepRun GetTestStepRun(string fullName)
        {
            return Runner.GetTestStepRuns(run => run.Step.FullName == fullName).First();
        }

        [Test]
        public void Empty_passes()
        {
            //TestStepRun step = GetTestStepRun("Sample/")
        }


    
    }
}
