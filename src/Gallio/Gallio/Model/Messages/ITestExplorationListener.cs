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
using Gallio.Model.Serialization;

namespace Gallio.Model.Messages
{
    /// <summary>
    /// A test exploration listener observes the progress of test exploration as a series of events.
    /// </summary>
    public interface ITestExplorationListener
    {
        /// <summary>
        /// Notifies the listener that a subtree of tests has been merged into the test model.
        /// </summary>
        /// <param name="parentTestId">The id of the parent test, or null if adding the root.</param>
        /// <param name="test">The test at the top of the subtree was merged.</param>
        void NotifySubtreeMerged(string parentTestId, TestData test);

        /// <summary>
        /// Notifies the listener that an annotation has been added to the test model.
        /// </summary>
        /// <param name="annotation">The annotation that was added.</param>
        void NotifyAnnotationAdded(AnnotationData annotation);
    }
}
