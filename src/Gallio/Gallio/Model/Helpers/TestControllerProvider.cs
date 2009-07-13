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

using Gallio.Model.Tree;

namespace Gallio.Model.Helpers
{
    /// <summary>
    /// Gets a test controller for a subtree of tests.
    /// </summary>
    /// <param name="test">The test.</param>
    /// <returns>The test controller or null if there is no specific controller
    /// available for the test so the test should be executed using a default rule.</returns>
    public delegate TestController TestControllerProvider(Test test);
}