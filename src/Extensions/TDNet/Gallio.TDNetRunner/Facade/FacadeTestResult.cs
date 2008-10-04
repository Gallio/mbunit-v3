// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Text;

namespace Gallio.TDNetRunner.Facade
{
    /// <summary>
    /// Describes a test result.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the TestDriven.Net interfaces.
    /// </remarks>
    [Serializable]
    public class FacadeTestResult
    {
        public FacadeTestState State { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string StackTrace { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public int TotalTests { get; set; }
    }
}
