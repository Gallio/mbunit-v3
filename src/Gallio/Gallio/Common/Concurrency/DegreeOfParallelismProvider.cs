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

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// A function that returns the maximum number of threads that a work scheduler 
    /// may use to perform work.  The value may change over time and cause the
    /// scheduler to adapt to changing degrees of parallelism.
    /// </summary>
    /// <returns>The degree of parallelism which must be at least 1.</returns>
    /// <seealso cref="WorkScheduler"/>
    public delegate int DegreeOfParallelismProvider();
}
