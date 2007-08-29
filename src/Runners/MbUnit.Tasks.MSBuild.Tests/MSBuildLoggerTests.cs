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

extern alias MbUnit2;
using MbUnit2::MbUnit.Framework;
using System;
using MbUnit.Tasks.MSBuild;
using Microsoft.Build.Utilities;

namespace MbUnit.Tasks.MSBuild.Tests
{
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MSBuildLogger))]
    public class MSBuildLoggerTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstantiateLogeerWithNullArgument()
        {
            MSBuildLogger logger = new MSBuildLogger(null);
        }

        [Test]
        public void InstantiateLogger()
        {
            MbUnit task = new MbUnit();
            TaskLoggingHelper taskLogger = new TaskLoggingHelper(task);
            new MSBuildLogger(taskLogger);
        }

        [Test]
        public void CreateChildLogger()
        {
            MbUnit task = new MbUnit();
            TaskLoggingHelper taskLogger = new TaskLoggingHelper(task);
            MSBuildLogger logger = new MSBuildLogger(taskLogger);
            Assert.AreSame(logger.CreateChildLogger("child").GetType(), typeof (MSBuildLogger));
        }
    }
}
