// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
    /// <para>
    /// A facade of the TestDriven.Net test listener.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the TestDriven.Net interfaces.
    /// </remarks>
    public interface IFacadeTestListener
    {
        void TestFinished(FacadeTestResult result);
        void TestResultsUrl(string url);
        void WriteLine(string text, FacadeCategory category);
    }
}
