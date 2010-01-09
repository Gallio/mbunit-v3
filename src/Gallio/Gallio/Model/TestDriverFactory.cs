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
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Model
{
    /// <summary>
    /// A factory for test drivers.
    /// </summary>
    /// <param name="testFrameworkHandles">The component handles of frameworks which share this test driver factory, not null.</param>
    /// <param name="testFrameworkOptions">The test framework options, not null.</param>
    /// <param name="logger">The logger for the test driver, not null.</param>
    public delegate ITestDriver TestDriverFactory(
        IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> testFrameworkHandles,
        TestFrameworkOptions testFrameworkOptions,
        ILogger logger);
}
