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

using Gallio.Runtime.ProgressMonitoring;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Utilities
{
    internal class MockProgressMonitor
    {
        public static IProgressMonitor Instance
        {
            get
            {
                var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
                var progressMonitorTaskCookie = new ProgressMonitorTaskCookie(progressMonitor);
                progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, 
                    Arg<double>.Is.Anything)).Return(progressMonitorTaskCookie);
                progressMonitor.Stub(x => x.CreateSubProgressMonitor(Arg<double>.Is.Anything))
                    .Return(progressMonitor).Repeat.Any();
                return progressMonitor;
            }
        }
    }
}