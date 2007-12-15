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

namespace MbUnit.Model
{
    /// <summary>
    /// A delegate used to lazily populate the children of an <see cref="MbUnitTest" />.
    /// </summary>
    /// <param name="recurse">If true, the populator should recursively populate
    /// all of its newly populated test elements in addition to itself</param>
    public delegate void MbUnitTestPopulator(bool recurse);
}
